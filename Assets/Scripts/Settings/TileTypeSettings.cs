using System;
using UnityEngine;
using System.Linq;

[Serializable]
public class TileTypeSettings
{
	public TileType TileType;
	public string Name;
}

[Serializable]
public class MineralPrice
{
	public int FlatPrice;
	public bool DependsOnSoil;
	public int Soil1Price;
	public int Soil2Price;
	public int Soil3Price;

	public int GetPrice(Layer layer)
	{
		if (!DependsOnSoil)
			return FlatPrice;

		switch (layer)
		{
			case Layer.A:
				return FlatPrice + Soil1Price;
			case Layer.B:
				return FlatPrice + Soil2Price;
			case Layer.C:
				return FlatPrice + Soil3Price;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}

[Serializable]
public class EnergyContribution
{
	public int FlatContribution;
	public bool DependsOnAdjacentMagma;
	public int PerAdjacentMagmaContribution;

	public int GetContribution(TileType[] adjacentTiles)
	{
		if (!DependsOnAdjacentMagma) return FlatContribution;

		return FlatContribution + Array.FindAll(adjacentTiles, t => t == TileType.Magma).Length * PerAdjacentMagmaContribution;
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

	public int GetProduction(Layer layer)
	{
		if (!DependsOnSoil)
			return FlatProduction;

		switch (layer)
		{
			case Layer.A:
				return FlatProduction + Soil1Production;
			case Layer.B:
				return FlatProduction + Soil2Production;
			case Layer.C:
				return FlatProduction + Soil3Production;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}

[Serializable]
public class Requirements
{
	public TileType[] GlobalFacilityRequirements;
	public TileType[] AdjacentTileRequirements;

	public bool AreMet(TileType[] allFacilities, TileType[] adjacentTiles)
	{
		var globalMet = GlobalFacilityRequirements.Except(allFacilities).Count() == 0;
		var adjacentMet = AdjacentTileRequirements.Except(adjacentTiles).Count() == 0;
		return globalMet && adjacentMet;
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

public struct DirectionMask
{
	public bool Left;
	public bool Right;
	public bool Top;
	public bool Bottom;

	public bool Get(Direction dir)
	{
		switch(dir)
		{
			case Direction.Left: return Left;
			case Direction.Top: return Top;
			case Direction.Right: return Right;
			case Direction.Bottom: return Bottom;
		}

		return false;
	}
}