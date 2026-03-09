using System.IO;
using Newtonsoft.Json;

namespace VeryVeryValetAPClient
{
    public class ConnectionInfo
    {
        public ConnectionInfo(string server, string slot, string password)
        {
            Server = server;
            Slot = slot;
            Password = password;
        }

        public string Server { get; set; }
        public string Slot { get; set; }
        public string Password { get; set; }
    }

    public static class ConnectionInfoHandler
    {
        private const string Path = "./ArchipelagoSaves/" + "connection_info.json";
        public static string SavedServer = "Archipelago.gg:";
        public static string SavedSlot = "Player1";
        public static string SavedPassword = "";

        public static void Save(string server, string slot, string password)
        {
            SavedServer = server;
            SavedSlot = slot;
            SavedPassword = password;
            if (!Directory.Exists("./ArchipelagoSaves/"))
                Directory.CreateDirectory("./ArchipelagoSaves/");
            var connectionInfo = new ConnectionInfo(server, slot, password);
            var text = JsonConvert.SerializeObject(connectionInfo);
            File.WriteAllText(Path, text);
        }

        public static bool Load()
        {
            if (!File.Exists(Path))
                Save(SavedServer, SavedSlot, SavedPassword);
            var json = File.ReadAllText(Path);
            var connectionInfo = JsonConvert.DeserializeObject<ConnectionInfo>(json);
            if (connectionInfo == null) 
                return false;
            SavedServer = connectionInfo.Server;
            SavedSlot = connectionInfo.Slot;
            SavedPassword = connectionInfo.Password;
            return true;
        }
    }
}
