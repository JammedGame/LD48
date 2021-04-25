using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacerButton : GameUIComponent,
	IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
	public void OnBeginDrag(PointerEventData eventData)
	{
		UIController.SelectAction(this);
	}

	public void OnDrag(PointerEventData eventData)
	{
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		UIController.DeselectAction(this);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		UIController.SelectAction(this);
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
}
