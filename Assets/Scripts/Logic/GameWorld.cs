using System;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld
{
	public readonly LevelData LevelData;
	private readonly Tile[,] tiles;

	// player's wallet
	public int Minerals { get; private set; }
	public int Energy { get; private set; }
	public int EnergyCap { get; private set; }

	public GameWorld(LevelData levelData)
	{
		LevelData = levelData;

		// init tile
		tiles = new Tile[levelData.Width, levelData.Height];
		for (var i = 0; i < levelData.Width; i++)
		for (var j = 0; j < levelData.Height; j++)
		{
			var tileType = levelData.Tiles[i, j];
			var soilVariant = levelData.SoilVariants[i, j];
			var tile = new Tile(this, i, j, tileType, soilVariant);
			tiles[i, j] = tile;
		}

		// init player's wallet
		var settings = GameSettings.Instance;
		Minerals = settings.InitialMinerals;
		Energy = settings.InitialEnergy;
		EnergyCap = settings.InitialEnergyCap;
	}

	public Tile GetTile(int x, int y)
	{
		if (x < 0 || x >= LevelData.Width)
			return null;
		if (y < 0 || y >= LevelData.Height)
			return null;

		return tiles[x, y];
	}

	public bool ValidateCoords(int x, int y)
	{
		if (x < 0 || x >= LevelData.Width)
			return false;
		if (y < 0 || y >= LevelData.Height)
			return false;

		return true;
	}

	public bool ProcessAction(PlayerAction action)
	{
		if (!Validate(action))
			return false;

		var tile = GetTile(action.X, action.Y);
		if (tile.TileType == TileType.Deposit)
		{
			// add deposit instant reward
		}

		ProcessTurn();
		return true;
	}

	public bool Validate(PlayerAction action)
	{
		var tile = GetTile(action.X, action.Y);
		if (tile == null)
			return false;

		// validate price agains wallet
		// validate tech tree
		// validate special tile conditions

		return true;
	}

	public void ProcessTurn()
	{
	}
}

public struct PlayerAction
{
	public TileType TileToPlace;
	public int X, Y;
}