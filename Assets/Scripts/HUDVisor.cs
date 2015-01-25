using UnityEngine;
using System.Collections;

public class HUDVisor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PickedUpObject(GameObject pickup) {
		// show the sprite icon for the pickup
		Sprite pickupIcon = pickup.GetComponent<DebrisAudio> ().HUDIconForThisDebris;
	}
}
