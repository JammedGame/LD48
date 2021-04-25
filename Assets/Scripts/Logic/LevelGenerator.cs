using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class LevelGenerator : ScriptableObject
{
	public int Width = 15;
	public int AtmosphereHeight = 5;
	public int Soil1Height = 12;
	public int Soil2Height = 8;
	public int Soil3Height = 5;
	public int CoreHeight = 5;
	public int AtmosphereEndsAt => AtmosphereHeight;
	public int Soil1EndsAt => AtmosphereHeight + Soil1Height;
	public int Soil2EndsAt => AtmosphereHeight + Soil1Height + Soil2Height;
	public int Soil3EndsAt => AtmosphereHeight + Soil1Height + Soil2Height + Soil3Height;
	public int Height => AtmosphereHeight + Soil1Height + Soil2Height + Soil3Height + CoreHeight;
	public Vector2Int TerraformingFacilityInitialPosition;
	public AnimationCurve MineralProbabilityByHeight;
	public AnimationCurve DepositProbabilityByHeight;
	public AnimationCurve GraniteProbabilityByHeight;
	public AnimationCurve MagmaProbabilityByHeight;
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

		levelData.SoilVariants = GenerateSoilVariantsMatrix(levelData);
		levelData.Tiles = GenerateTileTypeMatrix(levelData);
		return levelData;
	}

	private TileData[,] GenerateTileTypeMatrix(LevelData levelData)
	{
		var tiles = new TileData[Width, Height];

		// mineral, deposit, granite, magma, default
		for (int j = AtmosphereEndsAt; j < Soil3EndsAt; j++)
		{
			var heightProgress = (float)(j - AtmosphereEndsAt) / (Soil3EndsAt - AtmosphereEndsAt);
			var td = new List<TileData>();
			var mineralCount = (int)(Width * MineralProbabilityByHeight.Evaluate(heightProgress));
			td.AddRange(Enumerable.Repeat(GetMineralTileForHeight(j), mineralCount).ToList());
			var depositCount = (int)(Width * DepositProbabilityByHeight.Evaluate(heightProgress));
			td.AddRange(Enumerable.Repeat(GetDepositTileForHeight(j), depositCount).ToList());
			var graniteCount = (int)(Width * GraniteProbabilityByHeight.Evaluate(heightProgress));
			td.AddRange(Enumerable.Repeat(GetGraniteTileForHeight(j), graniteCount).ToList());
			var magmaCount = (int)(Width * MagmaProbabilityByHeight.Evaluate(heightProgress));
			td.AddRange(Enumerable.Repeat(GetMagmaTileForHeight(j), magmaCount).ToList());
			var defaultCount = Width - (mineralCount + depositCount + graniteCount + magmaCount);
			td.AddRange(Enumerable.Repeat(GetDefaultTileForHeight(j), defaultCount).ToList());
			td = RandomnessProvider.Shuffle(td);
			for (int i = 0; i < levelData.Width; i++)
			{
				tiles[i, j] = td[i];
			}
		}

		// todo jole: better mineral
		// for (int j = AtmosphereEndsAt; j < Soil3EndsAt; j++)
		// {
		// 	for (int i = 0; i < levelData.Width; i++)
		// 	{
		// 		if (tiles[i, j].TileType != TileType.Mineral) continue;

		// 		var widthProgress = 2.0 * (j - AtmosphereEndsAt) / Width;
		// 	}
		// }

		return tiles;
	}

	private TileData GetDefaultTileForHeight(int j)
	{
		if (j < AtmosphereEndsAt)
			return new TileData(TileType.Atmosphere, Layer.Atmosphere);
		if (j < Soil1EndsAt)
			return new TileData(TileType.Soil, Layer.A);
		if (j < Soil2EndsAt)
			return new TileData(TileType.Soil, Layer.B);

		return new TileData(TileType.Soil, Layer.C);
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

	private int[,] GenerateSoilVariantsMatrix(LevelData levelData)
	{
		var variants = new int[Width, Height];

		var variantPool = new int[]
		{
			0, 0, 0, 0,
			1, 1, 1, 1,
			2, 2, 2, 2,
			3
		};

		for (int i = 0; i < levelData.Width; i++)
			for (int j = 0; j < levelData.Height; j++)
			{
				for (int attempts = 0; attempts < 100; attempts++) //
				{
					var variant = variantPool[UnityEngine.Random.Range(0, variantPool.Length)];
					if (i > 0 && variants[i - 1, j] == variant) continue; // prevent repeating to the left
					if (j > 0 && variants[i, j - 1] == variant) continue; // prevent repeating to the up
					variants[i, j] = variant;
					break;
				}
			}

		return variants;
	}
}