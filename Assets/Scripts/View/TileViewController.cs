using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileViewController
{
	public readonly GameWorld GameWorld;
	public readonly TileView[,] TileViews;

	public TileViewController(GameWorld gameWorld)
	{
		this.GameWorld = gameWorld;
		this.TileViews = SpawnTileViews(gameWorld);
		this.GameWorld.OnTurnOver += OnUpdate;
	}

	public void OnUpdate(PlayerAction action)
	{
		GetTileView(action.X - 1, action.Y)?.UpdateMaterial();
		GetTileView(action.X + 1, action.Y)?.UpdateMaterial();
		GetTileView(action.X, action.Y - 1)?.UpdateMaterial();
		GetTileView(action.X, action.Y + 1)?.UpdateMaterial();
		GetTileView(action.X, action.Y)?.UpdateMaterial();
	}

	public TileView GetTileView(int x, int y)
	{
		if (GameWorld.GetTile(x, y) == null) return null;
		return TileViews[x, y];
	}

	private static TileView[,] SpawnTileViews(GameWorld gameWorld)
	{
		// init tile views.
		var width = gameWorld.LevelData.Width;
		var height = gameWorld.LevelData.Height;
		var tileViews = new TileView[width, height];
		var tileViewPrefab = Resources.Load<TileView>("Prefabs/Tile");
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				var tile = gameWorld.GetTile(i, j);
				tileViews[i, j] = TileView.CreateView(tile, tileViewPrefab);
			}

		return tileViews;
	}
}