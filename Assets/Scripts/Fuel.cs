using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fuel : MonoBehaviour {

	public Slider fuelBarSlider;  //reference for slider
	public Text fuelText;   //reference for text
	public int fuel;
	// Use this for initialization
	void Start () {
		fuel = 100;
	}
	
	// Update is called once per frame
	void Update () {
		fuel -= 1;
		if(fuel < 0) {
			fuel = 100;
		}
		fuelBarSlider.value = fuel/100.0f;
	}
}
