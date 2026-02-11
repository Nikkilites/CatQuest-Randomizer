using BepInEx;
using HarmonyLib;
using System.IO;
using UnityEngine;

namespace CatQuest_Randomizer.Patches
{
    public class SpriteChangePatches
    {
        [HarmonyPatch(typeof(QuestListItem2), nameof(QuestListItem2.SetData))]
        public class QuestBoardSpriteOverridePatch
        {
            static void Postfix(Quest quest, UnityEngine.UI.Image ___notice)
            {
                Randomizer.Logger.LogInfo($"Override {quest.questId} notice sprite");

                if (quest.isComplete)
                {
                    ___notice.sprite = Randomizer.DataStorageHandler.Sprites["questComplete"];
                }
                else if (Randomizer.LocationHandler.GetLocationLogicAvailability(quest))
                {
                    ___notice.sprite = Randomizer.DataStorageHandler.Sprites["questInLogic"];
                }
                else
                {
                    ___notice.sprite = Randomizer.DataStorageHandler.Sprites["questOutOfLogic"];
                }
            }
        }
    }
}
