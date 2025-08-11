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
    }
}
