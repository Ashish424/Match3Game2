using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
[DisallowMultipleComponent]
public class StartButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void loadMainScene(){
	
		if (!loading){
			StartCoroutine(_asynchronousLoad("MainScene"));
		}
		
	}
	
	//code reference - https://www.alanzucconi.com/2016/03/30/loading-bar-in-unity/
	private IEnumerator _asynchronousLoad (string scene){

		
		Debug.Log("loading begins");
		loading = true;
		
		yield return null;

		AsyncOperation ao = SceneManager.LoadSceneAsync(scene);
		ao.allowSceneActivation = false;

		while (! ao.isDone)
		{
			// [0, 0.9] > [0, 1]
			float progress = Mathf.Clamp01(ao.progress / 0.9f);
			Debug.Log("Loading progress: " + (progress * 100) + "%");
			
			
			

			// Loading completed
			if (ao.progress == 0.9f)
			{
				Debug.Log("loading is complete");
				ao.allowSceneActivation = true;
			}

			yield return null;
		}

		loading = false;
	}


	private bool loading = false;
	
}
