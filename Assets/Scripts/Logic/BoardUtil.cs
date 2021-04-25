using UnityEngine;

public static class BoardUtil
{
	public static Vector3 TileCoordToPosition(this Vector2Int coord)
	{
		return new Vector3(coord.x, coord.y, 0);
	}
}