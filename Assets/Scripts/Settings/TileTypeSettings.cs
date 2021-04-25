using System;
using UnityEngine;

[Serializable]
public class TileTypeSettings
{
	public TileType TileType;
}

public enum Direction
{
	Undefined = 0,
	Left = 1,
	Right = 2,
	Top = 3,
	Bottom = 4
}