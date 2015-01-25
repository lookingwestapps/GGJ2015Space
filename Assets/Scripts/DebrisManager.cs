using UnityEngine;
using System.Collections.Generic;

public class DebrisManager : MonoBehaviour {

	public int numberOfDebrisOnScreen = 30;
	public float generateDistance = 20f;
	private int currentPrefabIndex = 0;

	public Transform playerObject;

	// Use this for initialization
	void Start () {
		// init list
		allDebris = new List<Transform> ();

		// at start instantiate a bunch of objects
//		for (int i = 0; i < numberOfDebrisOnScreen; i++) {
//			CreateInitialDebris();    // now triggered in PlayerSpaceController after earth explodes
//		}
	}

	public Transform[] allDebrisPrefabs;
	private List<Transform> allDebris;

	// Update is called once per frame
	void Update () {
		// check if any object has left the radius
		List<Transform> objectsToDelete = new List<Transform> ();
		foreach (Transform trans in allDebris) {
			float dist = Vector3.Distance(playerObject.position, trans.position);
			if (dist > (generateDistance)) {
				// object has moved away,
				// delete object
				objectsToDelete.Add(trans);
				Destroy(trans.gameObject);

//				Debug.Log("object leaving!");
			}
		}

		foreach (Transform trans in objectsToDelete) {
			allDebris.Remove(trans);
			// create another object
			CreateDebrisOnEdge();
		}
	}

	Transform NextDebris() {
		Transform prefabToUse = allDebrisPrefabs[currentPrefabIndex];
		Transform debris = (Transform)Instantiate(prefabToUse);
		debris.parent = this.transform;
		allDebris.Add(debris);

		// inc prefab counter
		currentPrefabIndex++;
		if (currentPrefabIndex >= allDebrisPrefabs.Length) {
			currentPrefabIndex = 0;
		}
		return debris;
	}

	public void CreateInitialDebris() {
		for (int i = 0; i < numberOfDebrisOnScreen; i++) {
			Transform debris = NextDebris ();

			// start debris spinning
			debris.rigidbody.AddTorque (Random.insideUnitSphere * 100);
			
			// random position
			debris.transform.position = (Random.insideUnitSphere * generateDistance) + playerObject.position;
		}
	}

	void CreateDebrisOnEdge() {
		Transform debris = NextDebris ();

		// start debris spinning
		debris.rigidbody.AddTorque (Random.insideUnitSphere * 100);

		// random position
		debris.transform.position = (Random.onUnitSphere * generateDistance) + playerObject.position;
	}

	public void RemoveDebris(Transform debris) {
		allDebris.Remove (debris);

		// destroy object
		Destroy (debris.gameObject);

		// create another object
		CreateDebrisOnEdge();
	}
	public void RemoveAllDebris() {
		foreach (Transform debris in allDebris) {
			// destroy object
			Destroy (debris.gameObject);
		}
		allDebris.Clear();
	}
}
