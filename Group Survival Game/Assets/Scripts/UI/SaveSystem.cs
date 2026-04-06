using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class LegacySaveState
{
    public Vector3 pos;
    public Quaternion rot;
    public string savedAtUtc;
}

[Serializable]
public class SaveEntryData
{
    public string saveId;
    public string componentType;
    public string json;
}

[Serializable]
public class SaveFileData
{
    public int version = 2;
    public string savedAtUtc;
    public List<SaveEntryData> entries = new List<SaveEntryData>();
}

public static class SaveSystem
{
    private const string FolderName = "saves";
    private const string FilePrefix = "save_";
    private const string FileExt = ".json";

    public static string SaveFolderPath
    {
        get
        {
            string p = Path.Combine(Application.persistentDataPath, FolderName);
            if (!Directory.Exists(p)) Directory.CreateDirectory(p);
            return p;
        }
    }

    public static string CreateNextSavePath()
    {
        int max = 0;
        foreach (var f in Directory.GetFiles(SaveFolderPath, $"{FilePrefix}*{FileExt}"))
        {
            string name = Path.GetFileNameWithoutExtension(f);
            if (!name.StartsWith(FilePrefix)) continue;

            string numStr = name.Substring(FilePrefix.Length);
            if (int.TryParse(numStr, out int n))
                max = Mathf.Max(max, n);
        }

        int next = max + 1;
        string fileName = $"{FilePrefix}{next:0000}{FileExt}";
        return Path.Combine(SaveFolderPath, fileName);
    }

    public static string SaveNew(Transform player)
    {
        if (player == null)
        {
            Debug.LogWarning("[SaveSystem] Player not assigned.");
            return null;
        }

        SaveFileData fileData = new SaveFileData
        {
            savedAtUtc = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };

        var saveables = FindAllSaveables();
        HashSet<string> usedIds = new HashSet<string>();

        foreach (var saveable in saveables)
        {
            if (saveable == null) continue;

            if (string.IsNullOrWhiteSpace(saveable.SaveId))
            {
                Debug.LogWarning($"[SaveSystem] Missing saveId on {saveable.name}");
                continue;
            }

            if (!usedIds.Add(saveable.SaveId))
            {
                Debug.LogWarning($"[SaveSystem] Duplicate saveId: {saveable.SaveId} on {saveable.name}");
                continue;
            }

            fileData.entries.Add(new SaveEntryData
            {
                saveId = saveable.SaveId,
                componentType = saveable.GetType().FullName,
                json = saveable.CaptureAsJson()
            });
        }

        // 兼容：如果场景里还没挂任何 SaveableBehaviour，至少仍然保存 player 的位置/旋转
        if (fileData.entries.Count == 0)
        {
            LegacySaveState fallback = new LegacySaveState
            {
                pos = player.position,
                rot = player.rotation,
                savedAtUtc = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };

            string legacyJson = JsonUtility.ToJson(fallback, true);
            string legacyPath = CreateNextSavePath();
            File.WriteAllText(legacyPath, legacyJson);
            Debug.Log("[SaveSystem] Saved fallback legacy save to: " + legacyPath);
            return legacyPath;
        }

        string json = JsonUtility.ToJson(fileData, true);
        string path = CreateNextSavePath();
        File.WriteAllText(path, json);
        Debug.Log("[SaveSystem] Saved to: " + path);
        return path;
    }

    public static bool LoadFromFile(Transform player, string path)
    {
        if (!File.Exists(path))
            return false;

        string json = File.ReadAllText(path);

        // 先尝试新版格式
        SaveFileData fileData = null;
        try
        {
            fileData = JsonUtility.FromJson<SaveFileData>(json);
        }
        catch { }

        if (fileData != null && fileData.entries != null && fileData.entries.Count > 0)
        {
            return LoadNewFormat(fileData);
        }

        // 再兼容旧版格式
        LegacySaveState legacyData = null;
        try
        {
            legacyData = JsonUtility.FromJson<LegacySaveState>(json);
        }
        catch { }

        if (legacyData != null && player != null)
        {
            player.position = legacyData.pos;
            player.rotation = legacyData.rot;
            return true;
        }

        return false;
    }

    private static bool LoadNewFormat(SaveFileData fileData)
    {
        var saveables = FindAllSaveables();

        Dictionary<string, SaveableBehaviour> lookup = new Dictionary<string, SaveableBehaviour>();
        foreach (var saveable in saveables)
        {
            if (saveable == null) continue;
            if (string.IsNullOrWhiteSpace(saveable.SaveId)) continue;

            if (!lookup.ContainsKey(saveable.SaveId))
                lookup.Add(saveable.SaveId, saveable);
            else
                Debug.LogWarning($"[SaveSystem] Duplicate saveId in scene: {saveable.SaveId}");
        }

        var ordered = saveables
            .Where(s => s != null)
            .OrderBy(s => s.LoadPriority)
            .ToArray();

        // 1) BeforeRestore
        foreach (var saveable in ordered)
        {
            try
            {
                saveable.BeforeRestore();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] BeforeRestore failed on {saveable.name}: {ex}");
            }
        }

        // 2) RestoreFromJson
        foreach (var entry in fileData.entries)
        {
            if (entry == null) continue;

            if (lookup.TryGetValue(entry.saveId, out var saveable))
            {
                try
                {
                    saveable.RestoreFromJson(entry.json);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[SaveSystem] Restore failed on {saveable.name}: {ex}");
                }
            }
            else
            {
                Debug.LogWarning($"[SaveSystem] No scene object found for saveId: {entry.saveId}");
            }
        }

        // 3) AfterRestore
        foreach (var saveable in ordered)
        {
            try
            {
                saveable.AfterRestore();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] AfterRestore failed on {saveable.name}: {ex}");
            }
        }

        return true;
    }

    public static bool DeleteFile(string path)
    {
        if (!File.Exists(path)) return false;
        File.Delete(path);
        return true;
    }

    public static List<string> ListSaveFilesNewestFirst()
    {
        if (!Directory.Exists(SaveFolderPath)) return new List<string>();

        return Directory.GetFiles(SaveFolderPath, $"{FilePrefix}*{FileExt}")
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .ToList();
    }

    public static string GetDisplayName(string path)
    {
        string file = Path.GetFileName(path);
        DateTime t = File.GetLastWriteTime(path);
        return $"{file}  ({t:yyyy-MM-dd HH:mm})";
    }

    private static SaveableBehaviour[] FindAllSaveables()
    {
#if UNITY_2023_1_OR_NEWER
        return UnityEngine.Object.FindObjectsByType<SaveableBehaviour>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );
#else
        return UnityEngine.Object.FindObjectsOfType<SaveableBehaviour>(true);
#endif
    }
}