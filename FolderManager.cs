using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using CustomMapFolders.Interactive;
using UnityEngine.Localization.Components;


namespace CustomMapFolders;

public class FolderManager
{
    public void RemoveFolder(string tabName, string folderPath, GameObject gameObj, SongList __instance)
    {
        __instance.songFolderObjects.Remove(gameObj);
        __instance.songFolderPaths.Remove(folderPath);
        GameObject.Destroy(gameObj);
    }
    public void InitFolder(string tabName, string folderPath, GameObject prefab, SongList __instance)
    {
        var button = prefab.GetComponent<Button>();
        var count = __instance.songFolderObjects.Count;
        button.onClick.AddListener(() => __instance.SetSongLocation(count));
        
        var childObject = prefab.transform.GetChild(0).gameObject;
        var text = childObject.GetComponent<TextMeshProUGUI>();
        text.text = tabName;
        // PLUGIN
        var customFolder = Plugin.settings.settings.Where(x => x.Key == tabName).FirstOrDefault().Value;
        if (customFolder != null && customFolder.color != null)
        {
            text.color = customFolder.color.ToUnityColor();
            var folderComponent = prefab.gameObject.AddComponent<CustomFolder>();
            folderComponent.folderPath = folderPath;
            folderComponent.folderName = tabName;
            folderComponent.folderColor = customFolder.color;
            folderComponent.TMP = text;
            folderComponent.button = button;
        }
        // PLUGIN

        __instance.songFolderObjects.Add(prefab);
        __instance.songFolderPaths.Add(folderPath);

        prefab.SetActive(true);
        
        if(Plugin.addButton != null) Plugin.addButton.transform.SetAsLastSibling();

        // Use localised strings for default folders
        if (folderPath == global::Settings.Instance.CustomWIPSongsFolder || folderPath == global::Settings.Instance.CustomSongsFolder)
        {
            var localizeStringComponent = childObject.GetComponent<LocalizeStringEvent>();
            localizeStringComponent.StringReference.TableEntryReference =
                folderPath == global::Settings.Instance.CustomWIPSongsFolder
                    ? "wip"
                    : "custom";
            localizeStringComponent.enabled = true;
        }
    }
}