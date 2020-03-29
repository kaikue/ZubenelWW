using UnityEngine;
using UnityEngine.UI;

public class Portal : MonoBehaviour {

	public GameObject starRequiredPrefab;
	public int starsRequired;

	public GameObject starSprite;
	public GameObject numText;

	private void Start()
	{
		GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
		starSprite.GetComponent<SpriteRenderer>().color = gm.levelStarPrefab.GetComponent<Star>().GetColor();
		numText.GetComponent<Text>().text = "" + starsRequired;
	}

	public void TryActivate()
	{
		// TODO
	}
}
