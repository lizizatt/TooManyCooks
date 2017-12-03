using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

	public float speed = 15.0f;
	public bool hit = false;

	public void fire()
	{
		gameObject.SetActive (true);
	}

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		gameObject.transform.position += transform.right.normalized * speed * Time.deltaTime;

		if (Mathf.Abs(gameObject.transform.position.x) > 100 || Mathf.Abs(gameObject.transform.position.y) > 100) {
			Destroy (this.gameObject);
		}
	}
}
