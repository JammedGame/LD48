using System;
using TMPro;
using UnityEngine;

public class TileView : MonoBehaviour
{
	[SerializeField] private MeshRenderer meshRenderer;
	[SerializeField] private MeshRenderer facilityMeshRenderer;

	public Tile Tile { get; private set; }

	public static TileView CreateView(Tile tile, TileView tileViewPrefab)
	{
		var tileView = Instantiate(tileViewPrefab, tile.GetPosition3D(), Quaternion.identity);
		tileView.name = $"Tile[{tile.X}, {tile.Y}]";
		tileView.Tile = tile;
		tileView.UpdateMaterial();
		return tileView;
	}

	public void UpdateMaterial()
	{
		if (Tile.TileType == TileType.Undefined)
		{
			meshRenderer.enabled = false;
			facilityMeshRenderer.enabled = false;
			return;
		}

		// update main tile
		var tex = GetTextureForTile(Tile);
		meshRenderer.material.mainTexture = tex;
		meshRenderer.enabled = true;

		// update facility
		var facilityTexture = GetTextureForFacility(Tile);
		if (facilityTexture != null)
		{
			facilityMeshRenderer.material.mainTexture = facilityTexture;
			facilityMeshRenderer.enabled = true;
		}
		else
		{
			facilityMeshRenderer.enabled = false;
		}
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

	public static Texture GetTextureForFacility(Tile tile)
	{
		if (!tile.HasFacility)
			return null;

		if (tile.FacilityType == FacilityType.Tunnel)
		{
			return LoadTunnelTexture(tile);
		}

		//todo: other facilities.

		return null;
	}

	private static Texture LoadTunnelTexture(Tile tile)
	{
		// todo: tetris logic
		var upTileHasFacility = tile.GetAdjecentTile(Direction.Top) is Tile topTile && topTile.HasFacility;
		var downTileHasFacility = tile.GetAdjecentTile(Direction.Bottom) is Tile bottomTile && bottomTile.HasFacility;
		var leftTileHasFacility = tile.GetAdjecentTile(Direction.Left) is Tile leftTile && leftTile.HasFacility;
		var rightTileHasFacility = tile.GetAdjecentTile(Direction.Right) is Tile rightTile && rightTile.HasFacility;
		var tunnelName = GetTunnelName(leftTileHasFacility, rightTileHasFacility, upTileHasFacility, downTileHasFacility);
		return Resources.Load<Texture>($"Tiles/Tunnel_{tile.Layer}_{tunnelName}");
	}

	static string GetTunnelName(bool left, bool right, bool up, bool down)
	{
		// 4-way
		if (left && right && up && down) return "Cross";

		// 3-way
		if (left && right && down) return "T_0";
		if (right && down && up) return "T_1";
		if (left && right && up) return "T_2";
		if (left && down && up) return "T_3";

		// 2-way turn
		if (right && down) return "Turn_0";
		if (right && up) return "Turn_1";
		if (left && up) return "Turn_2";
		if (left && down) return "Turn_3";

		// 2-way
		if (left || right) return "Horizontal";
		if (up || down) return "Vertical";

		// default
		return "Vertical";
	}
}