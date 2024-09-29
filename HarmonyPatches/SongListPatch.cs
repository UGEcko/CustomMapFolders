using System.IO;
using System.Linq;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;   

namespace CustomMapFolders;
[HarmonyPatch(typeof(SongList), nameof(SongList.AddDefaultFolders))]
public class SongListPatch : MonoBehaviour
{
    [HarmonyPostfix]
    static void Postfix(SongList __instance)
    {
        var settings = Plugin.settings.settings;

        foreach (var folder in settings)
        {
            __instance.InitFolderObject(folder.Key, folder.Value.path);
        }
        
        // !!! Add the "Add Custom Folder" button
        
        var prefab = Instantiate(__instance.songFolderPrefab, __instance.songFolderPrefab.transform.parent, true);
        var button = prefab.GetComponent<Button>();
        var count = __instance.songFolderObjects.Count;
        button.onClick.AddListener(() =>
        {
            // Init CMUI to ask for folder details.
            DialogBox UI = PersistentUI.Instance.CreateNewDialogBox().WithTitle("Create new folder");
            
            var name = UI.AddComponent<TextBoxComponent>()
                .WithLabel("Name");
            var path = UI.AddComponent<TextBoxComponent>()
                .WithLabel("Path");
            var color = UI.AddComponent<NestedColorPickerComponent>()
                .WithLabel("Color")
                .WithInitialValue(new Color(255, 255, 255, 255));
            
            UI.AddFooterButton(() => // Cancel button
            {
                UI.Close();
            }, "Cancel");
            
            UI.AddFooterButton(() => // Add folder button
            {
                if (name.Value != null && path.Value != null && Directory.Exists(path.Value))
                {
                    CustomMapFolders.Plugin.settings.AddFolder(name.Value, path.Value, new Settings.SerializableColor(color.Value));
                    new FolderManager().InitFolder(name.Value,
                        path.Value,
                        Instantiate(__instance.songFolderPrefab, __instance.songFolderPrefab.transform.parent, true),
                        __instance);
                    Plugin.settings.UpdateConfig();
                    UI.Close();
                }
                else
                {
                    Debug.LogError($"Please ensure you have filled in all required fields (name and path).");
                }
                UI.Close();
            }, "Create Folder");
            
            UI.Open();
        });
        
        var childObject = prefab.transform.GetChild(0).gameObject;
        var text = childObject.GetComponent<TextMeshProUGUI>();
        text.text = "+";

        __instance.songFolderObjects.Add(prefab);
        __instance.songFolderPaths.Add(@"C:\\");
        
        // Modify width
        var layoutController = prefab.GetComponent<LayoutElement>();
        layoutController.preferredWidth = 13;
        
        // Tooltip
        var tooltip = prefab.AddComponent<Tooltip>();
        tooltip.TooltipOverride = "Add a custom folder";

        prefab.SetActive(true);

        Plugin.addButton = prefab;
    }
}