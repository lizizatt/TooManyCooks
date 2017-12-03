using UnityEngine;
using System.Collections;

public abstract class GunBase : MonoBehaviour {

	public abstract void Fire(); //sends projectiles wherever the gun's RIGHT direction is
	public abstract bool Auto(); //whether or not should fire when fire1 is held down
}
