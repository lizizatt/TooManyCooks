using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GunUIController : MonoBehaviour {

	public GunBase[] guns = null;
	public GameObject[] gunUIElements = null;  //these two arrays are assumed to match each other in length & entries
	private List<Image> backdropImages = new List<Image>();
	private List<Text> texts = new List<Text>();

	// Use this for initialization
	void Start () {
		for (int i = 0; i < guns.Length; i++) {
			Image[] ims = gunUIElements[i].GetComponentsInChildren<Image> ();
			for (int j = 0; j < ims.Length; j++) {
				if (ims [j].gameObject.name == "Backdrop") {
					backdropImages.Add (ims [j]);
					break;
				}
			}

			Text[] tex = gunUIElements[i].GetComponentsInChildren<Text> ();
			for (int j = 0; j < tex.Length; j++) {
				if (tex [j].gameObject.name == "Text") {
					texts.Add (tex [j]);
					break;
				}
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < guns.Length; i++) {
			GunBase gun = guns [i];
			Text tex = texts [i];
			Image im = backdropImages[i];

			Color c = im.color;
			c.g = (guns[i].IsActiveGun())? 120.0f/255.0f : (gun.Unlocked () ? 60.0f/255.0f : 43.0f/255.0f);
			//Debug.Log ("i " + i + " c.g " + c.g);
			im.color = c;

			tex.text = gun.GetGunName() + "\n" + (gun.Unlocked () ? "Unlocked" : "Unlocks at " + gun.UnlockKillCount ());
		}
	}
}
