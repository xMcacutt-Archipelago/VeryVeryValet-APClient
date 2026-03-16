using System.IO;
using Newtonsoft.Json;

namespace VeryVeryValetAPClient
{
    public class CustomSaveData
    {
        public int ItemIndex;
        public int StoredPowerups;
        public int StarCount;
    }
    
    public class CustomSaveDataHandler
    {
        public static CustomSaveData Data = new CustomSaveData();
        
        public static void Save()
        {
            if (!PluginMain.ArchipelagoHandler?.IsConnected ?? true)
                return;
            var saveFolder = PlayerSave.GetSaveFolder();
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);
            var path = Path.Combine(saveFolder, $"AP_{PluginMain.ArchipelagoHandler.Slot}_{PluginMain.ArchipelagoHandler.seed}.json");
            var text = JsonConvert.SerializeObject(Data);
            File.WriteAllText(path, text);
        }

        public static void Load()
        {
            if (!PluginMain.ArchipelagoHandler?.IsConnected ?? true)
                return;
            var saveFolder = PlayerSave.GetSaveFolder();
            var path = Path.Combine(saveFolder, $"AP_{PluginMain.ArchipelagoHandler.Slot}_{PluginMain.ArchipelagoHandler.seed}.json");
            if (!File.Exists(path))
                Save();
            var json = File.ReadAllText(path);
            var tempData = JsonConvert.DeserializeObject<CustomSaveData>(json);
            if (tempData == null) 
                return;
            Data.StarCount = tempData.StarCount;
            Data.ItemIndex = tempData.ItemIndex;
            Data.StoredPowerups = tempData.StoredPowerups;
            return;
        }
    }
}