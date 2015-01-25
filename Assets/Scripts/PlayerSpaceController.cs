using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSpaceController : MonoBehaviour {

	public float movementSpeed = 4.0f;
	public float maxSpeed = 10f;
	public Transform centerEyeAnchor; // a link to the center eye anchor
	public Transform leftEyeAnchor; // a link to the left eye anchor
	public Transform playerSpaceSuit; // a link to the space suit.
	public GameObject explosion; // for detonator prefab
	public Transform fadeToBlackSprite; // this is a black sprite place right in front of the eyes to fade the scene to black
	public Transform handInHandWinner; // link to the hand in hand model that we show for the win condition
	public HUDVisor hudVisor; // link
	public Fuel fuelObj;

	public TextMesh whatDoWeDoNowText; // a link to text mesh
	public TextMesh whereDoWeGoNowText; // a link to text mesh 

	private const float LOW_USAGE = 0.001f;
	private const float HIGH_USAGE = 0.01f;

//	private Fuel fuelObj;
	public Canvas canvasPrefab;

	private Object myExp; 

	public bool useWithNoController = true;

	private bool skipOpeningScene = false;

	private List<GameObject> debrisWithinReach;
	private bool earthHasExploded;
	private bool allowRotation;
	private bool partnerFound;
	private List<GameObject> objectsCollected;
//	private GameObject holdingLeftHand; // what each hand is currently holding or null
//	private GameObject holdingRightHand;

	
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
	public Transform partnerPrefab;
	public float minimumTimeToSpawnPartner = 60f;
	public float minObjectsBeforeSpawning = 2;
	private float partnerSpawnTimer = 0f;
	
	public Transform earth; // link to the earth.


	// Use this for initialization
	void Start () {
		debrisWithinReach = new List<GameObject> ();
		objectsCollected = new List<GameObject> ();

		StartCoroutine ("ExplodeEarthAfterDelay");

		// fade out texts and activate them
		whatDoWeDoNowText.GetComponent<MeshRenderer>().material.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0f);
		whatDoWeDoNowText.gameObject.SetActive (true);

		whereDoWeGoNowText.GetComponent<MeshRenderer>().material.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0f);
		whereDoWeGoNowText.gameObject.SetActive (true);

		fadeToBlackSprite.GetComponent<SpriteRenderer>().material.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0f);
		fadeToBlackSprite.gameObject.SetActive (true);

