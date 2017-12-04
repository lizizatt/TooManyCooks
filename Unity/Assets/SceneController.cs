using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;
using System.Collections.Generic;


public class SceneController : MonoBehaviour {

	public bool godMode = true;

	public int maxSpawnedCooks = 400;

	public float spawnWallXVal = 5;
	public float spawnWallHeight = 5;
	public GameObject cookPrefab = null;
	public GameObject[] cookSpawnPoints = null;

	public GameObject startupDoodads = null;

	public GameObject blackMask = null;
	public float fadeLength = 2.0f;

	public GameObject gunUI = null;

	public GameObject finalScoreObject = null;
	public GameObject tryAgainButton = null;

	public GameObject bigCook1 = null;
	public GameObject bigCook2 = null;

	public GameObject textObject = null;

	public Light sceneLight = null;
	public ParticleSystem mistParticleSystem = null;
	public float mistInKillCount = 10;
	public float mistMaxKillCount = 300;
	public float mistRateMax = 500;

	public int startingNumberOfCooks = 1;
	public int newCooksPerDeath = 1;
	public float startingCookSpeed = 5.0f;
	public float cookSpeedGainPerDeath = .01f;

	private float fadeOutStart = -1.0f;
	private float fadeInStart = -1.0f;

	private int numberOfCooks;
	private float cookSpeed;

	private bool playing = false;

	private Mutex cookMutex = new Mutex();

	private int cooksToSpawn = 0;
	private List<GameObject> cooks = new List<GameObject>();

