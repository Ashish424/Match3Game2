using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCoroutine : MonoBehaviour {
	private void Awake(){
		StartCoroutine(test());
	}

	// Use this for initialization
	void Start () {
		Debug.Log("in start");
	}

	private void Update(){

		bool thisFrame = false;
		Debug.Log("in update");


		if (Input.GetKeyDown(KeyCode.Space)){
			thisFrame = true;

		}
		
		if (Input.GetKey(KeyCode.Space)){

			if (thisFrame){
				Debug.Log("together");
			}
		}
	}

	IEnumerator test(){

		Debug.Log("here before yield");
		yield return null;
		Debug.Log("after yield");
	}
	
}
