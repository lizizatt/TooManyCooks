using UnityEngine;
using System.Collections;

public class Flamethrower : GunBase {

	public AudioClip flamethrowerSound = null;
	private AudioSource audioSource = null;
	public ParticleSystem myParticleSystem = null;
	public GameObject flameCollider = null;
	public float fireRate = 1.0f;
	private float lastFireTime = -1.0f;

	public void Start()
	{
		audioSource = GetComponent<AudioSource> ();
		audioSource.clip = flamethrowerSound;
	}

	public override int UnlockKillCount()
	{
		return 200;
	}

	public override string GetGunName()
	{
		return "Flamethrower";
	}

	public override void Fire()
	{
		if (Time.time - lastFireTime > fireRate) {

			//do whatever fire needs

			lastFireTime = Time.time;
			audioSource.Play ();
			myParticleSystem.Play ();
			flameCollider.SetActive (true);
		}
	}

	public override void StopFire()
	{
		audioSource.Stop ();
		myParticleSystem.Stop ();
		flameCollider.SetActive (false);
	}

	public override bool Auto()
	{
		return true;
	}
}
