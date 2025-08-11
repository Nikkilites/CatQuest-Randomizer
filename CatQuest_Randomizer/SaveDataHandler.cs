using Newtonsoft.Json;
using System;
using System.IO;

namespace CatQuest_Randomizer
{
    public static class SaveDataHandler
    {
        private static readonly string itemIndexPath = $"{Environment.CurrentDirectory}\\ArchipelagoRandomizer\\SaveData\\ItemIndex.json";
        private static readonly string roomInfoPath = $"{Environment.CurrentDirectory}\\ArchipelagoRandomizer\\SaveData\\RoomInfo.json";
        public static int ItemIndex
        {
            get
            {
                ItemIndexData data = (ItemIndexData)HelperMethods.LoadJson<ItemIndexData>(itemIndexPath);
                return data.ItemIndex;
            }
            set
            {
                SaveItemIndex(value);
            }
        }
        public static RoomInfoData RoomInfo
        {
            get
            {
                return (RoomInfoData)HelperMethods.LoadJson<RoomInfoData>(roomInfoPath);
            }
        }

        private static void SaveItemIndex(int itemIndex)
        {
            string directoryPath = Path.GetDirectoryName(itemIndexPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                Randomizer.Logger.LogInfo($"Created directory: {directoryPath}");
            }

            Randomizer.Logger.LogInfo($"Will save {itemIndex} to {itemIndexPath}");
            var json = JsonConvert.SerializeObject(new { ItemIndex = itemIndex }, Formatting.Indented);
            File.WriteAllText(itemIndexPath, json);
            Randomizer.Logger.LogInfo($"ItemIndex {itemIndex} saved to {itemIndexPath}");
        }

        public static void SaveRoomInfo(string server, string player, string password)
        {
            string directoryPath = Path.GetDirectoryName(roomInfoPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                Randomizer.Logger.LogInfo($"Created directory: {directoryPath}");
            }

            Randomizer.Logger.LogInfo($"Will save Room Info to {roomInfoPath}");
            RoomInfoData roomInfoData = new(server, player, password);
            string json = JsonConvert.SerializeObject(roomInfoData, Formatting.Indented);
            File.WriteAllText(roomInfoPath, json);
            Randomizer.Logger.LogInfo($"Room Info saved to {roomInfoPath}");
        }

        private class ItemIndexData
        {
            public int ItemIndex { get; set; }
        }

        public class RoomInfoData
        {
            public string Server { get; set; }
            public string Playername { get; set; }
            public string Password { get; set; }

            public RoomInfoData(string server, string playername, string password)
            {
                Server = server;
                Playername = playername;
                if (password == "")
                    Password = null;
                else
                    Password = password;
            }
        }
    }
}
