using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public AudioSource MpPlayer;
	public AudioClip FirstClip;
	public AudioClip SecondClip;
	// Use this for initialization
	void Start () {
		MpPlayer.clip = FirstClip;
		MpPlayer.loop = false;
		MpPlayer.Play();
		StartCoroutine(WaitForTrackTOend());
	}
	
	IEnumerator WaitForTrackTOend()
	{
		while (MpPlayer.isPlaying)
		{
			
			yield return new WaitForSeconds(0.01f);
			
		}
		MpPlayer.clip = FirstClip;
		MpPlayer.loop = true ;
		MpPlayer.Play();
		
	}
	
}
