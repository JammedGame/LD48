using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Main UI controller.
/// </summary>
public class GameUIController : MonoBehaviour
{
	public ActionPreview Preview;
	public TooltipUI Tooltip;
	public GameController GameController { get; private set; }
	public bool MouseIsOverMap { get; private set; }
	public GameWorld GameWorld => GameController.ActiveGame;
	public GameUIComponent SelectedAction => selectedAction;
	GameUIComponent selectedAction;

	public string MineralsFormat;
	public string EnergyFormat;
	public CalloutUI Callout;
	public Button NextTurnButton;
	public TextMeshProUGUI Minerals;
	public TextMeshProUGUI Energy;
	public TextMeshProUGUI Turn;
	public TextMeshProUGUI BuildPoints;

	private float lastMinerals = 0;
	private float lastEnergy = 0;
	private float lastEnergyCap = 0;
	private int lastTurn = 0;

	public void Initialize(GameController game)
	{
		GameController = game;
		Tooltip.ForceHideTooltip();
		NextTurnButton.onClick.AddListener(OnClick);
		Callout.Init();
	}

	private void OnClick()
	{
		GameWorld?.ProcessTurn();
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

		ShowActionPreview();

		string mineralsPerTurn = ChangeString(GameWorld.MineralsPerTurn());
		lastMinerals = Mathf.Lerp(lastMinerals, GameWorld.Minerals, Time.deltaTime * 5f);
		lastMinerals = Mathf.MoveTowards(lastMinerals, GameWorld.Minerals, Time.deltaTime * 10);

		Minerals.text = string.Format(MineralsFormat, Mathf.RoundToInt(lastMinerals)) + mineralsPerTurn;

		string energyPerTurnText = ChangeString(GameWorld.EnergyPerTurn());

		lastEnergy = Mathf.Lerp(lastEnergy, GameWorld.Energy, Time.deltaTime * 5f);
		lastEnergy = Mathf.MoveTowards(lastEnergy, GameWorld.Energy, Time.deltaTime * 10);

		lastEnergyCap = Mathf.Lerp(lastEnergyCap, GameWorld.EnergyCap, Time.deltaTime * 5f);
		lastEnergyCap = Mathf.MoveTowards(lastEnergyCap, GameWorld.EnergyCap, Time.deltaTime * 10);

		Energy.text = string.Format(EnergyFormat, Mathf.RoundToInt(lastEnergy), Mathf.RoundToInt(lastEnergyCap)) + energyPerTurnText;

		if (lastTurn != GameWorld.CurrentTurn)
		{
			lastTurn = GameWorld.CurrentTurn;
			Turn.text = $"TURN: {GameWorld.CurrentTurn}";
		}

		BuildPoints.text = $"BUILD BOTS: {GameWorld.BuildPoints} / {GameWorld.BuildPointsCap}";
	}

	private void ShowActionPreview()
	{
		if (Preview == null)
			return;

		if (selectedAction == null || !MouseIsOverMap)
		{
			Preview.Hide();
			return;
		}

		var tileCoord = BoardUtil.GetHoveredTile();
		var tile = GameWorld.GetTile(tileCoord.x, tileCoord.y);
		if (tile == null)
		{
			Preview.Hide();
			return;
		}

		var previewData = selectedAction.PreviewData(tile);
		Preview.Show(previewData);
	}

	public void OnMouseEnterMap()
	{
		MouseIsOverMap = true;
	}

	public void OnMouseLeaveMap()
	{
		MouseIsOverMap = false;
	}

	private static string ChangeString(int value)
	{
		if (value == 0) return "";
		return value > 0
			? $" (+{value})"
			: $" ({value})";
	}
}
