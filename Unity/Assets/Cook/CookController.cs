using UnityEngine;
using System.Collections;

public class CookController : MonoBehaviour {

	private GameObject _player;
	private float health = 50;
	public float speed = 1.0f;
	bool died = false;

	// Use this for initialization
	void Start () {
		_player = PlayerController.Instance.gameObject;
		Debug.Assert (_player != null);
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.position += new Vector3 (_player.transform.position.x - this.gameObject.transform.position.x, 0, _player.transform.position.z - this.gameObject.transform.position.z).normalized * speed * Time.deltaTime;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.enabled && collision.gameObject != null) {
			BulletController controller = collision.gameObject.GetComponent<BulletController> ();

			if (controller != null && !controller.hit) {
				takeDamage (25);
				collision.gameObject.GetComponent<BulletController> ().hit = true;
				collision.gameObject.SetActive (false);
				DestroyObject (collision.gameObject);
			}
		}
	}

	void takeDamage(float damage)
	{
		health -= damage;
		if (health <= 0) {
			if (!died) {
				SceneController.Instance.CookDied (gameObject);
				died = true;
			}
		}
	}
}
