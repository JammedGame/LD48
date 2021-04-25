using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
	public readonly GameWorld World;
	public readonly int X, Y;

	public Tile(GameWorld world, int x, int y, TileType type)
	{
		this.World = world;
		TileType = type;
		X = x;
		Y = y;
	}

	private TileTypeSettings tileTypeSettings => GameSettings.Instance.GetSettings(TileType);


	public TileType TileType { get; private set; }

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
}