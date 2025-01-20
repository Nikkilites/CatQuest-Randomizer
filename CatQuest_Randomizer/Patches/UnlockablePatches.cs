using HarmonyLib;
using System.Reflection;

namespace CatQuest_Randomizer.Patches
{
    [HarmonyPatch(typeof(UnlockBlock), nameof(UnlockBlock.Start))]
    public class DisableObtainingArtsPatch
    {
        private static FieldInfo questIdField = typeof(UnlockBlock).GetField("questId", BindingFlags.NonPublic | BindingFlags.Instance);

        static bool Prefix(UnlockBlock __instance)
        {
            Randomizer.Logger.LogInfo("Check if unlockable should be disabled.");

            switch (__instance.value)
            {
                case Unlockables.Flying:
                    Randomizer.Logger.LogInfo($"Obtaining Flying was disabled");
                    break;
                case Unlockables.WaterWalking:
                    Randomizer.Logger.LogInfo($"Obtaining Water Walking was disabled");
                    break;
                case Unlockables.GoldChest:
                    Randomizer.Logger.LogInfo($"Obtaining Golden Key was disabled");
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
