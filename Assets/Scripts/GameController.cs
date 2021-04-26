using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	// serialized
	public LevelGenerator LevelGenerator;
	public CameraController CameraController;
	public GameUIController UIController;
	public GameWorld ActiveGame => active;

	// runtime
	TileViewController viewController;
	GameWorld active;

	void Start()
    {
		var levelData = LevelGenerator.Generate();

		active = new GameWorld(levelData);
		viewController = new TileViewController(active);
		UIController.Initialize(this);
		CameraController.Initialize(levelData);
	}

    void Update()
    {
        if (active == null)
			return;

		CameraController.CameraUpdate();
	}
}