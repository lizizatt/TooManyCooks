using UnityEngine;
using System.Collections;

public class GatlingGun : GunBase {

	public GameObject bulletPrefab = null;
	public AudioClip bulletSound = null;
	public AudioClip bulletDropSound = null;
	public AudioSource audioSource = null;

	public void Start()
	{
		audioSource = GetComponent<AudioSource> ();
	}

	public override void Fire()
	{
		GameObject newBullet = GameObject.Instantiate (bulletPrefab);
		newBullet.transform.right = transform.right;
		newBullet.transform.position = transform.position + transform.rotation * new Vector3 (3.5f, 2.75f, 0.0f);
		newBullet.GetComponent<BulletController>().fire();

		if (bulletSound != null) {
			audioSource.PlayOneShot (bulletSound);
		}
		if (bulletDropSound != null) {
			audioSource.PlayOneShot (bulletDropSound);
		}
	}

	public override bool Auto()
	{
		return true;
	}
}
