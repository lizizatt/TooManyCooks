using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float mouseSensitivityMultiplier = 5.0f;

	public GunBase[] guns = null;
	public int activeGun = 0;

	private GameObject _reticle = null;

	private float prevMouseX = -1;
	private float prevMouseY = -1;

	public float gunDistance = 1.5f;

	private Vector3 originalGunRight;

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
	
	// Update is called once per frame
	void Update () {
		float mouseX = Input.GetAxis ("Mouse X") * mouseSensitivityMultiplier;
		float mouseY = Input.GetAxis ("Mouse Y") * mouseSensitivityMultiplier;
		if (prevMouseX != -1) {
			Vector3 newPos = _reticle.transform.position + new Vector3 (mouseX, mouseY, 0);
			newPos.x = Mathf.Min(Mathf.Max(newPos.x, -75), 75);
			newPos.y = Mathf.Min(Mathf.Max(newPos.y, -75), 75);

			_reticle.transform.position = newPos;
		}

		prevMouseX = mouseX;
		prevMouseY = mouseY;

		if (Input.GetButtonDown("Fire2")) {
			guns [activeGun].gameObject.SetActive (false);
			activeGun = (activeGun + 1) % guns.Length;
			guns [activeGun].gameObject.SetActive (true);
		}	

		PositionGun ();

		if (Input.GetButtonDown("Fire1")
			|| (guns[activeGun].Auto() && Input.GetButton("Fire1"))) {
			guns[activeGun].Fire ();
		}
	}

	void PositionGun()
	{
		guns[activeGun].transform.localPosition = _reticle.transform.localPosition.normalized * gunDistance;
		guns[activeGun].transform.right = _reticle.transform.localPosition.normalized ;
	}
}
