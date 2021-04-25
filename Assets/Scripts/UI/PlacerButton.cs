using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacerButton : GameUIComponent,
	IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
	public FacilityType Facility;

	public void OnBeginDrag(PointerEventData eventData)
	{
		UIController.SelectAction(this);
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
		UIController.GameWorld.ProcessAction(action);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		UIController.SelectAction(this);
	}

	public override void OnPointerDown(PointerEventData eventData)
    {
		PlaceFacility();
	}
}

public class GameUIComponent : UIBehaviour
{
	public GameUIController UIController => GetComponentInParent<GameUIController>();
	public bool IsSelected => UIController.SelectedAction == this;

    public virtual void OnSelect()
    {
    }

    public virtual void OnDeselect()
    {
    }

	public virtual void OnPointerDown(PointerEventData eventData)
	{
	}
}