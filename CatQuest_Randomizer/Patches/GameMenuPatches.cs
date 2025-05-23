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
                    textComponent.text = "Setup room info and start Archipelago";
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
            confirmation = "Enter your room information in ...\\ArchipelagoRandomizer\\SaveData\\RoomInfo.json";

            //FieldInfo confirmationPanelField = typeof(Title).GetField("confirmationPanel", BindingFlags.NonPublic | BindingFlags.Instance);
            //if (confirmationPanelField == null) return;

            //var confirmationPanel = confirmationPanelField.GetValue(__instance) as ConfirmationPanel;
            //if (confirmationPanel == null) return;

            //FieldInfo confirmationField = typeof(ConfirmationPanel).GetField("confirmation", BindingFlags.NonPublic | BindingFlags.Instance);
            //if (confirmationField == null) return;

            //var confirmationText = confirmationField.GetValue(confirmationPanel) as TextMeshProUGUI;
            //if (confirmationText == null) return;

            //// Clone the GameObject
            //var newTextObj = Object.Instantiate(confirmationText.gameObject, confirmationText.transform.parent);
            //newTextObj.name = "ArchipelagoRoomPrompt";

            //// Move it downward
            //var rt = newTextObj.GetComponent<RectTransform>();
            //rt.anchoredPosition -= new Vector2(0, rt.sizeDelta.y + 10); // move down

            //// Update the text
            //var tmp = newTextObj.GetComponent<TextMeshProUGUI>();
            //tmp.text = "Enter your Archipelago Server, Slot, and Password.";



            //var originalOnNo = onNo;
            //onNo = () =>
            //{
            //    Randomizer.Logger.LogInfo("Archipelago NO was clicked!");
            //    Object.Destroy(newTextObj);

            //    originalOnNo?.Invoke();
            //};

            //var originalOnYes = onYes;
            //onYes = () =>
            //{
            //    Randomizer.Logger.LogInfo("Archipelago YES was clicked!");
            //    //SaveDataHandler.SaveRoomInfo(string server, string player, string password)

            //    originalOnYes?.Invoke();
            //};
        }
    }

    [HarmonyPatch(typeof(ConfirmationPanel), "Hide")]
    public class ConfirmationPanelHidePatch
    {
        static void Prefix(ConfirmationPanel __instance)
        {
            Transform extraPrompt = __instance.transform.Find("ArchipelagoRoomPrompt");
            if (extraPrompt != null)
            {
                UnityEngine.Object.Destroy(extraPrompt.gameObject);
                Randomizer.Logger.LogInfo("Destroyed ArchipelagoRoomPrompt after confirmation panel closed.");
            }
        }
    }
}
