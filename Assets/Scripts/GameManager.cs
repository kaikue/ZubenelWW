using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public GameObject pauseOverlayPrefab;
	public GameObject starCollectOverlayPrefab;
	public GameObject hudOverlayPrefab;
	public GameObject loadingOverlayPrefab;

	public GameObject levelStarPrefab;

	public AudioClip pauseSound;
	public AudioClip unpauseSound;
	
	[HideInInspector]
	public bool paused = false;
	private bool overlayActive = false;

	public const string SAVE_FILE = ".save";
	private string savePath;

	private GameObject pauseOverlay;
	private HUDOverlay hudOverlay;
	private Star levelStar;
	private AudioSource audioSrc;

	[HideInInspector]
	public int[] starsCollected;
	private List<string> starCollectedNames;
	private List<string> doorsOpenedNames;

	public static string GetSavePath()
	{
		return Path.Combine(Application.persistentDataPath, SAVE_FILE);
	}

	private void Awake()
	{
		savePath = GetSavePath();
		GameObject hudObject = Instantiate(hudOverlayPrefab);
		hudOverlay = hudObject.GetComponent<HUDOverlay>();
		audioSrc = GetComponent<AudioSource>();
		LoadSave();
	}

	private void LoadSave()
	{
		int numTypes = Enum.GetValues(typeof(Star.StarType)).Length;
		starsCollected = new int[numTypes];
		starCollectedNames = new List<string>();
		doorsOpenedNames = new List<string>();

		string[] lines = ReadSave();
		for (int l = 1; l < lines.Length; l++) //skip first line (level index)
		{
			string line = lines[l];
			string[] split = line.Split('|');
			if (split[0] == "S")
			{
				starCollectedNames.Add(split[1]);
				int type = int.Parse(split[2]);
				int num = int.Parse(split[3]);
				starsCollected[type] += num;
			}
			else if (split[0] == "D")
			{
				doorsOpenedNames.Add(split[1]);
			}
		}

		lines[0] = SceneManager.GetActiveScene().name;
		File.WriteAllLines(savePath, lines);
	}

	private string[] ReadSave()
	{
		if (File.Exists(savePath))
		{
			return File.ReadAllLines(savePath);
		}
		else
		{
			return new string[1];
		}
	}

	private void AppendToSave(string line)
	{
		File.AppendAllText(savePath, line + Environment.NewLine);
	}

	private void Start()
	{
		Utils.SetVolume();
		SetTimeScale();
		SetHUDLevelStar();
		hudOverlay.Hold();
	}
	
	private void SetHUDLevelStar()
	{
		levelStar = levelStarPrefab.GetComponent<Star>();
		hudOverlay.SetStars(levelStar.GetColor(), starsCollected[(int)levelStar.starType]);
	}

	public void ShowHUDPortalStars(Portal portal)
	{
		GameObject portalStar = portal.starRequiredPrefab;
		Star star = portalStar.GetComponent<Star>();
		Color starColor = star.GetColor();
		int starCount = starsCollected[(int)star.starType];
		hudOverlay.SetStars(starColor, starCount);
		hudOverlay.SlideIn();
	}

	private void Update()
	{
		if (Input.GetButtonDown("Pause"))
		{
			TogglePauseMenu();
		}
	}

	private void TogglePause()
	{
		paused = !paused;
		if (paused)
		{
			Time.timeScale = 0;
		}
		else
		{
			SetTimeScale();
		}
	}

	public void TogglePauseMenu()
	{
		if (overlayActive) return;

		TogglePause();
		if (paused)
		{
			pauseOverlay = Instantiate(pauseOverlayPrefab);
			PlaySound(pauseSound);
		}
		else
		{
			Destroy(pauseOverlay);
			PlaySound(unpauseSound);
		}
	}

	public void LoadScene(int sceneIndex)
	{
		Instantiate(loadingOverlayPrefab);
		SceneManager.LoadScene(sceneIndex);
	}

	public void QuitToTitle()
	{
		TogglePauseMenu();
		LoadScene(1);
	}

	public bool WasStarCollected(Star star)
	{
		return starCollectedNames.Contains(star.starText);
	}

	public bool CollectStar(Star star)
	{
		if (!star.WasCollected())
		{
			star.Collect();
			AppendToSave("S|" + star.starText + "|" + ((int)star.starType) + "|" + star.starValue);
			starCollectedNames.Add(star.starText);
			TogglePause();
			overlayActive = true;
			GameObject o = Instantiate(starCollectOverlayPrefab);
			o.GetComponent<StarCollectOverlay>().SetStarName(star.starText);
			starsCollected[(int)star.starType] += star.starValue;
			SetHUDLevelStar();
			hudOverlay.SlideIn();
			return true;
		}
		return false;
	}

	public void FinishOverlay()
	{
		TogglePause();
		overlayActive = false;
	}

	public void SaveDoor(string doorName)
	{
		doorsOpenedNames.Add(doorName);
		AppendToSave("D|" + doorName);
	}

	public bool WasDoorOpened(string doorName)
	{
		return doorsOpenedNames.Contains(doorName);
	}

	public void SetTimeScale()
	{
		float speed = PlayerPrefs.GetFloat(Options.KEY_GAME_SPEED, 1);
		Time.timeScale = speed;
		Time.fixedDeltaTime = Time.fixedUnscaledDeltaTime * speed;
	}

	private void PlaySound(AudioClip sound)
	{
		audioSrc.PlayOneShot(sound);
	}
}
