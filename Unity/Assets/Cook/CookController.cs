using UnityEngine;
using System.Collections;

public class CookController : MonoBehaviour {

	private GameObject _player;
	private float health = 50;
	public int maxNumPlayingSongs = 50;
	public float speed = 1.0f;
	public GameObject bloodSplatterPrefab = null;
	bool died = false;

	static int numCooks = 0;

	// Use this for initialization
	void Start () {
		_player = PlayerController.Instance.gameObject;
		Debug.Assert (_player != null);
		numCooks++;

		if (numCooks < maxNumPlayingSongs) {
			GetComponentInChildren<AudioSource> ().Play ();
		}
	}

	void OnDestroy()
	{
		numCooks--;
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.position += new Vector3 (_player.transform.position.x - this.gameObject.transform.position.x, _player.transform.position.y - this.gameObject.transform.position.y, _player.transform.position.z - this.gameObject.transform.position.z).normalized * speed * Time.deltaTime;
	}

	void OnCollisionStay2D(Collision2D collision) {
		if (collision.gameObject.name == "FlamethrowerCollider") {
			takeDamage (1);
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
			if (collision.enabled && collision.gameObject != null) {
			if (collision.gameObject.name != "FlamethrowerCollider") {
				BulletController controller = collision.gameObject.GetComponent<BulletController> ();

				if (controller != null && !controller.hit) {
					takeDamage (25);
					collision.gameObject.GetComponent<BulletController> ().hit = true;
					collision.gameObject.SetActive (false);
					DestroyObject (collision.gameObject);
				}
			}
		}
	}

	void takeDamage(float damage)
	{
		health -= damage;
		if (health <= 0) {
			if (!died) {
				SceneController.Instance.CookDied (gameObject);
				GameObject bs = GameObject.Instantiate (bloodSplatterPrefab);
				bs.transform.position = transform.position;
				bs.transform.RotateAround (bs.transform.position, new Vector3 (0, 0, 1), Random.Range (0, 360));
				died = true;
			}
		}
	}
}
