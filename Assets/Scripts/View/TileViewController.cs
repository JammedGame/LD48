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