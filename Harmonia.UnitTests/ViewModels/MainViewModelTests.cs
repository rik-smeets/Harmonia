using System.Windows.Shell;
using Harmonia.Models;
using Harmonia.Properties;
using Harmonia.Services.Interfaces;
using Harmonia.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Videos;

namespace Harmonia.UnitTests.ViewModels
{
    [TestClass]
    public class MainViewModelTests
    {
        private MockRepository _mockRepository;
        private Mock<IYouTubeDownloadService> _youTubeDownloadServiceMock;
        private Mock<IDialogCoordinator> _dialogCoordinatorMock;
        private Mock<IConversionService> _conversionServiceMock;
        private Mock<IMp3TagService> _mp3TagServiceMock;
        private Mock<IAudioNormalizerService> _audioNormalizerServiceMock;
        private Mock<IAutoUpdateService> _autoUpdateServiceMock;
        private MainViewModel _mainViewModel;
        private const string ValidVideoId = "dQw4w9WgXcQ";
        private const string ValidYouTubeUrl = "youtube.com/watch?v=" + ValidVideoId;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            _youTubeDownloadServiceMock = _mockRepository.Create<IYouTubeDownloadService>();
            _dialogCoordinatorMock = _mockRepository.Create<IDialogCoordinator>();
            _conversionServiceMock = _mockRepository.Create<IConversionService>();
            _mp3TagServiceMock = _mockRepository.Create<IMp3TagService>();
            _audioNormalizerServiceMock = _mockRepository.Create<IAudioNormalizerService>();
            _autoUpdateServiceMock = _mockRepository.Create<IAutoUpdateService>();

            _mainViewModel = new MainViewModel(
                _youTubeDownloadServiceMock.Object,
                _dialogCoordinatorMock.Object,
                _conversionServiceMock.Object,
                _mp3TagServiceMock.Object,
                _audioNormalizerServiceMock.Object,
                _autoUpdateServiceMock.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockRepository.VerifyAll();
            _mockRepository.VerifyNoOtherCalls();
        }

        [TestMethod]
        [DataRow("https://www.google.com",
            DisplayName = "Random other website")]
        [DataRow("https://www.youtube.com",
            DisplayName = "youtube.com without video ID")]
        [DataRow("youtu.be",
            DisplayName = "youtu.be without HTTPS")]
        [DataRow("https://youtu.be",
            DisplayName = "youtu.be with HTTPS")]
        [DataRow("youtu.be/dQw4w9WgXc",
            DisplayName = "Video ID too short in shortened URL")]
        [DataRow("https://youtube.com/dQw4w9WgXc",
            DisplayName = "Video ID too short in full URL")]
        public async Task AddDownloadItem_WhenCalledWithClipboardTextNotMatchingYouTubeUrl_ThenDoesNothing(string clipboardText)
        {
            await _mainViewModel.AddDownloadItem(clipboardText);

            _mainViewModel.DownloadItems.ShouldBeEmpty();
        }

        [TestMethod]
        [DataRow("https://www.youtube.com/watch?v=" + ValidVideoId,
            DisplayName = "Full YouTube URL with video ID in query string with HTTPS")]
        [DataRow("www.youtube.com/watch?v=" + ValidVideoId,
            DisplayName = "Full YouTube URL with video ID in query string without HTTPS")]
        [DataRow("youtube.com/watch?v=" + ValidVideoId,
            DisplayName = "Full YouTube URL with video ID in query string without HTTPS and without WWW")]
        [DataRow("https://www.youtube.com/v/" + ValidVideoId,
            DisplayName = "Full YouTube URL with video ID in /v/ path with HTTPS")]
        [DataRow("www.youtube.com/v/" + ValidVideoId,
            DisplayName = "Full YouTube URL with video ID in /v/ path without HTTPS")]
        [DataRow("youtube.com/v/" + ValidVideoId,
            DisplayName = "Full YouTube URL with video ID in /v/ path without HTTPS and without WWW")]
        [DataRow("https://youtu.be/" + ValidVideoId,
            DisplayName = "Shortened YouTu.be URL with HTTPS")]
        [DataRow("youtu.be/" + ValidVideoId,
            DisplayName = "Shortened YouTu.be URL without HTTPS")]
        [DataRow("https://www.youtube.com/watch?v=" + ValidVideoId + "&list=SomeFakeList&index=1",
            DisplayName = "Full YouTube URL with more query string variables after video")]
        public async Task AddDownloadItem_WhenCalledWithClipboardTextMatchingYouTubeUrl_ThenAddsDownloadItem(string clipboardText)
        {
            _youTubeDownloadServiceMock
                .Setup(m => m.GetVideo(ValidVideoId))
                .ReturnsAsync(CreateVideo());

            await _mainViewModel.AddDownloadItem(clipboardText);

            _mainViewModel.DownloadItems.ShouldHaveSingleItem();
        }

