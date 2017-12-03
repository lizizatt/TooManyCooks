using UnityEngine;
using System.Collections;

public class BloodController : MonoBehaviour {

	public Sprite[] sprites;

	// Use this for initialization
	void Start () {
		GetComponent<SpriteRenderer> ().sprite = sprites [Random.Range (0, sprites.Length)];
	}
}
