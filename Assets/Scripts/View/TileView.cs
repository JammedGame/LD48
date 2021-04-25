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
		if (tile.TileType == TileType.Atmosphere)
		{
			var path0 = string.Format(TileType_Format, tile.TileType);
			return Resources.Load<Texture>(path0);
		}

		var path1 = string.Format(TileType_Layer_Variant_Format, tile.TileType, tile.Layer, tile.SoilVariant);
		Texture result = Resources.Load<Texture>(path1);
		if (result)
			return result;

		var path2 = string.Format(TileType_Layer_Format, tile.TileType, tile.Layer);
		Texture result2 = Resources.Load<Texture>(path1);
		if (result2)
			return result2;

		Debug.LogError($"Failed to find texture found for tile: {tile}");
		return null;
	}
}