//		fuelObj = (Fuel)GameObject.FindObjectOfType(typeof(Fuel));
//		canvasPrefab = (Canvas)GameObject.FindObjectOfType(typeof(Canvas));
		fuelObj.usageRate = 0;
	}

	IEnumerator ExplodeEarthAfterDelay() {
		float innerTimer = 8.0f; // wait for 8 seconds with the option to skip.
		while ((innerTimer > 0) && (skipOpeningScene == false)) {
			innerTimer -= Time.deltaTime;
			yield return 0;
		}

		BlowUpTheEarth();

		innerTimer = 6.0f; // wait for 6 seconds with the option to skip.
		while ((innerTimer > 0) && (skipOpeningScene == false)) {
			innerTimer -= Time.deltaTime;
			yield return 0;
		}

		// fade in "What do we do now" text
		for (int i = 0; i < 120; i++) {
			float alpha = (float)i / 120f;
			whatDoWeDoNowText.GetComponent<MeshRenderer>().material.color = new Color(Color.white.r, Color.white.g, Color.white.b, alpha);
			yield return 0;
		}

		// TODO: throw big meteors by the player

		// create debris cloud
		debrisManager.CreateInitialDebris();
		
		// set bool to allow player to turn
		earthHasExploded = true;
		allowRotation = true;
		canvasPrefab.gameObject.SetActive(true);
		fuelObj.usageRate = LOW_USAGE;

		innerTimer = 6.0f; // wait for 6 seconds with the option to skip.
		while ((innerTimer > 0) && (skipOpeningScene == false)) {
			innerTimer -= Time.deltaTime;
			yield return 0;
		}

		// return to soft breathing
		hudVisor.PlayBreathingSoft ();

		// fade out "What do we do now" text
		for (int i = 80; i >= 0; i--) {
			float alpha = (float)i / 80f;
			whatDoWeDoNowText.GetComponent<MeshRenderer>().material.color = new Color(Color.white.r, Color.white.g, Color.white.b, alpha);
			yield return 0;
		}
	}
	
	IEnumerator PartnerFound() {
		if (partnerFound == false) { // only run once
			partnerFound = true;
			// HURRAY! THIS RUNS WHEN YOU WIN!
			// fade in black rect in front of eyes
			for (int i = 0; i <= 20; i++) {
				float alpha = (float)i / 20;
				fadeToBlackSprite.GetComponent<SpriteRenderer>().material.color = new Color(Color.white.r, Color.white.g, Color.white.b, alpha);
				yield return 0;
			}

			// load up the hand in hand model,
			Transform handInHand = (Transform)Instantiate (handInHandWinner);
			// add handInHand to the player object
			handInHand.parent = transform;
			// position relative to cam
			handInHand.localPosition = new Vector3 (0, 0, 10);

			// remove debris field
			debrisManager.RemoveAllDebris ();

			// don't allow turning anymore
			allowRotation = false;

			// hide helmet
			hudVisor.gameObject.SetActive (false);

			// point camera down.
			transform.rotation = Quaternion.Euler (90f, 0f, 0f);
			transform.position = new Vector3(0f,0f,0f);

			// start them moving forward
			handInHand.rigidbody.AddForce (transform.forward);
			handInHand.rigidbody.AddTorque (new Vector3(5f, 0f, 10f));

			// match the camera motion, just a little bit slower so the model floats away

			// small pause
			yield return new WaitForSeconds (0.2f);

			// fade in scene
			// fade out black rect in front of eyes
			for (int i = 80; i >= 0; i--) {
				float alpha = (float)i / 80f;
				fadeToBlackSprite.GetComponent<SpriteRenderer>().material.color = new Color(Color.white.r, Color.white.g, Color.white.b, alpha);
				yield return 0;
			}

			// show the whereDoWeGoNowText  text
			// fade in "Where do we go now" text
			for (int i = 0; i < 120; i++) {
				float alpha = (float)i / 120f;
				whereDoWeGoNowText.GetComponent<MeshRenderer>().material.color = new Color(Color.white.r, Color.white.g, Color.white.b, alpha);
				yield return 0;
			}


			// pause to watch
			yield return new WaitForSeconds (10.0f);

			// fade in black rect in front of eyes
			for (int i = 0; i <= 60; i++) {
				float alpha = (float)i / 60;
				fadeToBlackSprite.GetComponent<SpriteRenderer>().material.color = new Color(Color.white.r, Color.white.g, Color.white.b, alpha);
				yield return 0;
			}

			// load credits


			// fade scene in
			// fade out for the last time
			for (int i = 80; i >= 0; i--) {
				float alpha = (float)i / 80f;
				fadeToBlackSprite.GetComponent<SpriteRenderer>().material.color = new Color(Color.white.r, Color.white.g, Color.white.b, alpha);
				yield return 0;
			}
		}


		yield return 0;
	}
	IEnumerator FadeInScene() {
		// fade out black rect in front of eyes
		for (int i = 80; i >= 0; i--) {
			float alpha = (float)i / 80f;
			fadeToBlackSprite.GetComponent<SpriteRenderer>().material.color = new Color(Color.white.r, Color.white.g, Color.white.b, alpha);
			yield return 0;
		}
	}
	IEnumerator FadeOutScene() {
		// fade in black rect in front of eyes
		for (int i = 0; i < 60; i++) {
			float alpha = (float)i / 60;
			fadeToBlackSprite.GetComponent<SpriteRenderer>().material.color = new Color(Color.white.r, Color.white.g, Color.white.b, alpha);
			yield return 0;
		}
	}

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

		if (allowRotation == true) { // only allow rotation after the earth has exploded
			transform.Rotate(new Vector3(v, h, 0));
		}

