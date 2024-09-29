using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;

namespace CustomMapFolders;

[HarmonyPatch(typeof(SongList), nameof(SongList.InitFolderObject))]
public class InitFolderObjectPatch : MonoBehaviour
{
    [HarmonyPrefix]
    static bool Prefix(string tabName, string folderPath, SongList __instance)
    {
        if(Plugin.songList == null) Plugin.songList = __instance;
        var prefab = Instantiate(__instance.songFolderPrefab, __instance.songFolderPrefab.transform.parent, true);
        new FolderManager().InitFolder(tabName, folderPath, prefab, __instance);

        return false; // Skip original thingy
    }
}