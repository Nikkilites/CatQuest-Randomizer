using CatQuest.UI;
using HarmonyLib;

namespace CatQuest_Randomizer.Patches
{
    [HarmonyPatch(typeof(WorldMapHUD))]
    [HarmonyPatch("EnterTrigger", MethodType.Normal)]
    public class TempleCheckPatch
    {
        static void Postfix(WorldMapTriggerEvent e)
        {
            ArcaneTempleTrigger arcaneTempleTrigger = e.trigger as ArcaneTempleTrigger;
            if (arcaneTempleTrigger != null)
            {
                if (arcaneTempleTrigger.skillId != "")
                {
                    Randomizer.Logger.LogInfo($"Temple of type {arcaneTempleTrigger.skillId} was found, send check from TempleCheckPatch");
                    Randomizer.LocationHandler.CheckLocation(arcaneTempleTrigger.skillId);
                }
            }
        }
    }
}
