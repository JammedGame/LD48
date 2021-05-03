using UnityEditor;
using UnityEngine;

public static class Cheats
{
	[MenuItem("Game/GiveMeEverything")]
	public static void GiveMeEverything()
	{
		GameObject.FindObjectOfType<GameController>()?.ActiveGame?.Cheat();
	}
}