using UnityEngine;

public enum TileType
{
	Undefined = 0,

	// NATURAL
	Atmosphere = 1,
	Soil1,
	Soil2,
	Soil3,
	Mineral1,
	Mineral2,
	Mineral3,
	Granite,
	Magma,
	Core,
	DepositPerk,

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
	public static (Texture tex, Texture overlay) LoadTexture(this TileType type)
	{
		return GameSettings.Instance.GetTexture(type);
	}
}