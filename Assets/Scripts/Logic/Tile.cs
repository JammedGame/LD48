using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
	public readonly GameWorld World;
	public readonly int X, Y;
	public readonly int SoilVariant;
	public readonly Layer Layer;
	public readonly TileType TileType;
	public FacilityType FacilityType { get; private set; }
	public bool HasFacility => FacilityType != FacilityType.None;

	public Tile(GameWorld world, int x, int y, TileData tileData, int soilVariant)
	{
		this.World = world;
		TileType = tileData.TileType;
		Layer = tileData.Layer;
		SoilVariant = soilVariant;
		FacilityType = tileData.FacilityType;
		X = x;
		Y = y;
	}

	private TileTypeSettings tileTypeSettings => GameSettings.Instance.GetSettings(TileType);
	private FacilitySettings facilitySettings => GameSettings.Instance.GetSettings(FacilityType);

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

	public override string ToString()
	{
		return $"Tile[{X}, {Y}] {TileType}_{Layer}";
	}

	public void SetTile(FacilityType facility)
	{
		FacilityType = facility;
	}
}