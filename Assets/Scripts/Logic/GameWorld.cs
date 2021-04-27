using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameWorld
{
	public readonly LevelData LevelData;
	private readonly Tile[,] tiles;
	private readonly HashSet<FacilityType> builtFacilities = new HashSet<FacilityType>();
	private readonly List<Tile> tilesWithFacilities = new List<Tile>();

	// player's wallet
	public int Minerals { get; private set; }
	public int BuildPoints { get; private set; }
	public int BuildPointsCap { get; private set; }
	public int Energy { get; private set; }
	public int EnergyCap { get; private set; }
	public int ReachedDepth { get; private set; }
	public int CurrentTurn { get; private set; } = 1;


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
		BuildPointsCap = settings.InitialBuildPoints;
		BuildPoints = settings.InitialBuildPoints;
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
		if (!ValidateAction(action))
			return false;

		var tile = GetTile(action.X, action.Y);
		var facilitySettings = GameSettings.Instance.GetSettings(action.Facility);

		if (tile.ExtractSoilDeposits())
		{
			Minerals += GameSettings.Instance.DepositReward.Get(tile.Layer);
		}

		var price = facilitySettings.MineralPrice.GetPrice(tile.Layer);
		Minerals -= price;
		BuildPoints -= facilitySettings.BuildPointsCost;
		EnergyCap += facilitySettings.EnergyContribution.EnergyCap;

		// change tileboard
		tiles[action.X, action.Y].SetFacility(action.Facility);
		ReachedDepth = Mathf.Max(action.Y, ReachedDepth);
		builtFacilities.Add(action.Facility);

		OnTurnOver?.Invoke(action);
		return true;
	}

	internal void OnFacilityAdded(Tile tile)
	{
		tilesWithFacilities.Add(tile);
	}

	internal void OnFacilityRemoved(Tile tile)
	{
		tilesWithFacilities.Remove(tile);
	}

	public int EnergyPerTurn()
	{
		int energyPerTurn = 0;
		foreach(var tile in tilesWithFacilities)
			energyPerTurn += tile.GetEnergyProduction();
		return energyPerTurn;
	}

	public int MineralsPerTurn()
	{
		int perTurn = 0;
		foreach(var tile in tilesWithFacilities)
			perTurn += tile.GetMineralProduction();
		return perTurn;
	}

	public bool ValidateAction(PlayerAction action)
	{
		if (action.Facility == FacilityType.None)
			return false;

		var facilitySettings = GameSettings.Instance.GetSettings(action.Facility);
		if (facilitySettings == null)
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

		if (Minerals < facilitySettings.MineralPrice.GetPrice(tile.Layer))
			return false;
		if (BuildPoints < facilitySettings.BuildPointsCost)
			return false;

		if (tile.GetFirstNeighbourTunnel() is Direction tunnelConnectionDirection)
		{
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
		foreach(var tile in tilesWithFacilities)
		{
			var settings = GameSettings.Instance.GetSettings(tile.FacilityType);
			if (settings == null)
				continue;

			Energy += settings.EnergyContribution.Get(tile.Layer);
			Minerals += settings.Production.MineralProduction.GetProduction(tile.Layer);
		}

		if (Energy > EnergyCap)
			Energy = EnergyCap;

		BuildPoints = BuildPointsCap;

		// todo: gain minerals

		CurrentTurn++;
	}
}

public struct PlayerAction
{
	public FacilityType Facility;
	public int X, Y;
}