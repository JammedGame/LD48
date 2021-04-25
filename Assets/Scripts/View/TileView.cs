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

	public static Texture GetTextureForTile(Tile tile)
	{
		switch(tile.TileType)
		{
			case TileType soil when soil.IsSoil():
				return tile.TileType.LoadSoilTexture(tile.SoilVariant);

			default:
				return tile.TileType.LoadTexture();
		}
	}
}