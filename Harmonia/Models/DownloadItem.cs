using System.ComponentModel;
using Harmonia.Properties;
using PropertyChanged;

namespace Harmonia.Models
{
    public class DownloadItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DownloadItem(string youTubeId)
        {
            YouTubeId = youTubeId;
        }

        public string Status { get; set; } = MainResources.New;
        public string Artist { get; set; }
        public string Title { get; set; }
        public int Completion { get; set; }

        public bool IsCompleted { get; set; }
        public bool IsFailed { get; set; }
        public bool IsRunning { get; set; }

        [DependsOn(nameof(IsCompleted), nameof(IsRunning))]
        public bool IsRunningOrCompleted => IsRunning || IsCompleted;

        public string YouTubeId { get; }

        public void SetFailed()
        {
            IsRunning = false;
            Status = MainResources.Failed;
            IsFailed = true;
            Completion = 100;
        }
    }
}