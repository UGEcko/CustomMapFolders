using System;
using HarmonyLib;
using UnityEngine;

namespace CustomMapFolders;

[Plugin("CustomMapFolders")]
public class Plugin
{
    private static string harmonyID = "ugecko.customMapFolders";
    private static Harmony harmony = null;
    
    internal static string SettingsPath = UnityEngine.Application.persistentDataPath + "/CustomMapFolders.json";

    internal static GameObject addButton;
    
    internal static SongList songList;
    
    internal static Settings settings { get; private set; }
    [Init]
    void Init()
    {
        harmony = new Harmony(harmonyID);
        harmony.PatchAll();
        settings = new Settings();
        settings.Init();
    }
    
    [Exit]
    void Exit()
    {
        if(harmony != null) harmony.UnpatchSelf();
    }
}