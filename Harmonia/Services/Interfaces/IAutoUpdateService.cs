using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace Harmonia.Services.Interfaces
{
    public interface IAutoUpdateService
    {
        Task<bool> CanPerformUpdateAsync();

        Task PerformUpdateAsync(ProgressDialogController progressDialog);
    }
}