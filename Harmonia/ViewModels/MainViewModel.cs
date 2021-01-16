using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Harmonia.Models;
using Harmonia.Properties;
using Harmonia.Services.Interfaces;
using MahApps.Metro.Controls.Dialogs;
using Notifications.Wpf.Core;

namespace Harmonia.ViewModels
{
    public class MainViewModel
    {
        private readonly ObservableCollection<DownloadItem> _downloadItems = new ObservableCollection<DownloadItem>();
        private readonly Regex YouTubeUrlRegex = new Regex(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]{11})+");
        private readonly IYouTubeDownloadService _youTubeDownloadService;
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IConversionService _conversionService;
        private readonly IMp3TagService _mp3TagService;
        private readonly IAudioNormalizerService _audioNormalizerService;
        private readonly INotificationManager _notificationManager;
        private readonly IAutoUpdateService _autoUpdateService;

        public MainViewModel(
            IYouTubeDownloadService youTubeDownloadService,
            IDialogCoordinator dialogCoordinator,
            IConversionService conversionService,
            IMp3TagService mp3TagService,
            IAudioNormalizerService audioNormalizerService,
            INotificationManager notificationManager,
            IAutoUpdateService autoUpdateService)
        {
            _youTubeDownloadService = youTubeDownloadService;
            _dialogCoordinator = dialogCoordinator;
            _conversionService = conversionService;
            _mp3TagService = mp3TagService;
            _audioNormalizerService = audioNormalizerService;
            _notificationManager = notificationManager;
            _autoUpdateService = autoUpdateService;

            DownloadItems = new ReadOnlyObservableCollection<DownloadItem>(_downloadItems);
        }

        public ReadOnlyObservableCollection<DownloadItem> DownloadItems { get; }

        public void AddDownloadItem(DownloadItem downloadItem) => _downloadItems.Add(downloadItem);

        public async Task AddDownloadItem(string clipboardText)
        {
            var youTubeMatch = YouTubeUrlRegex.Match(clipboardText);
            if (!youTubeMatch.Success)
            {
                return;
            }

            var youTubeId = youTubeMatch.Groups[1].Value;
            if (_downloadItems.Any(di => di.YouTubeId == youTubeId))
            {
                return;
            }

            var downloadItem = new DownloadItem(youTubeId);
            _downloadItems.Insert(0, downloadItem);

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

                await ShowToast(MainResources.VideoMetaDataToast_Error, NotificationType.Error);

                await ShowErrorDialog(ex);
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

                await ShowToast(MainResources.DownloadCompleteToast_Success, NotificationType.Success);
            }
            catch (Exception ex)
            {
                await ShowToast(MainResources.DownloadCompleteToast_Error, NotificationType.Error);

                await ShowErrorDialog(ex);
            }
        }

        public void DeleteDownloadItem(DownloadItem downloadItem)
            => _downloadItems.Remove(downloadItem);

        public async Task PerformUpdateAsync()
        {
            try
            {
                if (!await _autoUpdateService.CanPerformUpdateAsync())
                {
                    return;
                }

                var result = await _dialogCoordinator.ShowMessageAsync(
                    this,
                    MainResources.UpdateAvailable_Title,
                    MainResources.UpdateAvailable_Message,
                    MessageDialogStyle.AffirmativeAndNegative,
                    new MetroDialogSettings
                    {
                        DefaultButtonFocus = MessageDialogResult.Affirmative,
                        AnimateHide = false
                    });

                if (result == MessageDialogResult.Affirmative)
                {
                    var progressDialog = await _dialogCoordinator.ShowProgressAsync(
                        this,
                        MainResources.Updating_Title,
                        MainResources.Updating_Message,
                        isCancelable: false,
                        settings: new MetroDialogSettings
                        {
                            AnimateHide = false,
                            AnimateShow = false
                        });

                    await _autoUpdateService.PerformUpdateAsync(progressDialog);
                }
            }
            catch (Exception ex)
            {
                await ShowToast(MainResources.Update_Error, NotificationType.Error);

                await ShowErrorDialog(ex);
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

        private async Task ShowErrorDialog(Exception ex)
        {
            await _dialogCoordinator.ShowMessageAsync(
                context: this,
                title: CommonResources.ErrorOccurredTitle,
                message: string.Format(CommonResources.ErrorOccurredMessage, ex.Message));
        }

        private async Task ShowToast(string message, NotificationType notificationType)
        {
            await _notificationManager.ShowAsync(
                new NotificationContent
                {
                    Title = CommonResources.Harmonia,
                    Message = message,
                    Type = notificationType
                }, expirationTime: TimeSpan.FromSeconds(10));
        }
    }
}