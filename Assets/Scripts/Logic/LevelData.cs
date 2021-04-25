using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generated level data.
/// </summary>
[System.Serializable]
public class LevelData
{
	public int Width;
	public int Height;
	public TileData[,] Tiles;
	public int[,] SoilVariants;
	public Vector2Int TerraformerTile;
	public LevelGenerator Parent;
}