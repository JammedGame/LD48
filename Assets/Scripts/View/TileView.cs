using TMPro;
using UnityEngine;

public class TileView : MonoBehaviour
{
	[SerializeField] private MeshRenderer meshRenderer;

	public Tile Tile { get; private set; }

	public static TileView CreateView(Tile tile, TileView tileViewPrefab)
	{
		var tileView = Instantiate(tileViewPrefab, tile.GetPosition3D(), Quaternion.identity);
		tileView.name = $"Tile[{tile.X}, {tile.Y}]";
		tileView.Tile = tile;
		tileView.UpdateMaterialAndRotation();
		return tileView;
	}

	public void UpdateMaterialAndRotation()
	{
		if (Tile.TileType == TileType.Undefined)
		{
			meshRenderer.enabled = false;
			return;
		}

		var tex = GetTextureForTile(Tile);
		meshRenderer.material.mainTexture = tex;
		meshRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, 0);
		meshRenderer.enabled = true;
	}

	const string TileType_Format = "Tiles/{0}";
	const string TileType_Layer_Format = "Tiles/{0}_{1}";
	const string TileType_Layer_Variant_Format = "Tiles/{0}_{1}_{2}";

	public static Texture GetTextureForTile(Tile tile)
	{
		Texture result = null;

		// check transition
		var bottomTile = tile.GetAdjecentTile(Direction.Bottom);
		if (bottomTile != null && bottomTile.Layer != tile.Layer)
		{
			result = Resources.Load<Texture>($"Tiles/{tile.TileType}_{tile.Layer}{bottomTile.Layer}_{tile.SoilVariant}");
			if (result)
				return result;

			result = Resources.Load<Texture>($"Tiles/{tile.TileType}_{tile.Layer}{bottomTile.Layer}_0");
			if (result)
				return result;
		}

		// check layer and variant
		result = Resources.Load<Texture>($"Tiles/{tile.TileType}_{tile.Layer}_{tile.SoilVariant}");
		if (result)
			return result;

		// check layer only
		result = Resources.Load<Texture>($"Tiles/{tile.TileType}_{tile.Layer}_0");
		if (result)
			return result;

		result = Resources.Load<Texture>($"Tiles/{tile.TileType}");
		if (result)
			return result;

		Debug.LogError($"Failed to find texture found for tile: {tile}");
		return null;
	}
}