using System;
using System.Threading.Tasks;
using Harmonia.Services.Interfaces;
using MahApps.Metro.Controls.Dialogs;
using Onova;

namespace Harmonia.Services
{
    public class AutoUpdateService : IAutoUpdateService
    {
        private readonly IUpdateManager _updateManager;

        public AutoUpdateService(IUpdateManager updateManager)
        {
            _updateManager = updateManager;
        }

        public async Task<bool> CanPerformUpdateAsync()
            => (await _updateManager.CheckForUpdatesAsync()).CanUpdate;

        public async Task PerformUpdateAsync(ProgressDialogController progressDialog)
        {
            var result = await _updateManager.CheckForUpdatesAsync();
            if (!result.CanUpdate)
            {
                return;
            }

            var progress = new Progress<double>();
            progress.ProgressChanged += (s, e) =>
            {
                progressDialog.SetProgress(e);
            };

            await _updateManager.PrepareUpdateAsync(result.LastVersion, progress);

            _updateManager.LaunchUpdater(result.LastVersion);

            Environment.Exit(0);
        }
    }
}