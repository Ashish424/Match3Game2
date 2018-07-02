using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[DisallowMultipleComponent]
public class AdPanelActor : MonoBehaviour{
	

	
	private void Awake(){
		adButton = transform.Find("Button").GetComponent<Button>();
		
		
	}

	private void OnEnable(){
		Debug.Log("this is enable");
	}
	private void OnDisable(){
		Debug.Log("this is disable");

	}
	
	private Button adButton;
	
	
	
}
