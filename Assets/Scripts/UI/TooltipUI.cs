using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipUI : GameUIComponent
{
	public TextMeshProUGUI TooltipText;
	public RectTransform TooltipPivot;
	Vector3[] worldCorners = new Vector3[4];
	int currentToken;

	public void ShowTooltip(RectTransform target, Direction direction, string text, out int showToken)
	{
		var worldPos = GetWorldPos(target, direction);
		TooltipPivot.position = worldPos;
		TooltipText.text = text;
		gameObject.SetActive(true);
		currentToken++;
		showToken = currentToken;
	}

	public void ForceHideTooltip()
    {
        gameObject.SetActive(false);
    }
	public void HideTooltip(int token)
    {
        if (currentToken == token)
		    gameObject.SetActive(false);
    }

	public Vector3 GetWorldPos(RectTransform target, Direction direction)
    {
		var rect = target.rect;
		target.GetWorldCorners(worldCorners);

		switch(direction)
        {
            case Direction.Left:
				return (worldCorners[0] + worldCorners[1]) / 2;
            case Direction.Right:
				return target.localToWorldMatrix * new Vector3(rect.width, rect.height / 2, 0);
            case Direction.Top:
				return target.localToWorldMatrix * new Vector3(rect.width / 2, 0, 0);
            case Direction.Bottom:
				return target.localToWorldMatrix * new Vector3(rect.width / 2, rect.height, 0);
        }

		throw new System.Exception("wrong dir");
	}
}
