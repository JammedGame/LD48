using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Main UI controller.
/// </summary>
public class GameUIController : MonoBehaviour
{
	public TooltipUI Tooltip;
	public GameController GameController { get; private set; }
	public GameWorld GameWorld => GameController.ActiveGame;
	public GameUIComponent SelectedAction => selectedAction;
	GameUIComponent selectedAction;

	public void Initialize(GameController game)
	{
		GameController = game;
		Tooltip.ForceHideTooltip();
	}

	public void DeselectAction(GameUIComponent placerButton)
	{
		if (selectedAction == placerButton)
		{
			SelectAction(null);
		}
	}

	public void SelectAction(GameUIComponent placerButton)
	{
		if (selectedAction != placerButton)
		{
			selectedAction?.OnDeselect();
			selectedAction = placerButton;
			selectedAction?.OnSelect();
		}
	}
}