        [TestMethod]
        [DataRow("Rick Astley - Never Gonna Give You Up", "Rick Astley", "Never Gonna Give You Up",
            DisplayName = "Artist - Title")]
        [DataRow(" Rick Astley - Never Gonna Give You Up ", "Rick Astley", "Never Gonna Give You Up",
            DisplayName = "Artist - Title with extra spaces which should be trimmed")]
        [DataRow("Never Gonna Give You Up", "Never Gonna Give You Up", "Never Gonna Give You Up",
            DisplayName = "No '-' in title, so both artist and title should be the same since we don't know")]
        public async Task AddDownloadItem_WhenCalledWithValidUrl_ThenFillsArtistItemProperties(
            string videoTitle, string expectedArtist, string expectedTitle)
        {
            _youTubeDownloadServiceMock
                .Setup(m => m.GetVideo(ValidVideoId))
                .ReturnsAsync(CreateVideo(videoTitle));

            await _mainViewModel.AddDownloadItem(ValidYouTubeUrl);

            var downloadItem = _mainViewModel.DownloadItems.ShouldHaveSingleItem();

            downloadItem.Artist.ShouldBe(expectedArtist);
            downloadItem.Title.ShouldBe(expectedTitle);

            downloadItem.YouTubeId.ShouldBe(ValidVideoId);
            downloadItem.IsFailed.ShouldBeFalse();
            downloadItem.Status.ShouldBe(MainResources.New);
            downloadItem.Completion.ShouldBe(0);
        }

        [TestMethod]
        public async Task AddDownloadItem_WhenSameVideoIdHasAlreadyBeenAdded_ThenDoesNotAddAgain()
        {
            _youTubeDownloadServiceMock
                .Setup(m => m.GetVideo(ValidVideoId))
                .ReturnsAsync(CreateVideo());

            await _mainViewModel.AddDownloadItem(ValidYouTubeUrl);

            _mainViewModel.DownloadItems.ShouldHaveSingleItem();

            await _mainViewModel.AddDownloadItem(ValidYouTubeUrl);

            _mainViewModel.DownloadItems.ShouldHaveSingleItem();
        }

        [TestMethod]
        public async Task AddDownloadItem_WhenGettingVideoInfoFails_ThenThrowsExceptionSetsDownloadItemToFailedAndShowsDialog()
        {
            var expectedMessage = "Some error";

            _youTubeDownloadServiceMock
                .Setup(m => m.GetVideo(ValidVideoId))
                .Throws(new VideoUnavailableException(expectedMessage));

            SetupErrorDialogMock(expectedMessage);

            await _mainViewModel.AddDownloadItem(ValidYouTubeUrl);

            var downloadItem = _mainViewModel.DownloadItems.ShouldHaveSingleItem();

            ValidateFailedDownload(downloadItem);
        }

