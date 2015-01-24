using UnityEngine;
using System.Collections.Generic;

public class SpaceDustManager : MonoBehaviour {
	
	public int numberOfDustOnScreen = 300;
	public float generateDistance = 300f;
	public Transform dustPrefab;
	
	public Transform playerObject;

	// Use this for initialization
	void Start () {
		// init list
		allDebris = new List<Transform> ();
		
		// at start instantiate a bunch of objects
		for (int i = 0; i < numberOfDustOnScreen; i++) {
			CreateInitialDebris();
		}
	}

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
		Transform debris = (Transform)Instantiate(dustPrefab);
		debris.parent = this.transform;
		allDebris.Add(debris);

		return debris;
	}
	
	void CreateInitialDebris() {
		Transform debris = NextDebris ();

		// random position
		debris.transform.position = (Random.insideUnitSphere * generateDistance) + playerObject.position;
	}
	
	void CreateDebrisOnEdge() {
		Transform debris = NextDebris ();
		
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
}

