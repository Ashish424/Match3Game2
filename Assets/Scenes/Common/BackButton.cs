using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BackButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetKeyUp(KeyCode.Escape)){

			#if UNITY_ANDROID
				AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
				activity.Call<bool>("moveTaskToBack", true);
			#else
				Application.Quit();
			#endif
		}

		
	}
}
