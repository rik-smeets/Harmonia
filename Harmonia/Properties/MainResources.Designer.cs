﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Harmonia.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class MainResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MainResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Harmonia.Properties.MainResources", typeof(MainResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Artist.
        /// </summary>
        public static string Artist {
            get {
                return ResourceManager.GetString("Artist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Delete (Ctrl + Del).
        /// </summary>
        public static string btnDelete {
            get {
                return ResourceManager.GetString("btnDelete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to About _Harmonia.
        /// </summary>
        public static string btnOpenAboutWindow {
            get {
                return ResourceManager.GetString("btnOpenAboutWindow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Open output _directory.
        /// </summary>
        public static string btnOpenOutputDirectory {
            get {
                return ResourceManager.GetString("btnOpenOutputDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to _Settings.
        /// </summary>
        public static string btnSettings {
            get {
                return ResourceManager.GetString("btnSettings", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Start _all.
        /// </summary>
        public static string btnStartAll {
            get {
                return ResourceManager.GetString("btnStartAll", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Completed.
        /// </summary>
        public static string Completed {
            get {
                return ResourceManager.GetString("Completed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Delete.
        /// </summary>
        public static string Delete {
            get {
                return ResourceManager.GetString("Delete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error occurred during download.
        /// </summary>
        public static string DownloadCompleteToast_Error {
            get {
                return ResourceManager.GetString("DownloadCompleteToast_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Downloads finished.
        /// </summary>
        public static string DownloadCompleteToast_Success {
            get {
                return ResourceManager.GetString("DownloadCompleteToast_Success", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed.
        /// </summary>
        public static string Failed {
            get {
                return ResourceManager.GetString("Failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Harmonia is monitoring your clipboard. Copy a valid YouTube URL (Ctrl + C) and it will show up in the grid below. Artist and title will be suggested. You can change these if you like. Start downloading and converting by selecting Start all. Completed and running downloads will be ignored, so feel free to start downloading when still adding new YouTube URLs..
        /// </summary>
        public static string Harmonia_Description {
            get {
                return ResourceManager.GetString("Harmonia_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The MP3Gain executable, used for normalizing audio, cannot be found. Would you like to open Settings to fix this?.
        /// </summary>
        public static string MP3GainExecutableNotFound_Message {
            get {
                return ResourceManager.GetString("MP3GainExecutableNotFound_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Audio normalization will be skipped..
        /// </summary>
        public static string Mp3GainExecutableNotFound_SkippingAudioNormalization {
            get {
                return ResourceManager.GetString("Mp3GainExecutableNotFound_SkippingAudioNormalization", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MP3Gain executable cannot be found.
        /// </summary>
        public static string MP3GainExecutableNotFound_Title {
            get {
                return ResourceManager.GetString("MP3GainExecutableNotFound_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New.
        /// </summary>
        public static string New {
            get {
                return ResourceManager.GetString("New", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The output directory: &apos;{0}&apos; does not exist. Select an existing path in Settings..
        /// </summary>
        public static string OutputPathDoesNotExist_Message {
            get {
                return ResourceManager.GetString("OutputPathDoesNotExist_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Output path does not exist.
        /// </summary>
        public static string OutputPathDoesNotExist_Title {
            get {
                return ResourceManager.GetString("OutputPathDoesNotExist_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Progress.
        /// </summary>
        public static string Progress {
            get {
                return ResourceManager.GetString("Progress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Running.
        /// </summary>
        public static string Running {
            get {
                return ResourceManager.GetString("Running", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Status.
        /// </summary>
        public static string Status {
            get {
                return ResourceManager.GetString("Status", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Title.
        /// </summary>
        public static string Title {
            get {
                return ResourceManager.GetString("Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurred when Harmonia was updating.
        /// </summary>
        public static string Update_Error {
            get {
                return ResourceManager.GetString("Update_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An update for Harmonia is available. Upgrading is strongly recommended, since older versions might not work properly anymore. It won&apos;t take long. Would you like to download and install the update?.
        /// </summary>
        public static string UpdateAvailable_Message {
            get {
                return ResourceManager.GetString("UpdateAvailable_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Update available.
        /// </summary>
        public static string UpdateAvailable_Title {
            get {
                return ResourceManager.GetString("UpdateAvailable_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Harmonia is currently updating. It will close and relaunch automatically. .
        /// </summary>
        public static string Updating_Message {
            get {
                return ResourceManager.GetString("Updating_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Updating....
        /// </summary>
        public static string Updating_Title {
            get {
                return ResourceManager.GetString("Updating_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error occurred whilst getting video metadata.
        /// </summary>
        public static string VideoMetaDataToast_Error {
            get {
                return ResourceManager.GetString("VideoMetaDataToast_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Harmonia.
        /// </summary>
        public static string Window_Title {
            get {
                return ResourceManager.GetString("Window_Title", resourceCulture);
            }
        }
    }
}
