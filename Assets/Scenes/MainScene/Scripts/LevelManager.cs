using System;
using System.Collections;
using System.Collections.Generic;
using Match3;
using UnityEngine;
using UnityEngine.Advertisements;


[DisallowMultipleComponent]
[RequireComponent(typeof(GameBoard))]
public class LevelManager : MonoBehaviour{
    
    
    
    [SerializeField] private LevelList levelList;
    [Range(0,100)][SerializeField] private int maxTries;

    [SerializeField] private AdPanelActor adPanelActor;
    

    private void Awake(){

        
        
        Debug.Log("max tries is "+maxTries);
        
        gB = GetComponent<GameBoard>();
        

        
        if ( (currentTries = PlayerPrefs.GetInt(triesRemainingString, -1)) == -1){
            //this happens for the first time for a game installed on device.
            currentTries = maxTries;
        }
        
        
        if ((currentLevel = PlayerPrefs.GetInt(currentLevelString, -1)) == -1){
            currentLevel = 0;
        }
        else if (currentLevel > levelList.levels.Count){
            currentLevel = 0;
            Debug.Log("current level "+currentLevel);
        }

        //disable ad panel by default
        adPanelActor.gameObject.SetActive(false);
        
    }

    private void Start(){
        Debug.Log("current tries "+currentTries);
        Debug.Log("current level "+currentLevel);
        
        if (currentTries <= 0){
            //throw a blocking widget in front prompting for ad.
            StartCoroutine(_blockGame());
            
        }
    }



    public LevelData getCurrentLevel(){
        
        return levelList.levels[currentLevel];
    }


    IEnumerator _blockGame(){
        
        
        Debug.Log("in block game ");
        
        savePlayerState();
        adPanelActor.gameObject.SetActive(true);
        //disable user input for game board
        
        
        GetComponent<UserInput>().enabled = false;

        //wait till the user completes ad
        while (currentTries <=  0){

//            Debug.Log("stuck here ");
            yield return null;
        }
        
        //set back to non blocking state
        adPanelActor.gameObject.SetActive(false);
        GetComponent<UserInput>().enabled = true;
        savePlayerState();
        
    }

    void onAdEndCallback(ShowResult result){

        //on callback see state of ad completion
        //if ad complete,then reset stuff
        //add max tries
        //resetGame @ current Level
        //else  

        
        if (result == ShowResult.Failed || result == ShowResult.Skipped){
            //retry ad display here till successful.
        }

        else if(result == ShowResult.Finished){
            //finished case here
            currentTries = maxTries;
            
            gB.resetGame(levelList.levels[currentLevel]);
            Debug.Log("this ad is over "+result);

        }
        
        savePlayerState();

        
    }
    
    public void playAd(){
        
        
        ShowOptions options = new ShowOptions();
        options.resultCallback = onAdEndCallback;
        
        Advertisement.Show("rewardedVideo",options);
        
        
    }



    public class GameEndData{

        public GameEndData(bool won){
            this.won = won;
        }
        public readonly bool won;
    }
    public void onGameEnd(GameEndData gameEndData){

        
        if (gameEndData.won){
            Debug.Log("won game");
            //increment game index
            //load the selected preset   
            
            currentLevel = (currentLevel + 1) % levelList.levels.Count;

            
            savePlayerState();
            gB.resetGame(levelList.levels[currentLevel]);
        }
        else{

            currentTries = Mathf.Max(0, currentTries - 1);
            
            //game loss
            if (currentTries > 0){

                --currentTries;
                Debug.Log("using tries remaining is " + currentTries);
                gB.resetGame(levelList.levels[currentLevel]);
            }
            else{
                Debug.Log("trying to show ads here");
                //show add here

                currentTries = 0;
                StartCoroutine(_blockGame());

            }


        }
    }

    private void OnDestroy(){
        levelList = null;
        gB = null;
    }
    
    private GameBoard gB;
    private int currentTries;
    private int currentLevel;
    private const string currentLevelString = "currentLevel";
    private const string triesRemainingString = "currentLevel";

    
    
    
    void savePlayerState(){
        
        
        Debug.Log("this is tries remaining "+currentTries);
        Debug.Log("this is current level "+currentLevel);

        PlayerPrefs.SetInt(currentLevelString,currentLevel);
        PlayerPrefs.SetInt(triesRemainingString,currentTries);
        
    }
    
    
    
    
    //Unity State management Testing

    private void Update(){


        if (Debug.isDebugBuild){
            if (Input.GetKeyUp(KeyCode.Semicolon)){

                adPanelActor.gameObject.SetActive(!adPanelActor.gameObject.activeSelf);
                
            }

        }

        if (Input.GetKeyUp(KeyCode.Escape)){
			
			
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call<bool>("moveTaskToBack", true);
            }
            else{
                Application.Quit();
            }
        }
    }

    private void OnApplicationFocus(bool hasFocus){
        Debug.Log("in foucs +"+ hasFocus);


        if (!hasFocus){
            
            savePlayerState();
        }
    }


    private void OnApplicationPause(bool pauseStatus){

        Debug.Log("in pause + "+pauseStatus);

        //app paused here save data
        if (pauseStatus){
            savePlayerState();
        }
        //app resumes here
        else{
         Debug.Log("app resume here");   
        }
    }
    private void OnApplicationQuit(){
        Debug.Log("app quit");
    }
}
