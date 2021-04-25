using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
	public int Width = 10;
	public int Height = 25;
	public TileType[] Tiles { get; }

	public void Initialize()
	{
		Array.Clear(Tiles, 0, Width * Height);
	}

	public TileData GetTile(int index)
	{
		return new TileData
		{
			Type = Tiles[index], X = index % Width, Y = index / Width
		};
	}

	public void SetTileType(int index, TileType type)
	{
		Tiles[index] = type;
	}
}

public struct TileData
{
	public int X;
	public int Y;
	public TileType Type;
}