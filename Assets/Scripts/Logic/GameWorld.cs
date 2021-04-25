using System;
using System.Collections.Generic;

public class GameWorld
{
	public readonly LevelData LevelData;
	private readonly Tile[,] tiles;

	public GameWorld(LevelData levelData)
	{
		LevelData = levelData;

		tiles = new Tile[levelData.Width, levelData.Height];
		for (var i = 0; i < levelData.Width; i++)
		for (var j = 0; j < levelData.Height; j++)
		{
			var tileType = levelData.Tiles[i + j * levelData.Width];
			var tile = new Tile(this, i, j, tileType);
			tiles[i, j] = tile;
		}
	}

	public Tile GetTile(int x, int y)
	{
		if (x < 0 || x >= LevelData.Width)
			return null;
		if (y < 0 || y >= LevelData.Height)
			return null;

		return tiles[x, y];
	}
}