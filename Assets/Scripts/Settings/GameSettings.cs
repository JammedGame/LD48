using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    public List<TileTypeSettings> SettingsPerType;
    public List<LevelData> AllLevels;

    public TileTypeSettings GetSettings(TileType tileType)
    {
        foreach(var type in SettingsPerType)
        {
            if (type.TileType == tileType)
            {
                return type;
            }
        }

        return null;
    }

    public (Texture texture, Texture overlayTexture) GetTexture(TileType tileType)
    {
        if (!cache.TryGetValue((int)tileType, out var result))
        {
            var src = tileType;
            result = (Resources.Load<Texture>($"Tiles/{src}"), Resources.Load<Texture>($"Tiles/{src}Overlay"));
            cache[(int) tileType] = result;
        }
        return result;
    }

    Dictionary<int, (Texture texture, Texture overlayTexture)> cache =
        new Dictionary<int, (Texture texture, Texture overlayTexture)>();

    static GameSettings instance;

    public static GameSettings Instance
    {
        get
        {
            if (instance) return instance;
            instance = Resources.Load<GameSettings>("Settings/GameSettings");
            return instance;
        }
    }

    void OnValidate()
    {
        if (SettingsPerType == null)
            SettingsPerType = new List<TileTypeSettings>();

        foreach(var type in System.Enum.GetValues(typeof(TileType)))
        {
            if (type is TileType tileType)
            {
                var src = tileType;

                var settings = GetSettings(tileType);
                if (settings == null)
                {
                    settings = new TileTypeSettings() {
                        TileType = tileType
                    };
                    SettingsPerType.Add(settings);
                }
            }

            SettingsPerType.Sort((a, b) => a.TileType.CompareTo(b.TileType));
        }
    }
}