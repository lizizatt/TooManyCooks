using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;
using System.Collections.Generic;


public class SceneController : MonoBehaviour {

	public float spawnWallXVal = 5;
	public float spawnWallHeight = 5;
	public GameObject cookPrefab = null;

	public int startingNumberOfCooks = 1;
	private int numberOfCooks;
	public int newCooksPerDeath = 1;
	public float startingCookSpeed = 5.0f;
	public float cookSpeedGainPerDeath = .01f;
	private float cookSpeed;

	public GameObject textObject = null;

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

	}

	void spawnCooks()
	{
		cookMutex.WaitOne ();

		numberOfCooks += cooksToSpawn;

		while (cooks.Count < numberOfCooks) {
			Vector3 pos = new Vector3 (Random.Range (30.0f, 50f), Random.Range (-25f, 25f), transform.localPosition.z);
			GameObject newCook = GameObject.Instantiate (cookPrefab);
			newCook.transform.SetParent(transform);
			newCook.transform.position = pos;
			newCook.GetComponent<CookController> ().speed = cookSpeed;
			cooks.Add (newCook);
		}

		textObject.GetComponent<Text> ().text = numberOfCooks.ToString () + " cooks in the kitchen.";

		cooksToSpawn = 0;

		cookMutex.ReleaseMutex ();
	}
}
