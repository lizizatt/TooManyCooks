using UnityEngine;
using System.Collections;

public class GatlingGun : GunBase {

	public GameObject bulletPrefab = null;
	public AudioClip bulletSound = null;
	public AudioClip bulletDropSound = null;
	public AudioSource audioSource = null;
	public float soundFireRate = 1.0f;
	private float lastFireTime = -1.0f;

	public void Start()
	{
		audioSource = GetComponent<AudioSource> ();
	}

	public override void Fire()
	{
		GameObject newBullet = GameObject.Instantiate (bulletPrefab);
		newBullet.transform.right = transform.right;
		newBullet.transform.position = transform.position + transform.rotation * new Vector3 (9.5f, 0, 0.0f);
		newBullet.GetComponent<BulletController>().fire();

		if (bulletSound != null && bulletDropSound != null && (Time.time - lastFireTime) > soundFireRate) {
			audioSource.PlayOneShot (bulletSound, 0.5f);
			audioSource.PlayOneShot (bulletDropSound, 0.5f);
			lastFireTime = Time.time;
		}
	}

	public override bool Auto()
	{
		return true;
	}
}
