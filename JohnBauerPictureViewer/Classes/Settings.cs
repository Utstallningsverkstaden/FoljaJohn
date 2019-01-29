using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.ViewManagement;

namespace Spree.Classes
{
    /// <summary>
    /// 
    /// Created by Lars Rosén, Spree AB, 2016
    /// 
    /// 1. In the file Package.appxmanifest, you must specify the documentsLibrary-capability.
    ///     Below you find an example:
    ///     
    /// <Capabilities>
    ///     <Capability Name = "internetClient" />
    ///     <uap:Capability Name = "documentsLibrary" />
    /// </ Capabilities >
    /// 
    /// 2. You must also declare a file extension in the Package.appxmanifest file.
    ///     This is done under the Application tag.
    ///     You can also do this in the Manifest Designer (just double click on the file or "open" it)
    ///     This is what should be in the extension:
    /// 
    ///     <Extensions>
    ///         <uap:Extension Category = "windows.fileTypeAssociation" >
    ///             <uap:FileTypeAssociation Name = "json" >
    ///                 <uap:DisplayName>Demo App</uap:DisplayName>
    ///                 <uap:SupportedFileTypes>
    ///                     <uap:FileType ContentType = "application/json" >.json </ uap:FileType>
    ///                 </uap:SupportedFileTypes>
    ///             </uap:FileTypeAssociation>
    ///         </uap:Extension>
    ///     </Extensions>
    /// 
    /// 3. For a more convenient way of using the class, add a property to the App-class like this:
    ///     
    ///     private static Settings _Settings;
    ///     internal static Settings Settings { get { if (_Settings == null) { _Settings = new Settings(); } return _Settings; } }
    ///     Then you can access the Settings-class instance only by calling App.Settings.xxxxx
    ///     
    /// 4. (Optional) Use the FullScreen method if you want your app to start in full screen
    ///     App.Settings.FullScreen();
    /// 
    /// 5. Example usage:
    ///     You put your settings in a JSon-file and saves it in the Documents-directory.
    ///     The file should contain a list called "Settings" and each setting should have two values, "SettingsName" and "Data".
    ///     
    ///     The file could contain something like this:
    ///     
    ///     {
    ///         "Settings": [
    ///     		{ "SettingName": "ServerURL", "Data": "http://www.google.se" },
    ///     		{ "SettingName": "MaxNumberOfCards", "Data": "10" }
    ///     	]
    ///     }
    /// 
    ///     Add a function to your main window that is called CheckSettingsFile that is async.
    ///     Call it from your main windows contructor.
    ///     In the function you setup the settings file and checks a locally stored setting.
    ///     If the locally stored setting is not set you show the user s settings page where the user can select a Staion ID.
    ///     If you can have all your settings in the settings file you don't need the locally stored setting and if you
    ///     only need locaally stored settings you can check only for that.
    ///     Locally stored settings are sttings stoed in local storage for the app and not exposed to the user.
    ///     
    ///     private async void CheckSettingsFile()
    ///     {
    ///         await App.Settings.SetupSettingsFile("AppSettings.json");
    ///         
    ///         string StationID = App.Settings.GetLocalValue("StationID");
    ///         if ((StationID == null) || (StationID == ""))
    ///         {
    ///             ShowSettingsPage();
    ///         }
    ///     }
    ///     
    /// 
    ///     This is what you need to do to save a locally stored setting:
    ///     
    ///     App.Settings.SetLocalValue("StationID", "1");
    /// 
    /// 
    /// </summary>


    #region Sub-classes
    public class SettingsData
    {
        public System.Collections.Generic.List<SettingsRow> Settings { get; set; }
    }

    public class SettingsRow
    {
        public String SettingName { get; set; }
        public String Data { get; set; }

    } 
    #endregion

    public class Settings
    {
        #region Private fields
        private ApplicationDataContainer localSettings;
        private SettingsData _SettingsData;

        #endregion

        #region Contructor
        public Settings()
        {
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        }
        #endregion

        #region Public methods
        public async Task SetupSettingsFile(String FileName)
        {
            string text = await GetSettingsFileData(FileName);

            if (text == null)
            {
                var DisplayName = Path.GetFileNameWithoutExtension(FileName);

                text = await FindSettingsFileData(DisplayName);
            }

            _SettingsData = await Newtonsoft.Json.JsonConvert.DeserializeObjectAsync<SettingsData>(text);
        }

        public string GetValue(string SettingName)
        {
            if (_SettingsData != null)
            {
                foreach (var Setting in _SettingsData.Settings)
                {
                    if (Setting.SettingName.ToLower() == SettingName.ToLower())
                    {
                        return Setting.Data;
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        public bool HasValue(string SettingName)
        {
            return GetValue(SettingName) != null;
        }

        public string GetLocalValue(string ValueName)
        {
            return localSettings.Values[ValueName] as string;
        }

        public void SetLocalValue(string ValueName, string Value)
        {
            localSettings.Values[ValueName] = Value;
        }

        public void FullScreen()
        {
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
        }
        #endregion

        #region Private methods
        private async Task<bool> SettingsFilesExists(string DisplayName)
        {
            QueryOptions queryOption = new QueryOptions(CommonFileQuery.OrderByTitle, new string[] { ".json" });

            queryOption.FolderDepth = FolderDepth.Deep;

            System.Collections.Generic.Queue<IStorageFolder> folders = new System.Collections.Generic.Queue<IStorageFolder>();

            var files = await KnownFolders.DocumentsLibrary.CreateFileQueryWithOptions(queryOption).GetFilesAsync();

            foreach (var file in files)
            {
                if (file.DisplayName == DisplayName)
                {
                    return true;
                }
            }
            return false;
        }



        private async Task<string> GetSettingsFileData(string FileName)
        {
            StorageFile storeageFile = await GetSettingsFile(FileName);
            if (storeageFile != null)
            {
                string text = await Windows.Storage.FileIO.ReadTextAsync(storeageFile);
                return text;
            }
            else
            {
                return null;
            }
        }

        private async Task<StorageFile> GetSettingsFile(string FileName)
        {
            StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
            try
            {
                StorageFile storeageFile = await storageFolder.GetFileAsync(FileName);
                return storeageFile;
            }
            catch
            {
                return null;
            }
        }

        private async Task<String> FindSettingsFileData(string DisplayName)
        {
            StorageFile storeageFile = await FindSettingsFile(DisplayName);
            if (storeageFile != null)
            {
                string text = await Windows.Storage.FileIO.ReadTextAsync(storeageFile);
                return text;
            }
            else
            {
                return null;
            }
        }

        private async Task<StorageFile> FindSettingsFile(string DisplayName)
        {
            QueryOptions queryOption = new QueryOptions(CommonFileQuery.OrderByTitle, new string[] { ".json" });

            queryOption.FolderDepth = FolderDepth.Deep;

            System.Collections.Generic.Queue<IStorageFolder> folders = new System.Collections.Generic.Queue<IStorageFolder>();

            var files = await KnownFolders.DocumentsLibrary.CreateFileQueryWithOptions(queryOption).GetFilesAsync();

            foreach (var file in files)
            {
                if (file.DisplayName == DisplayName)
                {
                    return file;
                }
            }
            return null;
        }

        
        #endregion

    }
}
