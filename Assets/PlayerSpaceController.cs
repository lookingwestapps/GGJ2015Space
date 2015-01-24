using UnityEngine;
using System.Collections;

public class PlayerSpaceController : MonoBehaviour {

	public float speed = 4.0f;
	public Transform centerEyeAnchor;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += centerEyeAnchor.forward * speed * Time.deltaTime;
	}
}
