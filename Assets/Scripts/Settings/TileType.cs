using System.Collections.Generic;
using UnityEngine;

public enum Layer
{
	Undefined = 0,
	A = 2,
	B = 3,
	C = 4,
}

public enum TileType
{
	Undefined = 0,

	// NATURAL
	Surface = 1,
	Soil,
	Mineral,
	Deposit,
	Granite,
	Magma,
	Core
}

public enum FacilityType
{
	None = 0,

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
	public FacilityType FacilityType; // for initial facilities placed on start.

	public TileData(TileType tileType, Layer layer, FacilityType facilityType = FacilityType.None)
	{
		TileType = tileType;
		Layer = layer;
		FacilityType = facilityType;
	}

	public TileData WithFacility(FacilityType facility, TileType tileType)
	{
		return new TileData(tileType, Layer, facility);
	}
}