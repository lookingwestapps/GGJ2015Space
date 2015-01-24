using UnityEngine;
using System.Collections.Generic;

public class PlayerSpaceController : MonoBehaviour {

	public float movementSpeed = 4.0f;
	public Transform centerEyeAnchor; // a link to the center eye anchor
	public Transform playerSpaceSuit; // a link to the space suit.

	private List<GameObject> debrisWithinReach;
	private GameObject holdingLeftHand; // what each hand is currently holding or null
	private GameObject holdingRightHand;

	// Use this for initialization
	void Start () {
		debrisWithinReach = new List<GameObject> ();
	}

	public Vector2 leftStickSensitivity = new Vector2(2,2);
	public bool invertYAxis = false;

	public float thrustSensitivity = 2.5f;

	public Transform grabText;

	private bool leftArmExtended;
	private bool rightArmExtended;

	public DebrisManager debrisManager; // a link to the debris manager

	// Update is called once per frame
	void Update () {
		// == RESET OCULUS POSITION TRACKING POSTION ==
		bool startButtonPressed = Input.GetButtonUp ("Start");
		if (startButtonPressed) {
			// Start button pressed
			// recenter tracker position
			OVRManager.display.RecenterPose ();
		}

		//  ==== MOVEMENT / ROTATION ====
		// rotate the player based on user control.
		float h = Input.GetAxis ("Horizontal") * leftStickSensitivity.x;
		float v = Input.GetAxis ("Vertical") * leftStickSensitivity.y;
		if (invertYAxis) {
			v *= -1;
		}

		// ROTATE  where we are looking. If there is no controller input
		if ((h == 0) && (v == 0)) {
			v = centerEyeAnchor.localRotation.x;
			h = centerEyeAnchor.localRotation.y;
		}

		transform.Rotate(new Vector3(v, h, 0));

		// ADD THRUST
		if (Input.GetButton("Thrust")) {
			movementSpeed += thrustSensitivity * Time.deltaTime;
		}

		// move in the direction the player is looking
//		transform.position += centerEyeAnchor.forward * movementSpeed * Time.deltaTime;
		// move in the direction the player is facing
		transform.position += transform.forward * movementSpeed * Time.deltaTime;

		// we must also position the player's space suit to follow the camera's movement from the position tracking camera
		playerSpaceSuit.position = centerEyeAnchor.position;

		// === GRABBING OBJECTS ==
		// show UI "GRAB" text over any object that is both near enough and in our line of vision.
		// only show 1 "GRAB" text at a time, so choose the object nearest to the player's line of sight
		float minDistance = 9999999f;
		GameObject closestObjectWithinReach = null;
		foreach (GameObject debris in debrisWithinReach) {
			Vector2 posOnScreen = Camera.main.WorldToScreenPoint(debris.transform.position);
			float dist = Vector2.Distance(posOnScreen, new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
			if (dist < minDistance) {
				// this object is closer to the center of the screen
				minDistance = dist;
				closestObjectWithinReach = debris;
			}
		}
		if (closestObjectWithinReach != null) {
			// place "GRAB" text on this object
			grabText.position = closestObjectWithinReach.transform.position;
			grabText.gameObject.SetActive(true);
		} else {
			// remove the "GRAB" text
			grabText.gameObject.SetActive(false);
		}

		// check if player is grabbing anything
		if ((Input.GetAxis("RightTrigger") > 0f) && (rightArmExtended == false)) {
			// right trigger pushed, we also act when the trigger is released
			rightArmExtended = true;

			// TODO: animate the arm out to grab object


			// remove the "GRAB" text
			grabText.gameObject.SetActive(false);

		} 
		if ((Input.GetAxis("RightTrigger") <= 0f) && (rightArmExtended == true)) {
			// right trigger released
			rightArmExtended = false;

			// now we have pulled the object in
			holdingRightHand = closestObjectWithinReach;

			// tell the debris manager to remove the object
			debrisManager.RemoveDebris(closestObjectWithinReach.transform);

			// TODO: show inventory icon
		}


		// Same thing for the left arm
		if ((Input.GetAxis("LeftTrigger") > 0f) && (leftArmExtended == false)) {
			// left trigger pushed
			leftArmExtended = true;
		}
		// TODO: left arm implementation....
	}

	void OnCollisionEnter(Collision collision) {
//		Debug.Log ("Collision Enter! with:" + collision.gameObject.tag);

		// track all objects within reach
		if (collision.gameObject.tag == "Debris") {
			debrisWithinReach.Add(collision.gameObject);

		}
	}

	void OnCollisionExit(Collision collision) {
//		Debug.Log ("collision EXIT! with:" + collision.gameObject.tag);

		// track all objects leaving reach
		if (collision.gameObject.tag == "Debris") {
			debrisWithinReach.Remove(collision.gameObject);
		}
	}
}
