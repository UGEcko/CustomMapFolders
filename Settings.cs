using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;

namespace CustomMapFolders;

public class Settings
{
    public Dictionary<string,FolderSettings> settings = new Dictionary<string, FolderSettings>();

    public void Init()
    {
        if (File.Exists(Plugin.SettingsPath))
        {
            string data = File.ReadAllText(Plugin.SettingsPath);
            settings = JsonConvert.DeserializeObject<Dictionary<string, FolderSettings>>(data);
            if (settings == null)
            {
                Debug.Log("Settings are null.");
            }
        }
        else
        {
            Debug.Log("Creating file for CustomMapFolders configuration...");
            UpdateConfig();
        }
    }

    public void UpdateConfig()
    {
        File.WriteAllText(Plugin.SettingsPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
    }

    public void AddFolder( string name, string path, SerializableColor? color = null)
    {
        if (!Directory.Exists(path))
        {
            Debug.Log($"{path} is not a valid directory. Please try again.");
            return;
        }

        if (settings.Where(x => x.Key == name).FirstOrDefault().Key != null)
        {
            Debug.LogWarning($"{name} custom folder already exists. Please choose a different name.");
            return;
        }

        SerializableColor folderColor = color ?? new SerializableColor(Color.white);
        
        settings.Add(name, new FolderSettings
        {
            path = path,
            color = folderColor,
        });
    }

    public void RemoveFolder(string name)
    {
        var folder = settings.Where(x => x.Key == name).FirstOrDefault();
        Debug.Log($"Removing {folder.Key}");
        if (folder.Value != null)
        {
            settings.Remove(folder.Key);
            Debug.Log($"Removed {folder.Key}");
        }
    }


    public interface ISetting
    {
        string? path { get; set; }
        
        SerializableColor? color { get; set; }
    }

    public class FolderSettings : ISetting
    {
        public string? path { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SerializableColor? color { get; set; }
    }
    
    public class SerializableColor // For the serializer.
    {
        public float r, g, b, a;

        public SerializableColor(UnityEngine.Color color)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
        }

        public UnityEngine.Color ToUnityColor()
        {
            return new UnityEngine.Color(r, g, b, a);
        }
    }

}