using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
	Undefined = 0,

	// NATURAL
	Atmosphere = 1,
	Soil_A,
	Soil_B,
	Soil_C,
	Mineral_A,
	Mineral_B,
	Mineral_C,
	Deposit_A,
	Deposit_B,
	Deposit_C,
	Granite,
	Magma,
	Core,

	// FACILITIES
	TerraformingFacility = 101,
	Tunnel,
	MineralExtractor,
	PowerPlant,
	Battery,
	GeothermalPlant,
	Booster,
	NuclearPlant,
	BuildBotFacility,
	BFCoreExtractor,
}

public static class TileTypeExtensions
{
	const string TilePathDefault = "Tiles/{0}";
	const string SoilTilePath = "Tiles/{0}_{1}";

	static readonly Dictionary<int, Texture> cache = new Dictionary<int, Texture>();

	public static bool IsSoilA(this TileType tileType) => tileType == TileType.Soil_A;
	public static bool IsSoilB(this TileType tileType) => tileType == TileType.Soil_B;
	public static bool IsSoilC(this TileType tileType) => tileType == TileType.Soil_C;

	public static bool IsSoil(this TileType tileType) => tileType.IsSoilA()
		|| tileType == TileType.Soil_B
		|| tileType == TileType.Soil_C;

	public static bool IsMineral(this TileType tileType) =>
		tileType == TileType.Mineral_A
		|| tileType == TileType.Mineral_B
		|| tileType == TileType.Mineral_C;

	public static Texture LoadTexture(this TileType tileType)
	{
		if (!cache.TryGetValue((int)tileType, out var result))
		{
			var path = string.Format(TilePathDefault, tileType);
			result = LoadTileTextureAtPath(path);
			cache[(int)tileType] = result;
		}
		return result;
	}

	public static Texture LoadSoilTexture(this TileType tileType, int variant)
	{
		var path = string.Format(SoilTilePath, tileType, variant);
		return LoadTileTextureAtPath(path);
	}

	// error logging.
	private static Texture LoadTileTextureAtPath(string path)
	{
		var result = Resources.Load<Texture>(path);

		if (result == null)
			Debug.LogError($"No texture found for path: {path}");

		return result;
	}
}