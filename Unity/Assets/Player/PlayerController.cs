using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float mouseSensitivityMultiplier = 5.0f;

	public GunBase[] guns = null;
	public int activeGun = 0;

	public AudioClip[] murderSounds = null;
	public AudioSource playerAudioSource = null;


	private GameObject _reticle = null;

	public float gunDistance = 1.5f;

	private Vector3 originalGunRight;

	bool dead = true;

	private static PlayerController _instance = null;
	public static PlayerController Instance
	{
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<PlayerController>();
			}
			return _instance;
		}
	}

	// Use this for initialization
	void Start () {
		SpriteRenderer[] sprites = gameObject.GetComponentsInChildren<SpriteRenderer> (true);
		for (int i = 0; i < sprites.Length; i++) {
			if (sprites [i].gameObject.name == "Reticle") {
				_reticle = sprites [i].gameObject;
			}
		}
		Debug.Assert (_reticle != null);
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (!dead && collision.enabled && collision.gameObject != null) {
			CookController controller = collision.gameObject.GetComponent<CookController>();

			if (controller != null) {
				die ();
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (dead) {
			return;
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		float rayDistance;
		Plane playerPlane = new Plane (new Vector3 (0, 0, 1.0f), this.transform.position);
		if (playerPlane.Raycast(ray, out rayDistance))
			_reticle.transform.position = ray.GetPoint(rayDistance);

		if (Input.GetButtonDown("Fire2")) {
			while (true) {
				guns [activeGun].gameObject.SetActive (false);
				activeGun = (activeGun + 1) % guns.Length;
				if (guns[activeGun].Unlocked()) {
					guns [activeGun].gameObject.SetActive (true);
					break;
				}
			}
		}	

		PositionGun ();

		if (Input.GetButtonDown("Fire1")
			|| (guns[activeGun].Auto() && Input.GetButton("Fire1"))) {
			guns[activeGun].Fire ();
		}
		if (Input.GetButtonUp("Fire1") && guns[activeGun].Auto()) {
			guns[activeGun].StopFire ();
		}
	}

	public void die()
	{
		dead = true;
		for (int i = 0; playerAudioSource != null && i < murderSounds.Length; i++) {
			playerAudioSource.PlayOneShot (murderSounds [i]);
		}

		SceneController.Instance.PlayerDied ();
	}

	public void reset()
	{
		dead = false;

		guns [activeGun].gameObject.SetActive (false);
		activeGun = 0;
		guns [activeGun].gameObject.SetActive (true);
	}

	void PositionGun()
	{
		guns[activeGun].transform.localPosition = _reticle.transform.localPosition.normalized * gunDistance;
		guns[activeGun].transform.right = _reticle.transform.localPosition.normalized ;
	}
}
