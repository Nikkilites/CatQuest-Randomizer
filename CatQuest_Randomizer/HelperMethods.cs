using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.IO;

namespace CatQuest_Randomizer
{
    public static class HelperMethods
    {
        public static object LoadJson<T>(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new Exception($"File {filepath} not found.");
            }

            Randomizer.Logger.LogInfo($"Will load data from {filepath}");
            string json = File.ReadAllText(filepath);
            T data = JsonConvert.DeserializeObject<T>(json);
            Randomizer.Logger.LogInfo($"Data {data} loaded from {filepath}");
            return data;
        }

        public static void SaveCatQuestGame()
        {
            var saveManager = Game.instance.saveManager;
            var currentSaveObject = AccessTools.Field(typeof(SaveManager), "currentSaveObject").GetValue(saveManager);
            var updatePlayerProgressMethod = AccessTools.Method(typeof(SaveManager), "UpdatePlayerProgress");
            var updateSkillProgressMethod = AccessTools.Method(typeof(SaveManager), "UpdateSkillProgress");
            var updateEquipmentProgressMethod = AccessTools.Method(typeof(SaveManager), "UpdateEquipmentProgress");
            var updateMetaProgressMethod = AccessTools.Method(typeof(SaveManager), "UpdateMetaProgress");
            var flushMethod = AccessTools.Method(typeof(SaveManager), "Flush");

            if (currentSaveObject != null
                & updatePlayerProgressMethod != null
                & updateSkillProgressMethod != null
                & updateEquipmentProgressMethod != null
                & updateMetaProgressMethod != null
                & flushMethod != null)
            {
                updatePlayerProgressMethod.Invoke(saveManager, new[] { currentSaveObject });
                updateSkillProgressMethod.Invoke(saveManager, new[] { currentSaveObject });
                updateEquipmentProgressMethod.Invoke(saveManager, new[] { currentSaveObject });
                updateMetaProgressMethod.Invoke(saveManager, new[] { currentSaveObject });
                flushMethod.Invoke(saveManager, null);
                Randomizer.Logger.LogInfo($"Saved the game");
            }
        }
    }
}
