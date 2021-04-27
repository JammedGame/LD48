using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
	public readonly GameWorld World;
	public readonly int X, Y;
	public readonly int SoilVariant;
	public readonly Layer Layer;
	public TileType TileType { get; private set; }
	public FacilityType FacilityType { get; private set; }
	public bool HasFacility => FacilityType != FacilityType.None;
	public bool IsTunnel => FacilityType == FacilityType.Tunnel || FacilityType == FacilityType.TerraformingFacility;

	public Tile(GameWorld world, int x, int y, TileData tileData, int soilVariant)
	{
		this.World = world;
		TileType = tileData.TileType;
		Layer = tileData.Layer;
		SoilVariant = soilVariant;
		SetFacility(tileData.FacilityType);
		X = x;
		Y = y;
	}

	private TileTypeSettings tileTypeSettings => GameSettings.Instance.GetSettings(TileType);
	private FacilitySettings GetFacilitySettings() => GameSettings.Instance.GetSettings(FacilityType);

	public FacilityType GetAdjecentFacility(Direction direction)
	{
		var tile = GetAdjecentTile(direction);
		return tile != null ? tile.FacilityType : FacilityType.None;
	}

	public Direction? GetFirstNeighbourTunnel()
	{
		if (HasAdjecentTunnel(Direction.Left)) return Direction.Left;
		if (HasAdjecentTunnel(Direction.Top)) return Direction.Top;
		if (HasAdjecentTunnel(Direction.Right)) return Direction.Right;
		if (HasAdjecentTunnel(Direction.Bottom)) return Direction.Bottom;
		return null;
	}

	public Tile GetAdjecentTile(Direction direction)
	{
		switch (direction)
		{
			case Direction.Left: return World.GetTile(X - 1, Y);
			case Direction.Right: return World.GetTile(X + 1, Y);
			case Direction.Top: return World.GetTile(X, Y - 1);
			case Direction.Bottom: return World.GetTile(X, Y + 1);
			default: throw new Exception($"Undefined direction: {direction}");
		}
	}

	public bool HasAdjecentTunnel(Direction direction) => GetAdjecentTile(direction) is Tile tile && tile.IsTunnel;

	public override string ToString()
	{
		return $"Tile[{X}, {Y}] {TileType}_{Layer}";
	}

	public bool ExtractSoilDeposits()
	{
		if (TileType == TileType.Deposit)
		{
			TileType = TileType.Soil;
			return true;
		}

		return false;
	}

	public void SetFacility(FacilityType facility)
	{
		if (facility == FacilityType.None)
		{
			World.OnFacilityRemoved(this);
		}
		else
		{
			World.OnFacilityAdded(this);
		}

		FacilityType = facility;
	}

	public int GetEnergyProduction()
	{
		if (FacilityType == FacilityType.None)
			return 0;

		var settings = GetFacilitySettings();
		if (settings == null)
			return 0;

		return settings.EnergyContribution.Get(Layer);
	}

	public int GetMineralProduction()
	{
		if (FacilityType == FacilityType.None)
			return 0;

		var settings = GetFacilitySettings();
		if (settings == null)
			return 0;

		return settings.Production.MineralProduction.GetProduction(Layer);
	}
}