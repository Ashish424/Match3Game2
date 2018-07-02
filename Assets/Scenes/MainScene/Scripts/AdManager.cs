using System;
using UnityEngine;


using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour {
	private void Awake(){
		
		Debug.Log("awaked");

		//done automatically when using services
		if (Advertisement.isInitialized){
			Debug.Log("here already init done");
		}
		
	}


	public void showAd(ShowOptions options){
		
		Advertisement.Show("rewardedVideo",options);
		
	}
	
	
	
}
