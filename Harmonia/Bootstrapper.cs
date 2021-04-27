using System.Diagnostics;
using System.Reflection;
using Harmonia.Services;
using Harmonia.Services.Interfaces;
using Harmonia.Settings;
using Harmonia.Settings.Interfaces;
using Harmonia.Wrappers;
using Harmonia.Wrappers.Interfaces;
using MahApps.Metro.Controls.Dialogs;
using Onova;
using Onova.Models;
using Onova.Services;
using Unity;

namespace Harmonia
{
    public static class Bootstrapper
    {
        public static IUnityContainer Register(IUnityContainer container)
        {
            RegisterServices(container);
            RegisterSettings(container);
            RegisterWrappers(container);
            RegisterExternalDependencies(container);

            // Register instance of self so a Window can resolve another Window to show
            container.RegisterInstance(container);

            return container;
        }

        private static void RegisterServices(IUnityContainer container)
            => container
            .RegisterType<IYouTubeDownloadService, YouTubeDownloadService>()
            .RegisterType<IConversionService, ConversionService>()
            .RegisterType<IMp3TagService, Mp3TagService>()
            .RegisterType<IAudioNormalizerService, AudioNormalizerService>()
            .RegisterType<IAutoUpdateService, AutoUpdateService>();

        private static void RegisterSettings(IUnityContainer container)
            => container
            .RegisterType<ISettingsProvider, SettingsProvider>()
            .RegisterType<ISettingsRetriever, SettingsRetriever>();

        private static void RegisterWrappers(IUnityContainer container)
            => container
            .RegisterType<IStorageWrapper, StorageWrapper>()
            .RegisterType<IDialogCoordinator, DialogCoordinator>()
            .RegisterType<IProcessWrapper, ProcessWrapper>()
            .RegisterType<IClipboardWrapper, ClipboardWrapper>()
            .RegisterType<IYouTubeDownloadService, YouTubeDownloadService>()
            .RegisterType<IThemeManagerWrapper, ThemeManagerWrapper>();

        private static void RegisterExternalDependencies(IUnityContainer container)
            => container
            .RegisterType<IDialogCoordinator, DialogCoordinator>()
            .RegisterInstance<IUpdateManager>(CreateUpdateManager());

        private static UpdateManager CreateUpdateManager()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var executablePath = Process.GetCurrentProcess().MainModule.FileName;
            var assemblyMetaData = AssemblyMetadata.FromAssembly(entryAssembly, executablePath);

            return new UpdateManager(
                assemblyMetaData,
                new GithubPackageResolver("rik-smeets", "Harmonia", "*.zip"),
                new ZipPackageExtractor());
        }
    }
}