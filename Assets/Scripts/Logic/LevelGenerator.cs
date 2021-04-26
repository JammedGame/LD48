using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class LevelGenerator : ScriptableObject
{
	public int Width = 15;
	public int CloudsLimit = 3;
	public int AtmosphereHeight = 5;
	public int Soil1Height = 12;
	public int Soil2Height = 8;
	public int Soil3Height = 5;
	public int CoreAHeight = 1;
	public int CoreBHeight = 1;
	public int AtmosphereEndsAt => AtmosphereHeight;
	public int Soil1EndsAt => AtmosphereHeight + Soil1Height;
	public int Soil2EndsAt => AtmosphereHeight + Soil1Height + Soil2Height;
	public int Soil3EndsAt => AtmosphereHeight + Soil1Height + Soil2Height + Soil3Height;
	public int CoreAEndsAt => AtmosphereHeight + Soil1Height + Soil2Height + Soil3Height + CoreAHeight;
	public int Height => AtmosphereHeight + Soil1Height + Soil2Height + Soil3Height + CoreAHeight + CoreBHeight;
	public Vector2Int TerraformingFacilityInitialPosition;
	public AnimationCurve MineralProbabilityByHeight;
	public AnimationCurve DepositProbabilityByHeight;
	public AnimationCurve GraniteProbabilityByHeight;
	public AnimationCurve MagmaProbabilityByHeight;

	public bool BetterMineralsOn;
	public AnimationCurve BetterMineralProbabilityByWidth;

	public LevelData Generate()
	{
		var levelData = new LevelData
		{
			Height = Height,
			Width = Width,
			TerraformerTile = TerraformingFacilityInitialPosition,
			Parent = this,
		};

		levelData.Tiles = GenerateTileTypeMatrix();
		levelData.SoilVariants = GenerateSoilVariantsMatrix(levelData.Tiles);
		return levelData;
	}

	private TileData[,] GenerateTileTypeMatrix()
	{
		var tiles = new TileData[Width, Height];

		// atmosphere
		for (int j = 0; j < AtmosphereEndsAt; j++)
		{
			for (int i = 0; i < Width; i++)
			{
				tiles[i, j] = GetDefaultTileForHeight(j);
			}
		}

		// mineral, deposit, granite, magma, default
		for (int j = AtmosphereEndsAt; j < Soil3EndsAt; j++)
		{
			var y = (float)(j - AtmosphereEndsAt) / (Soil3EndsAt - AtmosphereEndsAt);
			var td = new List<TileData>();
			var mineralCount = (int)(Width * MineralProbabilityByHeight.Evaluate(y));
			td.AddRange(Enumerable.Repeat(GetMineralTileForHeight(j), mineralCount).ToList());
			var depositCount = (int)(Width * DepositProbabilityByHeight.Evaluate(y));
			td.AddRange(Enumerable.Repeat(GetDepositTileForHeight(j), depositCount).ToList());
			var graniteCount = (int)(Width * GraniteProbabilityByHeight.Evaluate(y));
			td.AddRange(Enumerable.Repeat(GetGraniteTileForHeight(j), graniteCount).ToList());
			var magmaCount = (int)(Width * MagmaProbabilityByHeight.Evaluate(y));
			td.AddRange(Enumerable.Repeat(GetMagmaTileForHeight(j), magmaCount).ToList());
			var defaultCount = Width - (mineralCount + depositCount + graniteCount + magmaCount);
			td.AddRange(Enumerable.Repeat(GetDefaultTileForHeight(j), defaultCount).ToList());
			td = RandomnessProvider.Shuffle(td);
			for (int i = 0; i < Width; i++)
			{
				tiles[i, j] = td[i];
			}
		}

		// better mineral
		if (BetterMineralsOn)
			for (int j = AtmosphereEndsAt; j < Soil3EndsAt; j++)
			{
				for (int i = 0; i < Width; i++)
				{
					if (tiles[i, j].TileType != TileType.Mineral) continue;

					if (tiles[i, j].Layer == Layer.C) continue;

					var x = 2f * i / (Width - 2) - 1;
					var betterMineralProbability = BetterMineralProbabilityByWidth.Evaluate(x);
					if (RandomnessProvider.GetFloat() > betterMineralProbability) continue;

					tiles[i, j].Layer++;
				}
			}

		// core
		for (int j = Soil3EndsAt; j < Height; j++)
		{
			for (int i = 0; i < Width; i++)
			{
				tiles[i, j] = GetDefaultTileForHeight(j);
			}
		}

		// place lander
		var startPoint = TerraformingFacilityInitialPosition;
		tiles[startPoint.x, startPoint.y] = tiles[startPoint.x, startPoint.y]
			.WithFacility(FacilityType.TerraformingFacility, TileType.Surface);
		tiles[startPoint.x, startPoint.y + 1] = tiles[startPoint.x, startPoint.y + 1]
			.WithFacility(FacilityType.Tunnel, TileType.Soil);

		return tiles;
	}

	private TileData GetDefaultTileForHeight(int j)
	{
		if (j < AtmosphereEndsAt - 1)
			return new TileData(TileType.Surface, Layer.B);
		if (j < AtmosphereEndsAt)
			return new TileData(TileType.Surface, Layer.A);
		if (j < Soil1EndsAt)
			return new TileData(TileType.Soil, Layer.A);
		if (j < Soil2EndsAt)
			return new TileData(TileType.Soil, Layer.B);
		if (j < Soil3EndsAt)
			return new TileData(TileType.Soil, Layer.C);
		if (j < CoreAEndsAt)
			return new TileData(TileType.Core, Layer.A);

		return new TileData(TileType.Core, Layer.B);
	}

	private TileData GetMineralTileForHeight(int j)
	{
		if (j < Soil1EndsAt)
			return new TileData(TileType.Mineral, Layer.A);
		if (j < Soil2EndsAt)
			return new TileData(TileType.Mineral, Layer.B);

		return new TileData(TileType.Mineral, Layer.C);
	}

	private TileData GetDepositTileForHeight(int j)
	{
		if (j < Soil1EndsAt)
			return new TileData(TileType.Deposit, Layer.A);
		if (j < Soil2EndsAt)
			return new TileData(TileType.Deposit, Layer.B);

		return new TileData(TileType.Deposit, Layer.C);
	}

	private TileData GetGraniteTileForHeight(int j)
	{
		if (j < Soil1EndsAt)
			return new TileData(TileType.Granite, Layer.A);
		if (j < Soil2EndsAt)
			return new TileData(TileType.Granite, Layer.B);

		return new TileData(TileType.Granite, Layer.C);
	}

	private TileData GetMagmaTileForHeight(int j)
	{
		if (j < Soil1EndsAt)
			return new TileData(TileType.Magma, Layer.A);
		if (j < Soil2EndsAt)
			return new TileData(TileType.Magma, Layer.B);

		return new TileData(TileType.Magma, Layer.C);
	}

	private int[,] GenerateSoilVariantsMatrix(TileData[,] tiles)
	{
		var variants = new int[Width, Height];

		var variantPoolSoil = new int[]
		{
			0, 0, 0, 0,
			1, 1, 1, 1,
			2, 2, 2, 2,
			3
		};

		var variantPoolClouds = new int[]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0,
			1,
			2
		};

		var variantPoolSky = new int[]
		{
			0
		};

		var variantPoolSurface = new int[]
		{
			0, 1
		};

		var variantPoolCore = new int[]
		{
			0, 1
		};

		for (int i = 0; i < Width; i++)
			for (int j = 0; j < Height; j++)
			{
				var tileData = tiles[i, j];

				// pick a deck of variants.
				int[] variantPool = variantPoolSoil;
				if (tileData.TileType == TileType.Surface && tileData.Layer == Layer.A)
				{
					variantPool = variantPoolSurface;
				}
				else if (tileData.TileType == TileType.Surface && j < CloudsLimit)
				{
					variantPool = variantPoolClouds;
				}
				else if (tileData.TileType == TileType.Surface)
				{
					variantPool = variantPoolSky;
				}
				else if (tileData.TileType == TileType.Core)
				{
					variantPool = variantPoolCore;
				}

				for (int attempts = 0; attempts < 100; attempts++) // sanity check
				{
					// choose index at random from the deck
					var variantIndex = UnityEngine.Random.Range(0, variantPool.Length);
					var variant = variantPool[variantIndex];

					// prevent too much repeating if deck is large enough
					if (variantPool == variantPoolSoil)
					{
						if (i > 0 && variants[i - 1, j] == variant) continue; // prevent repeating to the left
						if (j > 0 && variants[i, j - 1] == variant) continue; // prevent repeating to the up
					}

					variants[i, j] = variant;
					break;
				}
			}

		return variants;
	}
}