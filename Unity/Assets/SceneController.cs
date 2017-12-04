using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;
using System.Collections.Generic;


public class SceneController : MonoBehaviour {

	public float spawnWallXVal = 5;
	public float spawnWallHeight = 5;
	public GameObject cookPrefab = null;
	public GameObject[] cookSpawnPoints = null;

	public GameObject blackMask = null;
	public float fadeLength = 2.0f;

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

	private float fadeStart = -1.0f;

	private int numberOfCooks;
	private float cookSpeed;

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

	public void PlayerDied()
	{
		fadeStart = Time.time;
	}

	void runFade()
	{
		if (fadeStart > 0 && blackMask != null) {
			SpriteRenderer sr = blackMask.GetComponent<SpriteRenderer> ();
			Color c = sr.color;
			c.a = Mathf.Min (Mathf.Max (0.0f, (Time.time - fadeStart) / fadeLength), 1.0f);
			sr.color = c;

			if (c.a <= 0.0f) {
				fadeStart = -1.0f;
			}
		}
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

		spawnCooks ();
	}
	
	// Update is called once per frame
	void Update () {
		if (cooks.Count < numberOfCooks) {
			spawnCooks ();
		}
		runFade ();
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

		textObject.GetComponent<Text> ().text = numberOfCooks.ToString () + " cooks in the kitchen.";

		cooksToSpawn = 0;

		cookMutex.ReleaseMutex ();

		float intensity = Mathf.Max (0.0f, Mathf.Min (1.0f, (numberOfCooks - mistInKillCount) / (mistMaxKillCount - mistInKillCount)));
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
