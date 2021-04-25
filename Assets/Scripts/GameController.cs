using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	// serialized
	public LevelGenerator LevelGenerator;
	public CameraController CameraController;

	// runtime
	TileViewController viewController;
	GameWorld active;

	void Start()
    {
		var levelData = LevelGenerator.Generate();

		active = new GameWorld(levelData);
		viewController = new TileViewController(active);
		CameraController.Initialize(levelData);
	}

    void Update()
    {
        if (active == null)
			return;

		CameraController.CameraUpdate();
	}
}