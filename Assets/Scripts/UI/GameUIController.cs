using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

	public string MineralsFormat;
	public string EnergyFormat;
	public TextMeshProUGUI Minerals;
	public TextMeshProUGUI Energy;

	private float lastMinerals = 0;
	private float lastEnergy = 0;
	private float lastEnergyCap = 0;

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

	public void Update()
	{
		if (GameWorld == null)
			return;

		if (lastMinerals != GameWorld.Minerals)
		{
			lastMinerals = Mathf.Lerp(lastMinerals, GameWorld.Minerals, Time.deltaTime * 5f);
			lastMinerals = Mathf.MoveTowards(lastMinerals, GameWorld.Minerals, Time.deltaTime * 10);

			Minerals.text = string.Format(MineralsFormat, Mathf.RoundToInt(lastMinerals));
		}

		if (lastEnergy != GameWorld.Energy || lastEnergyCap != GameWorld.EnergyCap)
		{
			lastEnergy = Mathf.Lerp(lastEnergy, GameWorld.Energy, Time.deltaTime * 5f);
			lastEnergy = Mathf.MoveTowards(lastEnergy, GameWorld.Energy, Time.deltaTime * 10);

			lastEnergyCap = Mathf.Lerp(lastEnergyCap, GameWorld.EnergyCap, Time.deltaTime * 5f);
			lastEnergyCap = Mathf.MoveTowards(lastEnergyCap, GameWorld.EnergyCap, Time.deltaTime * 10);

			Energy.text = string.Format(EnergyFormat, Mathf.RoundToInt(lastEnergy), Mathf.RoundToInt(lastEnergyCap));
		}
	}
}
