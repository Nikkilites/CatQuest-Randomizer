using Newtonsoft.Json;
using System;
using System.IO;

namespace CatQuest_Randomizer
{
    public class SaveDataHandler
    {
        private readonly string itemIndexPath = $"{Environment.CurrentDirectory}\\ArchipelagoRandomizer\\SaveData\\ItemIndex.json";
        public int ItemIndex
        {
            get
            {
                return LoadItemIndex();
            }
            set
            {
                SaveItemIndex(value);
            }
        }

        private void SaveItemIndex(int itemIndex)
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

        private int LoadItemIndex()
        {
            if (!File.Exists(itemIndexPath))
            {
                Randomizer.Logger.LogInfo($"File {itemIndexPath} not found. Returning default value 0.");
                return 0;
            }

            Randomizer.Logger.LogInfo($"Will load ItemIndex from {itemIndexPath}");
            var json = File.ReadAllText(itemIndexPath);
            var data = JsonConvert.DeserializeObject<ItemIndexData>(json);
            Randomizer.Logger.LogInfo($"ItemIndex {data.ItemIndex} loaded from {itemIndexPath}");
            return data.ItemIndex;
        }

        private class ItemIndexData
        {
            public int ItemIndex { get; set; }
        }
    }
}
