using UnityEngine;
using System.Collections;

public class Earth : MonoBehaviour {
	public GameObject earth;
	public GameObject explode;
	public GameObject comet;
	public Transform centerEyeAnchor; // a link to the center eye anchor

	public float distance = .3f;
	private Vector3 placement = new Vector3();
	private GameObject myEarth;
	private GameObject myExplosion;


	// Use this for initialization
	void Start () {
		//placement = transform.position + centerEyeAnchor.forward * distance;
		placement = transform.position + new Vector3(0,0,1000); // 1000 looks good

		//placement.position = transform.position + centerEyeAnchor.forward * distance;
		myEarth = (GameObject)Instantiate(earth, placement, Quaternion.identity);
		myEarth.transform.localScale = new Vector3(10,10,10);
	}
	public void doExplode(){
		myExplosion = (GameObject)Instantiate(explode, myEarth.transform.position, Quaternion.identity);
		myExplosion.transform.localScale = new Vector3(200,200,200);
	}
}
