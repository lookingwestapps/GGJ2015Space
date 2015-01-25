using UnityEngine;
using System.Collections.Generic;

public class PlayerSpaceController : MonoBehaviour {

	public float movementSpeed = 4.0f;
	public float maxSpeed = 10f;
	public Transform centerEyeAnchor; // a link to the center eye anchor
	public Transform leftEyeAnchor; // a link to the left eye anchor
	public Transform playerSpaceSuit; // a link to the space suit.
	public GameObject explosion; // for detonator prefab

	private Object myExp; 

	public bool useWithNoController = true;

	private List<GameObject> debrisWithinReach;
//	private GameObject holdingLeftHand; // what each hand is currently holding or null
//	private GameObject holdingRightHand;

	// Use this for initialization
	void Start () {
		debrisWithinReach = new List<GameObject> ();
	}

	public Vector2 leftStickSensitivity = new Vector2(2,2);
	public Vector2 turningByLookingSensitivity = new Vector2(2,2);
	public bool invertYAxis = false;

	public float thrustSensitivity = 1f;
	public float speedWhileTurning = 4.0f;

	public Transform grabText;

	private bool leftArmExtended;
	private bool rightArmExtended;

	public DebrisManager debrisManager; // a link to the debris manager

	private Transform partner; // this is the goal object that appears after playing a while
	public float minimumTimeToSpawnPartner = 60f;
	private float partnerSpawnTimer = 0f;


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
			v = centerEyeAnchor.localRotation.x * turningByLookingSensitivity.x;
			h = centerEyeAnchor.localRotation.y * turningByLookingSensitivity.y;
		}

		transform.Rotate(new Vector3(v, h, 0));

//		Debug.Log ("v:" + v + " h:" + h + " rotation distance:" + Vector2.SqrMagnitude(new Vector2(centerEyeAnchor.localRotation.x, centerEyeAnchor.localRotation.y)));
		// slow and speed up based on turning if there is no controller input selected
		if (useWithNoController == true) {
			if ((Mathf.Abs(centerEyeAnchor.localRotation.x) > 0.1f) || (Mathf.Abs(centerEyeAnchor.localRotation.y) > 0.1f)) {
				// slow down while turning if there is no controller input selected
				movementSpeed -= thrustSensitivity * Time.deltaTime * 4f;
				if (movementSpeed < speedWhileTurning) {
					movementSpeed = speedWhileTurning;
				}
			} else if (Vector2.SqrMagnitude(new Vector2(centerEyeAnchor.localRotation.x, centerEyeAnchor.localRotation.y)) < 0.02f) {
				// speed up while looking straight ahead.
				movementSpeed += thrustSensitivity * Time.deltaTime;
				if (movementSpeed > maxSpeed) {
					movementSpeed = maxSpeed;
				}
			}
		}

		                        
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
			Vector2 posOnScreen = leftEyeAnchor.camera.WorldToScreenPoint(debris.transform.position);
			float dist = Vector2.Distance(posOnScreen, new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
			if (dist < minDistance) {
				// this object is closer to the center of the screen
				minDistance = dist;
				closestObjectWithinReach = debris;
			}
		}
		if (closestObjectWithinReach != null) {
			// place "GRAB" text on this object
			grabText.position = Vector3.Lerp(transform.position, closestObjectWithinReach.transform.position, 0.3f);
			grabText.rotation = transform.rotation;
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


		} 
		if ((Input.GetAxis("RightTrigger") <= 0f) && (rightArmExtended == true)) {
			// right trigger released
			rightArmExtended = false;

			if (closestObjectWithinReach != null) {
				// now we have pulled the object in
				// grab it!
				GrabObject(closestObjectWithinReach);
			}
		}
		// Same thing for the left arm
		if ((Input.GetAxis("LeftTrigger") > 0f) && (leftArmExtended == false)) {
			// left trigger pushed
			leftArmExtended = true;
		}
		// TODO: left arm implementation....


		// Auto grab objects when they collide with the astronaut 
		if (useWithNoController == true) {
			GameObject debrisToGrab = null;
			foreach (GameObject debris in debrisWithinReach) {
				float dist = Vector3.Distance(transform.position, debris.transform.position);
				if (dist < 1.5f) {
					// object is close enough to grab it. Grab it!
					Debug.Log("AUTO GRABBING OBJECT!!:" + debris);
					debrisToGrab = debris;
				}
			}
			if (debrisToGrab != null) {
				GrabObject(debrisToGrab);
			}
		}


		// == GAME LOGIC TO SPAWN PARTNER ==
		// Partner timer
		if (partnerSpawnTimer > 0) {
			partnerSpawnTimer -= Time.deltaTime;
		}
		// Check if it's time to spawn the "partner"
		if ((partner == null) && (minimumTimeToSpawnPartner <= 0)) {
			// spawn partners
		}


		// == EXPLODE OBJECT ==
		// when user presses button close to a debris
		if( Input.GetButton("Explode")) {
			if(closestObjectWithinReach != null && closestObjectWithinReach.transform != null) {
				myExp = Instantiate(explosion, closestObjectWithinReach.transform.position,
				            Quaternion.identity);
			}
		}
	}

	void GrabObject(GameObject debris) {
//		holdingRightHand = closestObjectWithinReach;

		// play sfx
		audio.PlayOneShot (debris.GetComponent<DebrisAudio> ().audioClip);

		debrisWithinReach.Remove(debris);
		
		// tell the debris manager to remove the object
		debrisManager.RemoveDebris(debris.transform);
		
//				Debug.Log("right trigger released, grabbed:" + closestObjectWithinReach);
		// TODO: show inventory icon
	}

	void OnTriggerEnter(Collider other) {
//		Debug.Log ("Collision Enter! with:" + other.gameObject.tag);

		// track all objects within reach
		if (other.gameObject.tag == "Debris") {
			debrisWithinReach.Add(other.gameObject);
		}
	}

	void OnTriggerExit(Collider other) {
//		Debug.Log ("collision EXIT! with:" + other.gameObject.tag);

		// track all objects leaving reach
		if (other.gameObject.tag == "Debris") {
			debrisWithinReach.Remove(other.gameObject);
		}
	}
}