	private static SceneController _instance = null;
	public static SceneController Instance
	{
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<SceneController>();
			}
			return _instance;
		}
	}

	public int GetNumCooks()
	{
		return numberOfCooks;
	}

	public void PlayerDied()
	{
		Debug.Log ("Player died");
		fadeOutStart = Time.time;
		playing = false;
		blackMask.SetActive (true);
	}

	//called every frame, if fadestart was set then it runs fade and eventually times out the scene, fading to black and showing the final score
	void RunFade()
	{
		if (fadeOutStart > 0) {
			SpriteRenderer sr = blackMask.GetComponent<SpriteRenderer> ();
			Color c = sr.color;
			c.a = Mathf.Min (Mathf.Max (0.0f, (Time.time - fadeOutStart) / fadeLength), 1.0f);
			sr.color = c;

			if (c.a >= 1.0f) {
				Debug.Log ("Faded out");
				//at this point players are put in to a screen showing final score & restart button
				fadeOutStart = -1.0f;
				finalScoreObject.SetActive (true);
				gunUI.SetActive (false);
				finalScoreObject.GetComponent<Text> ().text = "Final Score: " + numberOfCooks + (numberOfCooks > 1? " cooks" : " cook");
				textObject.SetActive (false);
				tryAgainButton.SetActive (true);

				for (int i = 10; i < cooks.Count; i++) {
					Destroy (cooks [i]);
				}
				if (cooks.Count >= 10) {
					cooks.RemoveRange(10, cooks.Count - 10);
				}

				HighScores.Instance.gameObject.SetActive (true);
				if (HighScores.Instance.IsNewHighScore (numberOfCooks)) {
					HighScores.Instance.ToggleNewScoreEntry ();
				}
			}
		}

		if (fadeInStart > 0) {
			SpriteRenderer sr = blackMask.GetComponent<SpriteRenderer> ();
			Color c = sr.color;
			c.a = Mathf.Min (Mathf.Max (0.0f, 1.0f - (Time.time - fadeInStart) / fadeLength), 1.0f);
			sr.color = c;

			if (c.a <= 0.0f) {
				Debug.Log ("Faded in");
				fadeInStart = -1.0f;
				blackMask.SetActive (false);
			}
		}
	}

	//Starts a fade back in and cleans up
	public void Restart()
	{
		cookMutex.WaitOne ();

		startupDoodads.SetActive (false);

		numberOfCooks = 1;
		for (int i = 0; i < cooks.Count; i++) {
			Destroy (cooks [i]);
		}
		cooks.Clear ();

		BloodController[] bloods = FindObjectsOfType<BloodController> ();
		for (int i = 0; i < bloods.Length; i++) {
			Destroy (bloods [i].gameObject);
		}

		fadeInStart = Time.time;
		finalScoreObject.GetComponent<Text> ().text = "";
		textObject.GetComponent<Text> ().text = "";
		textObject.SetActive (true);
		finalScoreObject.SetActive (false);
		tryAgainButton.SetActive (false);
		bigCook1.SetActive (false);
		bigCook2.SetActive (false);
		gunUI.SetActive (true);

		playing = true;

		HighScores.Instance.gameObject.SetActive (false);

		PlayerController.Instance.reset ();

		cookMutex.ReleaseMutex ();
	}

	public void CookDied(GameObject cook)
	{
		cookMutex.WaitOne ();

		bool res = cooks.Remove (cook);
		Destroy (cook);

		numberOfCooks += newCooksPerDeath;
		cookSpeed += cookSpeedGainPerDeath;

		cookMutex.ReleaseMutex ();
	}

	// Use this for initialization
	void Start () {
		cookMutex.WaitOne ();

		numberOfCooks = startingNumberOfCooks;
		cookSpeed = startingCookSpeed;

		tryAgainButton.GetComponent<Button> ().onClick.AddListener (Restart);

		cookMutex.ReleaseMutex ();
	}
	
	// Update is called once per frame
	void Update () {
		if (playing && cooks.Count < maxSpawnedCooks && cooks.Count < numberOfCooks) {
			spawnCooks ();
		}
		RunFade ();
	}

	void spawnCooks()
	{
		cookMutex.WaitOne ();

		numberOfCooks += cooksToSpawn;

		while (cooks.Count < numberOfCooks) {

			GameObject spawnPoint = cookSpawnPoints [Random.Range (0, cookSpawnPoints.Length)];
			Vector3 pos = new Vector3 (spawnPoint.transform.position.x, spawnPoint.transform.position.y, transform.localPosition.z);
			GameObject newCook = GameObject.Instantiate (cookPrefab);
			newCook.transform.SetParent(transform);
			newCook.transform.position = pos;
			newCook.GetComponent<CookController> ().speed = cookSpeed;
			cooks.Add (newCook);
		}


		cooksToSpawn = 0;

		cookMutex.ReleaseMutex ();

		float intensity = Mathf.Max (0.0f, Mathf.Min (1.0f, (numberOfCooks - mistInKillCount) / (mistMaxKillCount - mistInKillCount)));

		textObject.GetComponent<Text> ().text = numberOfCooks.ToString () + (numberOfCooks > 1? " cooks" : " cook") + " in the kitchen.";
		Color textColor = textObject.GetComponent<Text> ().color;
		textColor.g = 1.0f - intensity;
		textColor.b = 1.0f - intensity;
		textObject.GetComponent<Text> ().color = textColor;

		if (mistParticleSystem != null && numberOfCooks > mistInKillCount) {
			//mistParticleSystem.Emit((int) (intensity * mistRateMax));
		}
		if (sceneLight != null) {
			Color c = sceneLight.color;
			c.r = 1.0f - intensity * 0.9f;
			c.g = 1.0f - intensity * 0.9f;
			c.b = 1.0f - intensity;
			sceneLight.color = c;
		}
		if (bigCook1 != null && intensity > 0) {
			bigCook1.SetActive (true);
			SpriteRenderer[] sprites = bigCook1.GetComponentsInChildren<SpriteRenderer> ();
			for (int i = 0; i < sprites.Length; i++) {
				Color c = sprites [i].color;
				c.a = intensity;
				sprites [i].color = c;
			}
		}
		if (bigCook2 != null && intensity > 0) {
			bigCook2.SetActive (true);
			SpriteRenderer[] sprites = bigCook2.GetComponentsInChildren<SpriteRenderer> ();
			for (int i = 0; i < sprites.Length; i++) {
				Color c = sprites [i].color;
				c.a = intensity;
				sprites [i].color = c;
			}
		}
	}
}
