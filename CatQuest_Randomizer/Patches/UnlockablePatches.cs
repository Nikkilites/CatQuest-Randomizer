using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace CatQuest_Randomizer.Patches
{
    [HarmonyPatch(typeof(UnlockBlock), nameof(UnlockBlock.Start))]
    public class DisableObtainingArtsPatch
    {
        private static ManualLogSource Logger;
        private static FieldInfo questIdField = typeof(UnlockBlock).GetField("questId", BindingFlags.NonPublic | BindingFlags.Instance);

        static bool Prefix(UnlockBlock __instance)
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource("DisableObtainingArtsPatch");

            switch (__instance.value)
            {
                case Unlockables.Flying:
                    Logger.LogInfo($"Obtaining Flying was disabled");
                    break;
                case Unlockables.WaterWalking:
                    Logger.LogInfo($"Obtaining Water Walking was disabled");
                    break;
                default:
                    Game.instance.Unlock(__instance.value);
                    break;
            }

            string questId = questIdField.GetValue(__instance) as string;

            Game.gameStream.Publish(new QuestEvent
            {
                type = QuestEvent.EventType.QUEST_BLOCK_COMPLETE,
                questId = questId
            });

            return false;
        }
    }
}
