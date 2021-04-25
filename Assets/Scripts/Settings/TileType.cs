using UnityEngine;

public enum TileType
{
	Undefined = 0,
}

public static class TileTypeExtensions
{
	public static (Texture tex, Texture overlay) LoadTexture(this TileType type)
	{
		return GameSettings.Instance.GetTexture(type);
	}
}