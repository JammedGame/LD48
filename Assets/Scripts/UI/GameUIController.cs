﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main UI controller.
/// </summary>
public class GameUIController : MonoBehaviour
{
	public GameWorld GameWorld => FindObjectOfType<GameController>().ActiveGame;
	public GameUIComponent SelectedAction => selectedAction;
	GameUIComponent selectedAction;

	internal void DeselectAction(GameUIComponent placerButton)
	{
		if (selectedAction == placerButton)
		{
			SelectAction(null);
		}
	}

	internal void SelectAction(GameUIComponent placerButton)
	{
		if (selectedAction != placerButton)
		{
			selectedAction?.OnDeselect();
			selectedAction = placerButton;
			selectedAction?.OnSelect();
		}
	}
}