        [TestMethod]
        public async Task StartDownloads_WhenCalled_ThenPerformsAllConversionStepsFinishesDownloadItemAndUpdatesProgress()
        {
            var downloadItem = new DownloadItem(ValidVideoId);
            _mainViewModel.AddDownloadItem(downloadItem);

            var videoInfo = CreateVideo();
            SetupDownloadItemMocks(downloadItem, videoInfo);

            var downloadProgressEvents = new List<DownloadProgressEventArgs>();
            _mainViewModel.DownloadProgress += (s, args) =>
            {
                downloadProgressEvents.Add(args);
            };

            await _mainViewModel.StartDownloads();

            downloadItem.Completion.ShouldBe(100);
            downloadItem.IsCompleted.ShouldBe(true);
            downloadItem.IsRunning.ShouldBe(false);
            downloadItem.Status.ShouldBe(MainResources.Completed);

            downloadProgressEvents.Count.ShouldBe(2);
            downloadProgressEvents[0].State.ShouldBe(TaskbarItemProgressState.Indeterminate);

            downloadProgressEvents[1].State.ShouldBe(TaskbarItemProgressState.Normal);
            downloadProgressEvents[1].Value.ShouldBe(1);
        }

        [TestMethod]
        public async Task StartDownloads_WhenCalledButDownloadItemIsRunning_ThenDoesNothing()
        {
            var downloadItem = new DownloadItem(ValidVideoId)
            {
                IsRunning = true
            };

            _mainViewModel.AddDownloadItem(downloadItem);

            await _mainViewModel.StartDownloads();
        }

        [TestMethod]
        public async Task StartDownloads_WhenCalledButDownloadItemIsCompleted_ThenDoesNothing()
        {
            var downloadItem = new DownloadItem(ValidVideoId)
            {
                IsCompleted = true
            };

            _mainViewModel.AddDownloadItem(downloadItem);

            await _mainViewModel.StartDownloads();
        }

        [TestMethod]
        public async Task StartDownloads_WhenCalledButDownloadFails_ThenShowsDialogAndUpdatesDownloadProgress()
        {
            var expectedMessage = "Some error";

            var downloadItem = new DownloadItem(ValidVideoId);
            _mainViewModel.AddDownloadItem(downloadItem);

            _youTubeDownloadServiceMock
                .Setup(m => m.GetVideo(downloadItem.YouTubeId))
                .Throws(new VideoUnavailableException(expectedMessage));

            SetupErrorDialogMock(expectedMessage);

            var downloadProgressEvents = new List<DownloadProgressEventArgs>();
            _mainViewModel.DownloadProgress += (s, args) =>
            {
                downloadProgressEvents.Add(args);
            };

            await _mainViewModel.StartDownloads();

            ValidateFailedDownload(downloadItem);
            downloadProgressEvents.Count.ShouldBe(2);
            downloadProgressEvents[1].State.ShouldBe(TaskbarItemProgressState.Error);
        }

        [TestMethod]
        public async Task PerformUpdateAsync_WhenCalledButNoUpdateAvailable_ThenDoesNothing()
        {
            _autoUpdateServiceMock
                .Setup(m => m.CanPerformUpdateAsync())
                .ReturnsAsync(false);

            await _mainViewModel.PerformUpdateAsync();
        }

        [TestMethod]
        [DataRow(MessageDialogResult.Canceled)]
        [DataRow(MessageDialogResult.FirstAuxiliary)]
        [DataRow(MessageDialogResult.Negative)]
        [DataRow(MessageDialogResult.SecondAuxiliary)]
        public async Task PerformUpdateAsync_WhenCalledWithUpdateAvailableButUserCancels_ThenDoesNothing(
            MessageDialogResult messageDialogResult)
        {
            _autoUpdateServiceMock
                .Setup(m => m.CanPerformUpdateAsync())
                .ReturnsAsync(true);
            SetupUpdateAvailableDialog(messageDialogResult);

            await _mainViewModel.PerformUpdateAsync();
        }

