using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using static CatQuest_Randomizer.SaveDataHandler;
using UnityEngine;

namespace CatQuest_Randomizer.Patches
{
    [HarmonyPatch(typeof(Title), nameof(Title.StartNormalGame))]
    public class NewGamePatch
    {
        static void Prefix()
        {
            Randomizer.Logger.LogInfo("New game was started");
            ItemIndex = 0;
        }
    }

    [HarmonyPatch(typeof(Title))]
    [HarmonyPatch("Menu_OnNormalContinueSelect", MethodType.Normal)]
    public class ContinueGamePatch
    {
        static void Prefix()
        {
            Randomizer.Logger.LogInfo("Game was continued");
        }
    }

    [HarmonyPatch(typeof(Title), nameof(Title.StartGame))]
    public class GameOpenPatch
    {
        static void Postfix()
        {
            RoomInfoData room = SaveDataHandler.RoomInfo;
            Randomizer.ConnectionHandler.Connect(room.Server, room.Playername, room.Password);
        }
    }

    [HarmonyPatch(typeof(NewGamePanel), "Start")]
    public class EditMenuOptionPatch
    {
        private static NewGamePanelOption menuItem;

        static void Prefix(NewGamePanel __instance)
        {
            Randomizer.Logger.LogInfo("Will edit menu item for archipelago.");

            // Access private field listItems
            FieldInfo listField = typeof(NewGamePanel).GetField("listItems", BindingFlags.NonPublic | BindingFlags.Instance);
            if (listField == null)
            {

                Randomizer.Logger.LogError("Could not find the 'listItems' field in NewGamePanel.");
                return;
            }

            var listItems = listField.GetValue(__instance) as List<NewGamePanelOption>;
            if (listItems == null || listItems.Count == 0)
            {
                Randomizer.Logger.LogError("listItems is null or empty.");
                return;
            }

            // Clone the last item
            menuItem = listItems[listItems.Count - 1];

            menuItem.SetName("New Archipelago Game");

            // Access and modify private 'optionDescription' field
            FieldInfo descField = typeof(NewGamePanelOption).GetField("optionDescription", BindingFlags.NonPublic | BindingFlags.Instance);
            if (descField != null)
            {
                var textComponent = descField.GetValue(menuItem) as TextMeshProUGUI;
                if (textComponent != null)
                {
                    textComponent.text = "Connect to server and start game";
                }
            }
            else
            {
                Randomizer.Logger.LogError("Could not find 'optionDescription' field in NewGamePanelOption.");
            }
        }
    }

    [HarmonyPatch(typeof(Title), "Start")]
    public class ArchipelagoLogoPatch
    {
        static void Postfix(Title __instance)
        {

            FieldInfo imgField = typeof(Title).GetField("title", BindingFlags.NonPublic | BindingFlags.Instance);
            if (imgField != null)
            {
                var titleImg = imgField.GetValue(__instance) as UnityEngine.UI.Image;
                if (titleImg != null && Randomizer.DataStorageHandler.apTitleSprite != null)
                {
                    titleImg.sprite = Randomizer.DataStorageHandler.apTitleSprite;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Title), "ShowConfirmationPanel")]
    public class ConfirmationPanelPatch
    {
        static void Prefix(Title __instance, ref string title, ref string confirmation, ref System.Action onYes, ref System.Action onNo)
        {
            if (title != "New Game")
                return;

            Randomizer.Logger.LogInfo("Will make custom pop up for archipelago.");

            title = "Archipelago Setup";
            confirmation = "Is your room info updated in ...\\ArchipelagoRandomizer\\SaveData\\RoomInfo.json ?";
        }
    }
}
