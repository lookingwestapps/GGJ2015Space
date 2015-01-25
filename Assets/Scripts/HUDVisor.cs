using UnityEngine;
using System.Collections;

public class HUDVisor : MonoBehaviour {

	public Sprite[] allIcons;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PickedUpObject(GameObject pickup) {
		// show the sprite icon for the pickup
		Sprite pickupIcon = pickup.GetComponent<DebrisAudio> ().HUDIconForThisDebris;
		foreach (Sprite icon in allIcons) {
			if (icon == pickupIcon) {
				// show this icon
//				icon.GetComponent<SpriteRenderer>().enabled = true;
			}
		}
	}
}
