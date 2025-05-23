using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro;
using static CatQuest_Randomizer.SaveDataHandler;

namespace CatQuest_Randomizer.Patches
{
    [HarmonyPatch(typeof(Title), nameof(Title.StartNormalGame))]
    public class NewGamePatch
    {
        static void Prefix()
        {
            Randomizer.Logger.LogInfo("New game was started");
            //When implemented, gets implemented as an extension that takes the info written in the ui to login.
            //SaveDataHandler.SaveRoomInfo(server, player, password);
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
    public class AddArchipelagoMenuPatch
    {
        static void Prefix(NewGamePanel __instance)
        {
            Randomizer.Logger.LogInfo("Will add extra menu item for archipelago.");
            
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
            var lastItem = listItems[listItems.Count - 1];
            var newItem = Object.Instantiate(lastItem, lastItem.transform.parent);
            newItem.name = lastItem.name + "Archipelago Setup";

            // Access and modify private 'optionDescription' field
            FieldInfo descField = typeof(NewGamePanelOption).GetField("optionDescription", BindingFlags.NonPublic | BindingFlags.Instance);
            if (descField != null)
            {
                var textComponent = descField.GetValue(newItem) as TextMeshProUGUI;
                if (textComponent != null)
                {
                    textComponent.text = "Setup room info for Archipelago";
                }
            }
            else
            {
                Randomizer.Logger.LogError("Could not find 'optionDescription' field in NewGamePanelOption.");
            }

            newItem.OnMouseClick += ArchipelagoListItem_OnMouseClick;

            newItem.SetName("Archipelago");

            // Add the clone to the list
            listItems.Add(newItem);
        }
        public static void ArchipelagoListItem_OnMouseClick(NewGamePanelOption obj)
        {
            Randomizer.Logger.LogInfo("Will show Archipelago Room Info Pop Up here.");
            //Open Archipelago Room Info Pop Up here, then return
        }
    }
}
