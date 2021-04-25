using UnityEngine;

public static class BoardUtil
{
	public static Vector3 TileCoordToPosition3D(int coordX, int coordY)
	{
		return new Vector3(coordX, -coordY, 0);
	}

	public static Vector3 TileCoordToPosition3D(this Vector2Int coord)
	{
		return new Vector3(coord.x, -coord.y, 0);
	}

	public static Vector3 GetPosition3D(this Tile tile)
	{
		return TileCoordToPosition3D(tile.X, tile.Y);
	}
}