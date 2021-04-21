using System.Windows.Shell;

namespace Harmonia.ViewModels
{
    public class DownloadProgressEventArgs
    {
        public TaskbarItemProgressState State { get; set; }

        public double TotalItems { get; set; }

        public int FinishedItems { get; set; }

        public double Value => FinishedItems / TotalItems;

        public bool IsFinished => TotalItems == FinishedItems;
    }
}