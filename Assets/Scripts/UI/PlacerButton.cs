using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlacerButton : GameUIComponent,
	IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler,
	IPointerEnterHandler, IPointerExitHandler
{
	public Image BG;
	public Color BGColor;
	public Color BGColorFlash;
	public TextMeshProUGUI Text;
	public FacilityType Facility { get; private set; }
	public FacilitySettings Settings { get; private set; }
	public override ActionPreview.MyData PreviewData(Tile tile) =>
		new ActionPreview.MyData(
			tile.GetPosition3D(),
			TileView.GetTextureForFacility(tile, Facility),
			UIController.GameWorld.ValidateAction(new PlayerAction{Facility = Facility, X = tile.X, Y = tile.Y}, out var _)
		);
	int tooltipShowToken;

	public void Init(FacilityType facility)
	{
		Facility = facility;
		Settings = GameSettings.Instance.GetSettings(facility);
		Text.text = Settings.Name;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		UIController.SelectAction(this);
		UIController.Tooltip.ForceHideTooltip();
	}

	public void OnDrag(PointerEventData eventData)
	{
	}

	public void OnEndDrag(PointerEventData eventData)
	{
        if (eventData.hovered != null && eventData.hovered.Count > 0 && !eventData.hovered[0].GetComponent<CameraController>())
        {
			Debug.Log("Drag ended over UI");
			return;
        }

		PlaceFacility();
		UIController.DeselectAction(this);
	}

    public void PlaceFacility()
    {
		var tileCoord = BoardUtil.GetHoveredTile();
		var action = new PlayerAction()
		{
			Facility = Facility,
			X = tileCoord.x,
			Y = tileCoord.y
		};

		if (UIController.GameWorld.ValidateAction(action, out string error))
		{
			UIController.GameWorld.ProcessAction(action);
		}
		else if (!string.IsNullOrWhiteSpace(error))
		{
			UIController.Callout.Show(error.ToUpper());
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		UIController.SelectAction(this);
	}

	public override void ExecuteAction(PointerEventData eventData)
    {
		PlaceFacility();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ShowTooltip();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		UIController.Tooltip.HideTooltip(tooltipShowToken);
	}

	private void ShowTooltip()
	{
		var tooltipText = CreateTooltipText();
		UIController.Tooltip.ShowTooltip(rectTransform, Direction.Left, tooltipText, out tooltipShowToken);
	}

	public string CreateTooltipText()
	{
		var isLocked = !UIController.GameWorld.IsFacilityTypeUnlocked(Facility);
		var text = $"Build {Settings.Name}\nCosts {Settings.MineralPrice} minerals\n{Settings.EnergyContribution}\n\n{Settings.Description}";
		if (isLocked)
			text += $"\n\nBuild {Settings.Requirements.RequirementToUnlock.GetName()} to unlock this facility";

		return text;
	}

	public override void OnSelect()
	{
		base.OnSelect();
		BG.color = BGColorFlash;
	}

	public override void OnDeselect()
	{
		StopAllCoroutines();
		BG.color = BGColor;
	}
}

public class GameUIComponent : UIBehaviour
{
	public RectTransform rectTransform => transform as RectTransform;
	public GameUIController UIController => GetComponentInParent<GameUIController>();
	public bool IsSelected => UIController.SelectedAction == this;
	public virtual ActionPreview.MyData PreviewData(Tile tile) => null;

    public virtual void OnSelect()
    {
    }

    public virtual void OnDeselect()
    {
    }

	public virtual void ExecuteAction(PointerEventData eventData)
	{
	}
}