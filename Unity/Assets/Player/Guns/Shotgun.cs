using UnityEngine;
using System.Collections;

public class Shotgun : GunBase {

	public GameObject bulletPrefab = null;
	public AudioClip bulletSound = null;
	public AudioSource audioSource = null;
	public float bulletsPerFire = 5.0f;
	public float fireRate = 1.0f;
	private float lastFireTime = -1.0f;

	public void Start()
	{
		audioSource = GetComponent<AudioSource> ();
	}

	public override int UnlockKillCount()
	{
		return 10;
	}

	public override string GetGunName()
	{
		return "Shotgun";
	}

	public override void Fire()
	{
		if (Time.time - lastFireTime > fireRate) {

			for (int i = 0; i < bulletsPerFire; i++) {
				GameObject newBullet = GameObject.Instantiate (bulletPrefab);
				newBullet.transform.right = transform.right;
				newBullet.transform.position = transform.position + transform.rotation * new Vector3 (3.0f, 0, 0.0f);
				newBullet.GetComponent<BulletController>().fire();
			}

			lastFireTime = Time.time;
			audioSource.PlayOneShot (bulletSound, 0.5f);
		}
	}

	public override bool Auto()
	{
		return true;
	}
}
