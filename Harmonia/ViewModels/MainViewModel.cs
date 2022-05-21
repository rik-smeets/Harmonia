using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Shell;
using Harmonia.Models;
using Harmonia.Properties;
using Harmonia.Services.Interfaces;
using MahApps.Metro.Controls.Dialogs;

namespace Harmonia.ViewModels
{
    public class MainViewModel
    {
        private readonly ObservableCollection<DownloadItem> _downloadItems = new();
        private readonly Regex YouTubeUrlRegex = new(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]{11})+");
        private readonly IYouTubeDownloadService _youTubeDownloadService;
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IConversionService _conversionService;
        private readonly IMp3TagService _mp3TagService;
        private readonly IAudioNormalizerService _audioNormalizerService;
        private readonly IAutoUpdateService _autoUpdateService;

        public MainViewModel(
            IYouTubeDownloadService youTubeDownloadService,
            IDialogCoordinator dialogCoordinator,
            IConversionService conversionService,
            IMp3TagService mp3TagService,
            IAudioNormalizerService audioNormalizerService,
            IAutoUpdateService autoUpdateService)
        {
            _youTubeDownloadService = youTubeDownloadService;
            _dialogCoordinator = dialogCoordinator;
            _conversionService = conversionService;
            _mp3TagService = mp3TagService;
            _audioNormalizerService = audioNormalizerService;
            _autoUpdateService = autoUpdateService;

            DownloadItems = new ReadOnlyObservableCollection<DownloadItem>(_downloadItems);
        }

        public ReadOnlyObservableCollection<DownloadItem> DownloadItems { get; }

        public event EventHandler<DownloadProgressEventArgs> DownloadProgress;

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
                InvokeDownloadProgressEvent(TaskbarItemProgressState.Normal);

                return;
            };

            InvokeDownloadProgressEvent(TaskbarItemProgressState.Indeterminate);

            try
            {
                await Task.WhenAll(newDownloads.Select(async downloadItem =>
                {
                    await HandleDownload(downloadItem);

                    InvokeDownloadProgressEvent(
                        TaskbarItemProgressState.Normal,
                        DownloadItems.Count,
                        DownloadItems.Where(di => di.IsCompleted).Count());
                }));
            }
            catch (Exception ex)
            {
                InvokeDownloadProgressEvent(TaskbarItemProgressState.Error);

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

                if (result is not MessageDialogResult.Affirmative)
                {
                    return;
                }

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
            catch (Exception ex)
            {
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

        private void InvokeDownloadProgressEvent(TaskbarItemProgressState state) => InvokeDownloadProgressEvent(state, default, default);

        private void InvokeDownloadProgressEvent(TaskbarItemProgressState state, double totalItems, int finishedItems)
        {
            DownloadProgress?.Invoke(this, new DownloadProgressEventArgs
            {
                State = state,
                TotalItems = totalItems,
                FinishedItems = finishedItems,
            });
        }
    }
}