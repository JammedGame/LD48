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
		if (!ValidateAction(action, out _))
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
		BuildPointsCap += facilitySettings.BuildPointsCapIncrease;
		BuildPoints += facilitySettings.BuildPointsCapIncrease;

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

	public bool ValidateAction(PlayerAction action, out string error)
	{
		error = default;

		if (action.Facility == FacilityType.None)
		{
			error = "Tried to place undefined facility!";
			return false;
		}

		var facilitySettings = GameSettings.Instance.GetSettings(action.Facility);
		if (facilitySettings == null)
		{
			error = "Tried to place undefined facility!";
			return false;
		}

		var tile = GetTile(action.X, action.Y);
		if (tile == null)
		{
			error = "Tried to place at undefined tile!";
			return false;
		}

		if (tile.TileType == TileType.Surface)
		{
			error = "Can't place facilities on the surface - lets dig!";
			return false;
		}

		if (!IsFacilityTypeUnlocked(action.Facility))
		{
			error = $"Build {facilitySettings.Requirements.RequirementToUnlock.GetSettings().Name} to unlock {facilitySettings.Name}!";
			return false;
		}

		if (tile.TileType == TileType.Core)
		{
			error = "We can't place facilities in the Core!";
			return false;
		}
		if (tile.TileType == TileType.Granite)
		{
			error = "We can't place facilities on Granite!";
			return false;
		}

		if (tile.TileType == TileType.Mineral && action.Facility != FacilityType.MineralExtractor)
		{
			error = "Only Mineral Extractors can be placed on Mineral Tiles!";
			return false;
		}

		if (tile.TileType != TileType.Mineral && action.Facility == FacilityType.MineralExtractor)
		{
			error = "Mineral Extractors must be placed on Mineral tiles!";
			return false;
		}

		if (tile.TileType == TileType.Magma && action.Facility != FacilityType.GeothermalPowerPlant)
		{
			error = "Only Geothermal Power Plants can be placed on Lava Tiles!";
			return false;
		}

		if (tile.TileType != TileType.Magma && action.Facility == FacilityType.GeothermalPowerPlant)
		{
			error = "Geothermal Power Plants must be placed on Lava tiles!";
			return false;
		}

		if (action.Facility == FacilityType.BFCoreExtractor)
		{
			var bottomIsCore = tile.GetAdjecentTile(Direction.Bottom) is Tile t && t.TileType == TileType.Core;
			if (!bottomIsCore)
			{
				error = "BFCore Extractor must be placed next to the Planet core! Dig deeper";
				return false;
			}
		}

		if (tile.HasFacility)
		{
			error = "Tile is not empty!";
			return false;
		}

		if (Minerals < facilitySettings.MineralPrice.GetPrice(tile.Layer))
		{
			error = $"WE NEED MORE MINERALS";
			return false;
		}

		if (BuildPoints < facilitySettings.BuildPointsCost)
		{
			error = $"WE NEED MORE BUILD BOTS!";
			return false;
		}

		var tunnelConnectionDirection = tile.GetFirstNeighbourTunnel();
		if (tunnelConnectionDirection == null)
		{
			error = $"CONSTRUCT TUNNELS TO THIS LOCATION FIRST!";
			return false;
		}

		error = default;
		return true;
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
		Energy += EnergyPerTurn();
		Minerals += MineralsPerTurn();

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