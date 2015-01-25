using UnityEngine;
using System.Collections;

public class PlayIntro : MonoBehaviour {
	public MovieTexture movTexture;
	void Start() {
		renderer.material.mainTexture = movTexture;
		movTexture.Play();
	}

	
	// Update is called once per frame
	void Update () {
	
	}
}
