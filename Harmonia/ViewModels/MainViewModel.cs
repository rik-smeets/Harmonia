using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Harmonia.Models;
using Harmonia.Properties;
using Harmonia.Services.Interfaces;
using MahApps.Metro.Controls.Dialogs;
using Notifications.Wpf;

namespace Harmonia.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<DownloadItem> DownloadItems { get; } = new ObservableCollection<DownloadItem>();

        private readonly Regex YouTubeUrlRegex = new Regex(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]{11})+");
        private readonly IList<string> _previousMatches = new List<string>();
        private readonly IYouTubeDownloadService _youTubeDownloadService;
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IConversionService _conversionService;
        private readonly IMp3TagService _mp3TagService;
        private readonly IAudioNormalizerService _audioNormalizerService;
        private readonly INotificationManager _notificationManager;

        public MainViewModel(
            IYouTubeDownloadService youTubeDownloadService,
            IDialogCoordinator dialogCoordinator,
            IConversionService conversionService,
            IMp3TagService mp3TagService,
            IAudioNormalizerService audioNormalizerService,
            INotificationManager notificationManager)
        {
            _youTubeDownloadService = youTubeDownloadService;
            _dialogCoordinator = dialogCoordinator;
            _conversionService = conversionService;
            _mp3TagService = mp3TagService;
            _audioNormalizerService = audioNormalizerService;
            _notificationManager = notificationManager;
        }

        public async Task AddDownloadItem(string clipboardText)
        {
            var youTubeMatch = YouTubeUrlRegex.Match(clipboardText);
            if (!youTubeMatch.Success)
            {
                return;
            }

            var youTubeId = youTubeMatch.Groups[1].Value;
            if (_previousMatches.Contains(youTubeId))
            {
                return;
            }

            _previousMatches.Add(youTubeId);
            var downloadItem = new DownloadItem(youTubeId);
            DownloadItems.Insert(0, downloadItem);

            try
            {
                var videoInfo = await _youTubeDownloadService.GetVideo(youTubeId);

                var suggestions = videoInfo.Title.Split(new[] { "-" }, 2, StringSplitOptions.RemoveEmptyEntries);
                var artistSuggestion = suggestions[0].Trim();
                var titleSuggestion = suggestions.Length > 1 ? suggestions[1].Trim() : artistSuggestion;

                downloadItem.Artist = artistSuggestion;
                downloadItem.Title = titleSuggestion;
            }
            catch (Exception ex)
            {
                downloadItem.SetFailed();

                ShowToast(MainResources.VideoMetaDataToast_Error, NotificationType.Error);

                await ShowDialog(ex);
            }
        }

        public async Task StartDownloads()
        {
            var newDownloads = DownloadItems
                .Where(downloadItem => !downloadItem.IsRunningOrCompleted)
                .ToArray();
            if (!newDownloads.Any())
            {
                return;
            };

            try
            {
                await Task.WhenAll(newDownloads.Select(async downloadItem =>
                {
                    await HandleDownload(downloadItem);
                }));

                ShowToast(MainResources.DownloadCompleteToast_Success, NotificationType.Success);
            }
            catch (Exception ex)
            {
                ShowToast(MainResources.DownloadCompleteToast_Error, NotificationType.Error);

                await ShowDialog(ex);
            }
        }

        private async Task HandleDownload(DownloadItem downloadItem)
        {
            await Task.Run(async () =>
            {
                try
                {
                    downloadItem.Status = MainResources.Running;
                    downloadItem.IsRunning = true;

                    var videoInfo = await _youTubeDownloadService.GetVideo(downloadItem.YouTubeId);

                    var formattedArtistTitle = $"{downloadItem.Artist} - {downloadItem.Title}";

                    var mp4Path = await _youTubeDownloadService.DownloadMp4AndGetPath(videoInfo, formattedArtistTitle);

                    downloadItem.Completion = 25;

                    var mp3Path = await _conversionService.ConvertMp4ToMp3AndGetMp3Path(mp4Path);

                    downloadItem.Completion = 50;

                    _audioNormalizerService.NormalizeAudio(mp3Path);

                    downloadItem.Completion = 70;

                    _mp3TagService.SetMp3Tags(mp3Path, downloadItem.Artist, downloadItem.Title);

                    downloadItem.Completion = 100;
                    downloadItem.IsCompleted = true;
                    downloadItem.IsRunning = false;
                    downloadItem.Status = MainResources.Completed;
                }
                catch (Exception)
                {
                    downloadItem.SetFailed();

                    throw;
                }
            });
        }

        private async Task ShowDialog(Exception ex)
        {
            await _dialogCoordinator.ShowMessageAsync(
                context: this,
                title: CommonResources.ErrorOccurredTitle,
                message: string.Format(CommonResources.ErrorOccurredMessage, ex.Message));
        }

        private void ShowToast(string message, NotificationType notificationType)
        {
            _notificationManager.Show(
                new NotificationContent
                {
                    Title = CommonResources.Harmonia,
                    Message = message,
                    Type = notificationType
                }, expirationTime: TimeSpan.FromSeconds(10));
        }
    }
}