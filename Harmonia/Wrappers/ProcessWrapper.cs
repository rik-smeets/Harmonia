using System.Diagnostics;
using Harmonia.Properties;
using Harmonia.Wrappers.Interfaces;

namespace Harmonia.Wrappers
{
    public class ProcessWrapper : IProcessWrapper
    {
        public void Start(string fileName)
            => Process.Start(fileName);

        public void StartWaitForExitWithTimeoutKill(ProcessStartInfo processStartInfo, int timeoutSeconds = 60)
        {
            using (var process = new Process
            {
                StartInfo = processStartInfo
            })
            {
                process.Start();
                var success = process.WaitForExit((int)TimeSpan.FromSeconds(timeoutSeconds).TotalMilliseconds);
                if (!success)
                {
                    process.Kill();
                    throw new TimeoutException(ServicesResources.MP3Gain_Timeout);
                }
            }
        }
    }
}