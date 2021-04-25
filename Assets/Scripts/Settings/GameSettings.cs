using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
	[Header("Wallet")]
	public int InitialMinerals;
	public int InitialEnergy;
	public int InitialEnergyCap;

	[Header("Tile Settings")]
    public List<TileTypeSettings> SettingsPerType;
    public List<LevelGenerator> AllLevels;

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

                if (settings.Name == null || settings.Name.Length == 0)
                {
                    settings.Name = settings.TileType.ToString();
                }

                settings.IsFacility = (int)settings.TileType > 100;
                if (settings.IsFacility) settings.FacilitySettings = new FacilitySettings();
                else settings.FacilitySettings = null;
            }

            SettingsPerType.Sort((a, b) => a.TileType.CompareTo(b.TileType));
        }
    }
}