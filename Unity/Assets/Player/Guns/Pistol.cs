using UnityEngine;
using System.Collections;

public class Pistol : GunBase {

	public GameObject bulletPrefab = null;
	public AudioClip bulletSound = null;
	public float fireRate = 0.4f;
	AudioSource audioSource = null;
	private float lastFireTime = -1.0f;

	public void Start()
	{
		audioSource = GetComponent<AudioSource> ();
	}

	public override int UnlockKillCount()
	{
		return 0;
	}

	public override string GetGunName()
	{
		return "Pistol";
	}

	public override void Fire()
	{
		if (Time.time - lastFireTime > fireRate) {

			GameObject newBullet = GameObject.Instantiate (bulletPrefab);
			newBullet.transform.right = transform.right;
			newBullet.transform.position = transform.position + transform.rotation * new Vector3 (3.5f, 2.75f, 0.0f);
			newBullet.GetComponent<BulletController>().fire();

			lastFireTime = Time.time;
			audioSource.PlayOneShot (bulletSound, 0.5f);
		}
	}

	public override bool Auto()
	{
		return false;
	}
}
