using UnityEngine;
using System.Collections;

public class HUDVisor : MonoBehaviour {

	public Transform sriracha;
	public Transform clockIcon;
	public Transform nesController;
	public Transform phonographIcon;
	public Transform bearIcon;
	public Transform tacoIcon;
	public Transform coffeeIcon;
	public Transform paintingIcon;
	public Transform pizzaIcon;
	public Transform carIcon;

	public AudioClip breathingSlow;
	public AudioClip breathingFast;
	public AudioClip breathingHeavy;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PickedUpObject(GameObject pickup) {
		// show the sprite icon for the pickup
		Sprite pickupIcon = pickup.GetComponent<DebrisAudio> ().HUDIconForThisDebris;
		Debug.Log("Pickedup ICON:" + pickupIcon.ToString());
		if (pickupIcon.ToString() == "car (UnityEngine.Sprite)") {
			carIcon.renderer.enabled = true;
		} else if (pickupIcon.ToString() == "bear (UnityEngine.Sprite)") {
			bearIcon.renderer.enabled = true;
		} else if (pickupIcon.ToString() == "clock (UnityEngine.Sprite)") {
			clockIcon.renderer.enabled = true;
		} else if (pickupIcon.ToString() == "controller (UnityEngine.Sprite)") {
			nesController.renderer.enabled = true;
		} else if (pickupIcon.ToString() == "painting (UnityEngine.Sprite)") {
			paintingIcon.renderer.enabled = true;
		} else if (pickupIcon.ToString() == "phonograph (UnityEngine.Sprite)") {
			phonographIcon.renderer.enabled = true;
		} else if (pickupIcon.ToString() == "pizza (UnityEngine.Sprite)") {
			pizzaIcon.renderer.enabled = true;
		} else if (pickupIcon.ToString() == "sriracha (UnityEngine.Sprite)") {
			sriracha.renderer.enabled = true;
		} else if (pickupIcon.ToString() == "starbucks (UnityEngine.Sprite)") {
			coffeeIcon.renderer.enabled = true;
		} else if (pickupIcon.ToString() == "taco (UnityEngine.Sprite)") {
			tacoIcon.renderer.enabled = true;
		} else {
			Debug.Log("ERROR can't find icon!"); // shouldn't run
		}
	}

	public void PlayBreathingFast() {
		audio.clip = breathingFast;
		audio.Play ();
	}
	public void PlayBreathingSoft() {
		audio.clip = breathingSlow;
		audio.Play ();
	}
	public void PlayBreathingHeavy() {
		audio.clip = breathingHeavy;
		audio.Play ();
	}
}