        [TestMethod]
        public async Task PerformUpdateAsync_WhenCalledWithUpdateAvailableAndUserConfirms_ThenPerformsUpdate()
        {
            _autoUpdateServiceMock
                .Setup(m => m.CanPerformUpdateAsync())
                .ReturnsAsync(true);

            SetupUpdateAvailableDialog(MessageDialogResult.Affirmative);

            var progressDialogController = default(ProgressDialogController);

            _dialogCoordinatorMock
                .Setup(d => d.ShowProgressAsync(
                    _mainViewModel,
                    MainResources.Updating_Title,
                    MainResources.Updating_Message,
                    false,
                    It.Is<MetroDialogSettings>(
                        mds => mds.AnimateShow == false && mds.AnimateHide == false)))
                .ReturnsAsync(progressDialogController);

            _autoUpdateServiceMock
                .Setup(m => m.PerformUpdateAsync(progressDialogController))
                .Returns(Task.CompletedTask);

            await _mainViewModel.PerformUpdateAsync();
        }

        [TestMethod]
        public async Task PerformUpdateAsync_WhenCalledButExceptionsOccurs_ThenDialogIsShown()
        {
            var expectedMessage = "Some error";

            _autoUpdateServiceMock
                .Setup(m => m.CanPerformUpdateAsync())
                .Throws(new Exception(expectedMessage));

            SetupErrorDialogMock(expectedMessage);

            await _mainViewModel.PerformUpdateAsync();
        }

        [TestMethod]
        public void DeleteDownloadItem_WhenCalled_ThenRemovesDownloadItem()
        {
            var downloadItem = new DownloadItem(ValidVideoId);

            _mainViewModel.AddDownloadItem(downloadItem);
            _mainViewModel.DownloadItems.ShouldHaveSingleItem();

            _mainViewModel.DeleteDownloadItem(downloadItem);
            _mainViewModel.DownloadItems.ShouldBeEmpty();
        }

        private void SetupDownloadItemMocks(DownloadItem downloadItem, Video videoInfo)
        {
            _youTubeDownloadServiceMock
                .Setup(m => m.GetVideo(downloadItem.YouTubeId))
                .ReturnsAsync(videoInfo);

            var mp4Path = @"C:\Temp\test.mp4";
            _youTubeDownloadServiceMock
                .Setup(m => m.DownloadMp4AndGetPath(videoInfo, $"{downloadItem.Artist} - {downloadItem.Title}"))
                .ReturnsAsync(mp4Path);

            var mp3Path = @"C:\Temp\test.mp3";
            _conversionServiceMock
                .Setup(m => m.ConvertMp4ToMp3AndGetMp3Path(mp4Path))
                .ReturnsAsync(mp3Path);

            _audioNormalizerServiceMock
                .Setup(m => m.NormalizeAudio(mp3Path));

            _mp3TagServiceMock
                .Setup(m => m.SetMp3Tags(mp3Path, downloadItem.Artist, downloadItem.Title));
        }

        private void SetupUpdateAvailableDialog(MessageDialogResult messageDialogResult)
        {
            _dialogCoordinatorMock
                .Setup(d => d.ShowMessageAsync(
                    _mainViewModel,
                    MainResources.UpdateAvailable_Title,
                    MainResources.UpdateAvailable_Message,
                    MessageDialogStyle.AffirmativeAndNegative,
                    It.Is<MetroDialogSettings>(
                        mds => mds.DefaultButtonFocus == MessageDialogResult.Affirmative &&
                        mds.AnimateHide == false)
                    ))
                .ReturnsAsync(messageDialogResult);
        }

        private void SetupErrorDialogMock(string expectedMessage)
        {
            _dialogCoordinatorMock
                 .Setup(d => d.ShowMessageAsync(
                     _mainViewModel,
                     CommonResources.ErrorOccurredTitle,
                     string.Format(CommonResources.ErrorOccurredMessage, expectedMessage),
                     MessageDialogStyle.Affirmative,
                     null))
                 .ReturnsAsync(MessageDialogResult.Affirmative);
        }

        private static void ValidateFailedDownload(DownloadItem downloadItem)
        {
            downloadItem.IsRunning.ShouldBeFalse();
            downloadItem.Status.ShouldBe(MainResources.Failed);
            downloadItem.IsFailed.ShouldBeTrue();
            downloadItem.Completion.ShouldBe(100);
        }

        private static Video CreateVideo(string title = "Artist - Title")
            => new(default, title, default, default, "Description", default, default, default, default);
    }
}