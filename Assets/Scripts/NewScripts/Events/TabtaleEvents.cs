/*using System;
using System.Collections.Generic;
using Tabtale.TTPlugins;
using UnityEngine;
using Newtonsoft.Json;

public static class TabtaleEvents
{
    private static Dictionary<string, int> _unlockables;
    private static int _unlockedTilesCount;
    private static bool _unlockablesLoaded;
    private static bool _tilesLoaded;

    public static void OnTileUnlocked(int tile)
    {
        if (!_tilesLoaded)
        {
            LoadTiles();
        }

        if (tile <= _unlockedTilesCount) return;

        if (tile > 1)
        {
            TTPGameProgression.FirebaseEvents.MissionComplete(null);
        }
        TTPGameProgression.FirebaseEvents.MissionStarted(tile, null);
        _unlockedTilesCount = tile;
        SaveTiles();
    }

    public static void OnObjectUnlocked(DuplicateScales duplicateScales)
    {
        if (!_unlockablesLoaded)
        {
            LoadUnlockables();
        }

        var dictionary = new Dictionary<string, object>();
        var objectType = Enum.GetName(typeof(DuplicateScales), duplicateScales);

        IncreaseUnlockableCount(objectType);
        dictionary.Add(objectType, GetActivatedObjectsCount(objectType));
        TTPAnalytics.LogEvent(AnalyticsTargets.ANALYTICS_TARGET_FIREBASE, "unlockedObject", dictionary, false);
    }

    private static void IncreaseUnlockableCount(string unlockableKey)
    {
        if (_unlockables.ContainsKey(unlockableKey))
        {
            _unlockables[unlockableKey]++;
        }
        else
        {
            _unlockables.Add(unlockableKey, 1);
        }

        SaveUnlockables();
    }

    private static int GetActivatedObjectsCount(string unlockableKey)
    {
        if (_unlockables.ContainsKey(unlockableKey))
        {
            return _unlockables[unlockableKey];
        }

        return 0;
    }

    private static void SaveTiles()
    {
        PlayerPrefs.SetInt(nameof(_unlockedTilesCount), _unlockedTilesCount);
    }

    private static void LoadTiles()
    {
        _unlockedTilesCount = PlayerPrefs.GetInt(nameof(_unlockedTilesCount), 0);
        _tilesLoaded = true;
    }

    private static void SaveUnlockables()
    {
        var unlockablesJson = JsonConvert.SerializeObject(_unlockables);
        Debug.Log(unlockablesJson);
        PlayerPrefs.SetString(nameof(_unlockables), unlockablesJson);
    }

    private static void LoadUnlockables()
    {
        var unlockablesJson = PlayerPrefs.GetString(nameof(_unlockables));
        _unlockables = string.IsNullOrEmpty(unlockablesJson)
            ? new Dictionary<string, int>()
            : JsonConvert.DeserializeObject<Dictionary<string, int>>(unlockablesJson);
        _unlockablesLoaded = true;
    }
}
*/
