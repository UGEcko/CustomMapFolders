using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CustomMapFolders.Interactive;

public class CustomFolder : MonoBehaviour,  IPointerClickHandler
{
    public string? folderName;
    public string? folderPath;
    public Settings.SerializableColor? folderColor;

    public TMPro.TextMeshProUGUI TMP;
    public Button button;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if the right mouse button was clicked
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // SETTINGS OBJ
            var folderSetting = Plugin.settings.settings.Where(x => x.Key == folderName).FirstOrDefault();
            
            // Add CMUI for the custom folder
            var obj = this.gameObject;
            Debug.Log($"Selected {folderName}");
            
            
            // FIELDS
            
            DialogBox UI = PersistentUI.Instance.CreateNewDialogBox().WithTitle("Update Custom Folder");
            
            var name = UI.AddComponent<TextBoxComponent>()
                .WithLabel("Name")
                .WithInitialValue(folderName);
            var path = UI.AddComponent<TextBoxComponent>()
                .WithLabel("Path")
                .WithInitialValue(folderPath);
            var color = UI.AddComponent<NestedColorPickerComponent>()
                .WithLabel("Color")
                .WithInitialValue(folderColor.ToUnityColor()); // REPLACE THIS INITIAL VALUE WITH THE VALUE OF THE FOLDER COLOR.
            
            
            // BUTTONS
            
            
            UI.AddFooterButton(() => // Cancel button
            {
                UI.Close();
            }, "Cancel");
            
            UI.AddFooterButton(() => // Delete button
            {
                new FolderManager().RemoveFolder(folderName, folderPath, obj, Plugin.songList);
                CustomMapFolders.Plugin.settings.RemoveFolder(folderName);
                Plugin.settings.UpdateConfig();
                UI.Close();
            }, "Delete");
            
            UI.AddFooterButton(() => // Update button
            {
                if (name.Value != null && path.Value != null && Directory.Exists(path.Value))
                {
                    // Check for changes.
                    if (name.Value != folderName)
                    {
                        TMP.text = name.Value;
                        folderName = name.Value;
                    }
                    else if (color.Value != folderColor.ToUnityColor())
                    {
                        TMP.color = color.Value;
                        folderColor = new Settings.SerializableColor(color.Value);
                    } 
                    else if (path.Value != folderPath)
                    {
                        var oldPath = Plugin.songList.songFolderPaths.Where(x => x == folderPath).FirstOrDefault();
                        if (oldPath != null)
                        {
                            oldPath = path.Value;
                            folderPath = path.Value;
                        }
                    }
                    else
                    {
                        UI.Close();
                        return;
                    }
                    KeyValuePair<string, Settings.FolderSettings> newFolderSettings = new KeyValuePair<string, Settings.FolderSettings>(name.Value, new Settings.FolderSettings
                    {
                        path = path.Value,
                        color = new Settings.SerializableColor(color.Value),
                    });

                    Plugin.settings.settings.Remove(folderSetting.Key);
                    Plugin.settings.settings[newFolderSettings.Key] = newFolderSettings.Value;
                    
                    
                    Plugin.settings.UpdateConfig();
                    UI.Close();
                }
                else
                {
                    Debug.LogError($"Please ensure you have filled in all required fields (name and path).");
                }
                UI.Close();
            }, "Update");
            
            UI.Open();
        }
    }
}