//		Debug.Log ("v:" + v + " h:" + h + " rotation distance:" + Vector2.SqrMagnitude(new Vector2(centerEyeAnchor.localRotation.x, centerEyeAnchor.localRotation.y)));
		// slow and speed up based on turning if there is no controller input selected
		if (useWithNoController == true) {
			if ((Mathf.Abs(centerEyeAnchor.localRotation.x) > 0.1f) || (Mathf.Abs(centerEyeAnchor.localRotation.y) > 0.1f)) {
				// slow down while turning if there is no controller input selected
				movementSpeed -= thrustSensitivity * Time.deltaTime * 4f;
				if (movementSpeed < speedWhileTurning) {
					movementSpeed = speedWhileTurning;
				}
				fuelObj.usageRate = LOW_USAGE;
			} else if (Vector2.SqrMagnitude(new Vector2(centerEyeAnchor.localRotation.x, centerEyeAnchor.localRotation.y)) < 0.02f) {
				// speed up while looking straight ahead.
				movementSpeed += thrustSensitivity * Time.deltaTime;
				if (movementSpeed > maxSpeed) {
					movementSpeed = maxSpeed;
				}
				fuelObj.usageRate = HIGH_USAGE;
			}
		}

		                        
		// ADD THRUST
		if (allowRotation == true) { // only allow thrust after the earth has exploded
			if (Input.GetButton("Thrust")) {
				movementSpeed += thrustSensitivity * Time.deltaTime;
			}
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
//					Debug.Log("AUTO GRABBING OBJECT!!:" + debris);
					debrisToGrab = debris;
				}
			}
			if (debrisToGrab != null) {
				GrabObject(debrisToGrab);
			}
		}


		// == GAME LOGIC TO SPAWN PARTNER ==
		if (earthHasExploded == true) {
			// Partner timer decrement, once player has control
			if (partnerSpawnTimer < minimumTimeToSpawnPartner) {
				partnerSpawnTimer += Time.deltaTime;
			}
			// Check if it's time to spawn the "partner"
			if ((partner == null) && (partnerSpawnTimer >= minimumTimeToSpawnPartner) && (objectsCollected.Count >= minObjectsBeforeSpawning)) {
				RespawnPartner();
			} else if ((partner != null) && (partnerFound == false) && (partnerSpawnTimer >= minimumTimeToSpawnPartner)) {
				// parter already spawned, check if the player has turned and we need to respawn
				float distFromPartner = Vector3.Distance(partner.position, transform.position);
				if (distFromPartner > 83f) {
					Debug.Log("re spawning parter");
					RespawnPartner();
				}
			}
		}

		// == FAST BREATHING WHEN THE PARTNER IS IN VIEW ==
		if (partner != null) {
			Vector2 posOnScreen = leftEyeAnchor.camera.WorldToScreenPoint(partner.transform.position);
			float dist = Vector2.Distance(posOnScreen, new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
			if (dist < 3.0f) {
				// partner is right in front of us.
				// play fast breathing
				hudVisor.PlayBreathingFast();
			}
		}


		// == SKIP OPENING INTRO SCENE == 
		if (Input.GetButtonUp("DPadUp")) {
			// when the dpad up is pressed, skip the opening scene
			skipOpeningScene = true;
		}
		if (Input.GetButtonUp("DPadLeft")) {
			// when the dpad up is pressed, skip the opening scene
			StartCoroutine("PartnerFound");
		}
		// == EXPLODE OBJECT ==
		// when user presses button close to a debris
//		if( Input.GetButtonUp("Explode")) {
//			BlowUpTheEarth();
//		}
	}
	void RespawnPartner() {
		// spawn partner
		if (partner != null) {
			Destroy(partner.gameObject);
		}
		partner = (Transform)Instantiate(partnerPrefab);
		// position the partner right in front of the player in the distance
		partner.position = transform.position + (centerEyeAnchor.forward * 80);
		partner.rotation = transform.rotation;
		Debug.Log("Spawning Partner!");
	}

	void BlowUpTheEarth() {
		foreach (Transform child in earth.transform) {
			child.rigidbody.AddForceAtPosition(Random.insideUnitSphere * 10000f, child.position);
		}

		// play fast breathing
		hudVisor.PlayBreathingFast ();
	}

	void GrabObject(GameObject debris) {
//		holdingRightHand = closestObjectWithinReach;
		objectsCollected.Add(debris);

		// play sfx
		audio.PlayOneShot (debris.GetComponent<DebrisAudio> ().audioClip);

		debrisWithinReach.Remove(debris);
		
		// tell the debris manager to remove the object
		debrisManager.RemoveDebris(debris.transform);
		
//				Debug.Log("right trigger released, grabbed:" + closestObjectWithinReach);
		// TODO: show inventory icon
		hudVisor.PickedUpObject (debris);
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log ("Trigger Enter! with:" + other.gameObject.tag);

		// track all objects within reach
		if (other.gameObject.tag == "Debris") {
			debrisWithinReach.Add(other.gameObject);
		} else if (other.gameObject.tag == "Partner") {
			// REACHED GOAL OF FINDING PARTNER! GAME WIN!!
			// fade out scene
			Debug.Log("FADING OUT");
			StartCoroutine("PartnerFound");
		}

	}

	void OnCollisionEnter(Collision collision) {
		Debug.Log ("Collision Enter! with:" + collision.gameObject.tag);
	}
	void OnTriggerExit(Collider other) {
//		Debug.Log ("collision EXIT! with:" + other.gameObject.tag);

		// track all objects leaving reach
		if (other.gameObject.tag == "Debris") {
			debrisWithinReach.Remove(other.gameObject);
		}
	}
}
