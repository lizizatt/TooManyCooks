using UnityEngine;
using System.Collections;

public abstract class GunBase : MonoBehaviour {

	public abstract void Fire(); //sends projectiles wherever the gun's RIGHT direction is
	public abstract bool Auto(); //whether or not should fire when fire1 is held down
	public abstract int UnlockKillCount();  //what kill count this weapon becomes available
	public abstract string GetGunName(); //gun name

	//unlocked accessor
	public bool Unlocked() 
	{ 
		return SceneController.Instance.GetNumCooks() > UnlockKillCount();
	}

	public bool IsActiveGun()
	{
		return gameObject.activeInHierarchy;
	}
}
