using UnityEngine;
using System.Collections;

public class GatlingGun : GunBase {

	public GameObject bulletPrefab = null;
		
	public override void Fire()
	{
		GameObject newBullet = GameObject.Instantiate (bulletPrefab);
		newBullet.transform.right = transform.right;
		newBullet.transform.position = transform.position + transform.rotation * new Vector3 (3.5f, 2.75f, 0.0f);
		newBullet.GetComponent<BulletController>().fire();
	}

	public override bool Auto()
	{
		return true;
	}
}
