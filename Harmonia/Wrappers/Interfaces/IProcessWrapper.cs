using System.Diagnostics;

namespace Harmonia.Wrappers.Interfaces
{
    public interface IProcessWrapper
    {
        void Start(string fileName);

        void StartWaitForExitWithTimeoutKill(ProcessStartInfo processStartInfo);
    }
}