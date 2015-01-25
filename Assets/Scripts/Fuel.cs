using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fuel : MonoBehaviour {

	public Slider fuelBarSlider;  //reference for slider
	public Text fuelText;   //reference for text
	public float fuel;
	public float usageRate;
	// Use this for initialization
	void Start () {
		fuel = 100;
		usageRate = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		fuel -= usageRate;
		if(fuel < 0) {
			// DIE
			// TODO: replace this with the die routine
			fuel = 100;
		}
		fuelBarSlider.value = fuel/100.0f;
	}
}
