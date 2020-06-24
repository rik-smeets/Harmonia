using System;
using System.Diagnostics;
using Harmonia.Properties;
using Harmonia.Wrappers.Interfaces;

namespace Harmonia.Wrappers
{
    public class ProcessWrapper : IProcessWrapper
    {
        public void Start(string fileName)
            => Process.Start(fileName);

        public void StartWaitForExitWithTimeoutKill(ProcessStartInfo processStartInfo)
        {
            using (var process = new Process
            {
                StartInfo = processStartInfo
            })
            {
                process.Start();
                var success = process.WaitForExit((int)TimeSpan.FromSeconds(60).TotalMilliseconds);
                if (!success)
                {
                    process.Kill();
                    throw new TimeoutException(ServicesResources.MP3Gain_Timeout);
                }
            }
        }
    }
}