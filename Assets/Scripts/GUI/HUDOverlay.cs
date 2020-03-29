using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUDOverlay : MonoBehaviour
{
	public GameObject canvas;
	public GameObject content;

	private const float SLIDE_TIME = 0.5f;
	private const float HEIGHT_OFFSCREEN = 250;
	private const float HOLD_TIME = 3.0f;

	private RectTransform contentRect;
	private float contentGoalY;

	private bool showing = true;

	private void Start()
	{
		contentRect = content.GetComponent<RectTransform>();
		contentGoalY = contentRect.anchoredPosition.y;
	}

	public void SetStars(Color starColor, int starCount)
	{
		content.GetComponentInChildren<Image>().color = starColor;
		content.GetComponentInChildren<Text>().text = "" + starCount;
	}

	public void Hold()
	{
		showing = true;
		StartCoroutine(HoldContents());
	}

	public IEnumerator HoldContents()
	{
		yield return new WaitForSeconds(HOLD_TIME);
		StartCoroutine(SlideContents(false));
	}

	public void SlideIn()
	{
		if (!showing)
		{
			showing = true;
			StartCoroutine(SlideContents(true));
		}
	}

	private IEnumerator SlideContents(bool isIn)
	{
		for (float t = 0; t < SLIDE_TIME; t += Time.deltaTime)
		{
			float newY = GetInY(contentGoalY, t, isIn);
			contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, newY);
			yield return new WaitForEndOfFrame();
		}
		if (isIn)
		{
			Hold();
		}
		else
		{
			showing = false;
		}
	}

	private float GetInY(float goalY, float t, bool isIn)
	{
		float tScale = t / SLIDE_TIME;
		if (isIn)
		{
			tScale = 1 - tScale; //reverse the numbers
		}
		float oneMinusTScale = 1 - tScale;

		//return Mathf.Pow(timeRemaining, 2) * goalY + HEIGHT_OFFSCREEN; //TODO: quadratic slide
		return tScale * HEIGHT_OFFSCREEN + oneMinusTScale * goalY;
	}

}
