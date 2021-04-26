using System;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld
{
	public readonly LevelData LevelData;
	private readonly Tile[,] tiles;
	private readonly HashSet<FacilityType> builtFacilities = new HashSet<FacilityType>();

	// player's wallet
	public int Minerals { get; private set; }
	public int Energy { get; private set; }
	public int EnergyCap { get; private set; }
	public int ReachedDepth { get; private set; }

	public event Action<PlayerAction> OnTurnOver;

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
		ReachedDepth = levelData.Parent.AtmosphereEndsAt;
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
		if (!ValidateAction(action, out Direction placingDirection))
			return false;

		var tile = GetTile(action.X, action.Y);

		if (tile.ExtractSoilDeposits())
		{
			// todo: add deposit instant reward
		}

		// todo: spend currencies.

		// change tileboard
		tiles[action.X, action.Y].SetFacility(action.Facility, placingDirection);
		ReachedDepth = Mathf.Max(action.Y, ReachedDepth);
		builtFacilities.Add(action.Facility);

		ProcessTurn();
		OnTurnOver?.Invoke(action);
		return true;
	}

	public bool ValidateAction(PlayerAction action, out Direction placingDirection)
	{
		placingDirection = default;

		if (action.Facility == FacilityType.None)
			return false;

		var tile = GetTile(action.X, action.Y);
		if (tile == null)
			return false;

		if (tile.TileType == TileType.Surface)
			return false;
		if (tile.TileType == TileType.Core)
			return false;
		if (tile.TileType == TileType.Granite)
			return false;
		if (tile.TileType == TileType.Mineral && action.Facility != FacilityType.MineralExtractor)
			return false;
		if (tile.TileType != TileType.Mineral && action.Facility == FacilityType.MineralExtractor)
			return false;
		if (tile.TileType == TileType.Magma && action.Facility != FacilityType.GeothermalPowerPlant)
			return false;
		if (tile.TileType != TileType.Magma && action.Facility == FacilityType.GeothermalPowerPlant)
			return false;
		if (action.Facility == FacilityType.BFCoreExtractor)
		{
			var bottomIsCore = tile.GetAdjecentTile(Direction.Bottom) is Tile t && t.TileType == TileType.Core;
			if (!bottomIsCore)
				return false;
		}
		if (tile.HasFacility)
			return false;
		if (!IsFacilityTypeUnlocked(action.Facility))
			return false;

		// todo: validate price against wallet
		// todo: validate tech tree
		// todo: validate special tile conditions

		if (tile.GetFirstNeighbourTunnel() is Direction tunnelConnectionDirection)
		{
			placingDirection = tunnelConnectionDirection;
			return true;
		}

		return false;
	}

	public bool IsFacilityTypeUnlocked(FacilityType facilityType)
	{
		var facilitySettings = GameSettings.Instance.GetSettings(facilityType);
		if (facilitySettings == null)
			return false;
		if (facilitySettings.Requirements.RequirementToUnlock == FacilityType.None)
			return true;

		return builtFacilities.Contains(facilitySettings.Requirements.RequirementToUnlock);
	}

	public void ProcessTurn()
	{
		// todo: spend energy
		// todo: gain minerals
		// todo: gain energy
		// todo: cap energy
	}
}

public struct PlayerAction
{
	public FacilityType Facility;
	public int X, Y;
}