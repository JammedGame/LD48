using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
	[Header("Wallet")]
	public int InitialMinerals;
	public int InitialEnergy;
	public int InitialEnergyCap;
	public ValuePerLayer DepositReward;

	[Header("Tile Settings")]
    public List<TileTypeSettings> SettingsPerType;

    [Header("Facility Settings")]
    public List<FacilitySettings> FacilitySettings;

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

    public FacilitySettings GetSettings(FacilityType type)
    {
        if (type == FacilityType.None)
			return null;

		foreach(var setting in FacilitySettings)
        {
            if (setting.FacilityType == type)
            {
                return setting;
            }
        }

		Debug.LogError($"Missing settings for type: {type}");
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
            }

            SettingsPerType.Sort((a, b) => a.TileType.CompareTo(b.TileType));
        }
    }
}

[Serializable]
public struct ValuePerLayer
{
	public int A, B, C;

    public int Get(Layer l)
    {
        switch(l)
        {
            case Layer.A: return A;
            case Layer.B: return A;
            case Layer.C: return C;
			default: return 0;
		}
    }
}