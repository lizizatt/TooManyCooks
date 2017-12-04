using UnityEngine;
using System.Collections;

public abstract class GunBase : MonoBehaviour {
	public abstract void Fire(); //sends projectiles wherever the gun's RIGHT direction is
	public virtual void StopFire() {}  //optional override callback that will be called after button is released.  only called if Auto() returns true
	public abstract bool Auto(); //whether or not should fire when fire1 is held down
	public abstract int UnlockKillCount();
	public abstract string GetGunName(); //gun name

	//unlocked accessor
	public bool Unlocked() 
	{ 
		return SceneController.Instance.godMode? true : (SceneController.Instance.GetNumCooks() > UnlockKillCount());
	}

	public bool IsActiveGun()
	{
		return gameObject.activeInHierarchy;
	}
}
