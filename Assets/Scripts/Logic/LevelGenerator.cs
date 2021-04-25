using System;
using System.Collections.Generic;
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
	public AnimationCurve BetterMineralProbabilityByWidth;
	public AnimationCurve GraniteProbabilityByHeight;
	public AnimationCurve MagmaProbabilityByHeight;

	public LevelData Generate()
	{
		var tiles = new TileType[Width, Height];

		var levelData = new LevelData()
		{
			Height = Height,
			Width = Width,
			Tiles = tiles,
			TerraformerTile = TerraformingFacilityInitialPosition,
			Parent = this
		};

		// do variants
		levelData.SoilVariants = GenerateSoilVariantsMatrix(levelData);

		// todo dummy
		for (int i = 0; i < levelData.Width; i++)
			for (int j = 0; j < levelData.Height; j++)
			{
				tiles[i, j] = TileType.Soil_A;
			}

		return levelData;
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
				while (true)
				{
					var variant = variantPool[UnityEngine.Random.Range(0, variantPool.Length)];
					if (i > 0 && variants[i - 1, j] == variant) continue;
					if (j > 0 && variants[i, j - 1] == variant) continue;
					variants[i, j] = variant;
					break;
				}
			}

		return variants;
	}
}