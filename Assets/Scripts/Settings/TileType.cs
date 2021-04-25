using System.Collections.Generic;
using UnityEngine;

public enum Layer
{
	Undefined = 0,
	Atmosphere = 1,
	A = 2,
	B = 3,
	C = 4
}

public enum TileType
{
	Undefined = 0,

	// NATURAL
	Atmosphere = 1,
	Soil,
	Mineral,
	Deposit,
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

public struct TileData
{
	public TileType TileType;
	public Layer Layer;

	public TileData(TileType tileType, Layer layer)
	{
		TileType = tileType;
		Layer = layer;
	}
}