using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class LevelData:ScriptableObject{


    
    [Serializable]
    public class TileConfig{
        [ReadOnlyWhenPlaying]public Color color;
	    [ReadOnlyWhenPlaying]public Sprite sprite;

    }


	
	private void OnValidate(){
		//note add all game related constraints here
		if (numRows < 3) numRows = 3;
		if (numCols < 3) numCols = 3;
		if (tileConfigs.Count < 3){
			int addMore = 3 - tileConfigs.Count;
			for (int i = 0; i < addMore; ++i){
				tileConfigs.Add(null);
			}
		}
	}
	
	[Header("View Data")]
	
	[ReadOnlyWhenPlaying] [Range(0, 100)] public float tileSpacing;
	[ReadOnlyWhenPlaying] public TileActor tileInstance;
    [ReadOnlyWhenPlaying] public List<TileConfig> tileConfigs;

	
	
	[Header("GamePlay Data")]
	[ReadOnlyWhenPlaying] public float swipeDelta = 0.5f;
	[ReadOnlyWhenPlaying] public float tileScaleTime = 0.5f;
	[ReadOnlyWhenPlaying] public float tileMovementTime = 0.5f;
	[ReadOnlyWhenPlaying] public float tileDeathTime = 0.5f;


	[Header("Model Data")] 
    [ReadOnlyWhenPlaying] public int numRows;
    [ReadOnlyWhenPlaying] public int numCols;
    [ReadOnlyWhenPlaying] public int numMoves;
    [ReadOnlyWhenPlaying] public int targetScore;
    [ReadOnlyWhenPlaying] public int chainBaseScore;
    [ReadOnlyWhenPlaying] public bool useSeed;
    [ReadOnlyWhenPlaying] public int seed;
	
}
