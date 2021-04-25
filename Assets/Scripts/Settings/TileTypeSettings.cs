using System;
using UnityEngine;

[Serializable]
public class TileTypeSettings
{
	public TileType TileType;
	public string Name;
	public FacilitySettings FacilitySettings;
	public bool IsFacility => FacilitySettings != null;
}

[Serializable]
public class FacilitySettings
{
	public int ConstructionTime;
	public MineralPrice MineralPrice;
	public EnergyContribution EnergyContribution;
	public Production Production;
	public Requirements Requirements;
}

[Serializable]
public class MineralPrice
{
	public int FlatPrice;
	public bool DependsOnSoil;
	public int Soil1Price;
	public int Soil2Price;
	public int Soil3Price;

	public int GetPrice(TileType soil)
	{
		// todo jole
		return 0;
	}
}

[Serializable]
public class EnergyContribution
{
	public int FlatContribution;
	public bool DependsOnAdjacentMagma;
	public int PerAdjacentMagmaContribution;

	public int GetProduction(TileType[] adjacentTiles)
	{
		// todo jole
		return 0;
	}
}

[Serializable]
public class Production
{
	public MineralProduction MineralProduction;
	public float AdjacentFacilitiesBoost;
	public int ConstructionTimeReductionGlobal;
}

[Serializable]
public class MineralProduction
{
	public int FlatProduction;
	public bool DependsOnSoil;
	public int Soil1Production;
	public int Soil2Production;
	public int Soil3Production;

	public int GetProduction(TileType soil)
	{
		// todo jole
		return 0;
	}
}

[Serializable]
public class Requirements
{
	public TileType[] GlobalFacilityRequirement;
	public TileType[] AdjacentTileRequirements;

	public bool IsAllowed(TileType[] allFacilities, TileType[] adjacentTiles)
	{
		// todo jole
		return false;
	}
}

public enum Direction
{
	Undefined = 0,
	Left = 1,
	Right = 2,
	Top = 3,
	Bottom = 4
}