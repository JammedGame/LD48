using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalloutUI : MonoBehaviour
{
	public TextMeshProUGUI label;
	public Image b1;
	public Image b2;


	public AnimationCurve Shake;
	public AnimationCurve Alpha;

	public void Init()
    {
		gameObject.SetActive(false);
	}

    public void Show(string text)
    {
		gameObject.SetActive(true);
		label.text = text;

		StopAllCoroutines();
		StartCoroutine(OnShow(text));
	}

	private IEnumerator OnShow(string text)
	{
		float timer = 0f;
		while(timer < 3f)
		{
			var pos = transform.localPosition;
			pos.x = Shake.Evaluate(timer);
			transform.localPosition = pos;

			SetAlpha(Alpha.Evaluate(timer));

			timer += Time.deltaTime;
            yield return null;
		}

		gameObject.SetActive(false);
	}

	private void SetAlpha(float v)
	{
		label.color = label.color.WithAlpha(v);
		b2.color = b2.color.WithAlpha(v);
		b1.color = b1.color.WithAlpha(v);
	}
}

public static class ColorUtil
{
    public static Color WithAlpha(this Color c, float alpha)
    {
		return new Color(c.r, c.g, c.b, alpha);
	}
}
