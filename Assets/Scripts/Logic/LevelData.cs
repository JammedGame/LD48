using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
	public int Width = 15;
	public int SkyHeight = 5;
	public int Soil1Height = 12;
	public int Soil2Height = 8;
	public int Soil3Height = 5;
	public int CoreHeight = 5;
	public int SkyEndsAt => SkyHeight;
	public int Soil1EndsAt => SkyHeight + Soil1Height;
	public int Soil2EndsAt => SkyHeight + Soil1Height + Soil2Height;
	public int Soil3EndsAt => SkyHeight + Soil1Height + Soil2Height + Soil3Height;
	public int Height => SkyHeight + Soil1Height + Soil2Height + Soil3Height + CoreHeight;
	public Vector2Int TerraformingFacilityInitialPosition;
	public AnimationCurve MineralProbabilityByHeight;
	public AnimationCurve BetterMineralProbabilityByWidth;
	public AnimationCurve GraniteProbabilityByHeight;
	public AnimationCurve MagmaProbabilityByHeight;
	public TileType[] Tiles { get; private set; }

	public void Initialize()
	{
		Tiles = new TileType[Width * Height];
		Array.Clear(Tiles, 0, Width * Height);

		// todo jole: generate the level
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