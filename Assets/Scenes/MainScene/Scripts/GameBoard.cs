using System.Collections;
using System.Collections.Generic;
using Match3;



#if UNITY_EDITOR
using UnityEditor;
#endif


using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;


[DisallowMultipleComponent]
//[RequireComponent(typeof(UserInput))]
public class GameBoard : MonoBehaviour,UserInputEventHandler{




	[SerializeField] private LevelData defaultData;


	private void OnValidate(){
		Assert.IsNotNull(defaultData,"attach a default data for testing");
		
		currentData = defaultData;
		
	}


	private void Awake(){

		myTransform = transform;
		
		
		//note make sure only one component ever
		#if UNITY_ANDROID
			userInput = gameObject.AddComponent<TouchUserInput>();
		#else 
			userInput = gameObject.AddComponent<UserInput>();		
		#endif
		
		

	}

	private void Start(){
		LevelData level = GetComponent<LevelManager>().getCurrentLevel();
		resetGame(level);
	}

	public void resetGame(LevelData gameData){
		
		
		deleteTileActors();

		boardModel = null;
		if(userInput!=null)userInput.unregisterInputHandler(this);
		scoreGui = movesRemaining = 0;

		
		if (gameData != null){
			currentData = gameData;
		}
		else{
			currentData = defaultData;
		}
		

		if (currentData.useSeed){
			Random.InitState(currentData.seed);
		}
		else{
			Random.InitState((int) System.DateTime.Now.Ticks);
			
		}
		
		//controller work
		//create model
		boardModel = new BoardModel(currentData.tileConfigs.Count,currentData.numRows,currentData.numCols,currentData.numMoves,currentData.targetScore,currentData.chainBaseScore);
		createTilesFromModel(boardModel);
		//register user input for view
		userInput.registerInputHandler(this);
		movesRemaining = currentData.numMoves;

	}




	
	
	private void OnGUI(){
		GUIStyle myStyle = new GUIStyle();        
		myStyle.fontSize = 20;
		myStyle.normal.textColor = Color.black;
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Score "+scoreGui,myStyle);	
		GUILayout.Space(20);
		
		GUILayout.Label("Moves Remaining "+movesRemaining,myStyle);
		GUILayout.Space(20);

		
		GUILayout.Label("Target "+currentData.targetScore,myStyle);
		
		GUILayout.EndHorizontal();
		
	}

	IEnumerator _testing(){



		for (int i = 0; i < currentData.numRows; ++i){
			for (int j = 0; j < currentData.numCols; ++j){
			boardModel.getTileModelAt(i, j).AttachedTileActor.setHighlight(Color.white);
				yield return new WaitForSeconds(0.125f);
				boardModel.getTileModelAt(i, j).AttachedTileActor.restoreHighlight();
				
			}

		}
		
		

		yield return null;
	}


	
	
	



	//C# interface requirments :( making mmebers public 
	public void onInputDisable(){
	}

	public void onInputEnable(){
	}
	
	
	
	public void onInputBegin(){
	
		Assert.IsNull(selectedTile);
		TileActor t  = userInput.getActorUnderPointer(20.0f);
		if(t == null)return;
		
//		Debug.Log("selected actor model data "+t.tileModel.ToString());
		selectedTile = t;
		Color32 tC = selectedTile.getColor();
		
		selectedTile.setHighlight(new Color(tC.r,tC.g,tC.b,0.5f));
		
	


	}

	//input state variables	
	private TileActor selectedTile;



	public void onInputHold(){
			
		if(selectedTile == null)return;
		Debug.DrawLine(selectedTile.transform.position,userInput.getPointerPosInWorldSpace(),Color.black);
		
		
		TileActor selected = selectedTile;
		if (selected!=null){
			
			Vector2 diff = getpointerPosInBoardSpace() - (Vector2) selected.transform.localPosition;
			if(diff.sqrMagnitude > currentData.swipeDelta){
				selected.restoreHighlight();

				selectedTile = null;
				float angle = Mathf.Repeat(Vector2.SignedAngle(myTransform.right,diff)+360.0f,360.0f);
				Dir userDir = Match3.Utils.getSwipeDirection(diff,currentData.swipeDelta,angle);
				int otherRow = selected.tileModel.row, otherCol = selected.tileModel.col;
				switch (userDir){
					case Dir.Up:
						++otherRow;
						break;
					case Dir.Down:
						--otherRow;
						break;
					case Dir.Left:
						--otherCol;
						break; 
					case Dir.Right:
						++otherCol;
						break;
					case Dir.NoDir:
						return;
				}
			
				if(otherCol < 0 || otherCol >= currentData.numCols|| otherRow < 0   || otherRow >= currentData.numRows)return;

				TileModel other = boardModel.getTileModelAt(otherRow, otherCol);
				if(other == null || !boardModel.canSwap(selected.tileModel,other))return;
				Assert.IsNotNull(other.AttachedTileActor);
				TileActor otherActor = other.AttachedTileActor;
				//controller updating model,disabling interaction for the view.
				//model work
				boardModel.swapTiles(selected.tileModel,other);
				//view work
				userInput.enabled = false;
			
				StartCoroutine(_tileMovmentAndStabilization(selected,otherActor,currentData.tileMovementTime,onStabilizatonOver));
			}
			
		
		}
		
		

	}
	
	
	
	
	public void onInputEnd(){
	
		TileActor selected = selectedTile;
		//clear state at top
		selectedTile = null;
		if (selected!=null){
			selected.restoreHighlight();


			Vector2 diff = getpointerPosInBoardSpace() - (Vector2) selected.transform.localPosition;
			float angle = Mathf.Repeat(Vector2.SignedAngle(myTransform.right,diff)+360.0f,360.0f);

			Dir userDir = Match3.Utils.getSwipeDirection(diff,currentData.swipeDelta,angle);
			int otherRow = selected.tileModel.row, otherCol = selected.tileModel.col;
			switch (userDir){
				case Dir.Up:
					++otherRow;
					break;
				case Dir.Down:
					--otherRow;
					break;
				case Dir.Left:
					--otherCol;
					break; 
				case Dir.Right:
					++otherCol;
					break;
				case Dir.NoDir:
					return;
			}
			
			if(otherCol < 0 || otherCol >= currentData.numCols|| otherRow < 0   || otherRow >= currentData.numRows)return;




			TileModel other = boardModel.getTileModelAt(otherRow, otherCol);
			if(other == null || !boardModel.canSwap(selected.tileModel,other))return;
			
			Assert.IsNotNull(other.AttachedTileActor);
			
			TileActor otherActor = other.AttachedTileActor;

			//controller updating model,disabling interaction for the view.

			//model work
			boardModel.swapTiles(selected.tileModel,other);
			//view work
			userInput.enabled = false;
			
			StartCoroutine(_tileMovmentAndStabilization(selected,otherActor,currentData.tileMovementTime,onStabilizatonOver));
		
		}
	}
	delegate void OncompleteBoardStabilization();

	private IEnumerator _tileMovmentAndStabilization(TileActor first,TileActor second,float moveTime,OncompleteBoardStabilization callback){
		
		
		float timer = 0;
		if (moveTime <= 0) moveTime = Mathf.Epsilon;


		Vector3 firstPos = first.transform.localPosition;
		Vector3 secondPos = second.transform.localPosition;
		
		//move sprites
		while (timer <= moveTime){
			float factor = Match3.Utils.smoothStop2(timer / moveTime);
			
			first.transform.localPosition = Vector3.Lerp(firstPos,secondPos,factor );
			second.transform.localPosition = Vector3.Lerp(secondPos, firstPos, factor);
			timer += Time.deltaTime;
			yield return null;
		}

		//snap positions
		first.transform.localPosition = secondPos;
		second.transform.localPosition = firstPos;
		yield return null;
		
	
		
		
		

//		yield return StartCoroutine(_testing());


		
		Chains c = boardModel.deleteChains();
		TileModel[] tilesDeleted = c.toArray();

		
		
		//note update gui variables
		--movesRemaining;

		while (tilesDeleted.Length > 0){


			
			//delete chained actors
			yield return StartCoroutine(_deleteTileActors(tilesDeleted, currentData.tileDeathTime));
			
			List<BoardModel.TileShift> [] shift = boardModel.settleTiles();

			//todo updating gui
			scoreGui += c.getScore();

			yield return StartCoroutine(_fallTiles(shift));			

						
			//add new cookies
//			boardModel.print("before adding");
			List<TileModel> addedTiles = boardModel.addTilesTillfull();
//			Debug.Log("tiles added "+addedTiles.Count);
//			boardModel.print("after adding");
			yield return StartCoroutine(_addNewTileActors(addedTiles,currentData.tileScaleTime));
			

			//next iteration setup here
			c = boardModel.deleteChains();
			tilesDeleted = c.toArray();


		}



		
		
		callback();
		
		
		//this is the last thing since this might reset game
		if (checkGameOver()){
			var gameEnd = new LevelManager.GameEndData(scoreGui >= currentData.targetScore && movesRemaining > 0);
			GetComponent<LevelManager>().onGameEnd(gameEnd);
			
		}
		
		
	}


	//note move this to board model
	private bool checkGameOver(){
		return scoreGui >= currentData.targetScore || movesRemaining <= 0;
	}
	
	IEnumerator _addNewTileActors(List<TileModel> addedTiles,float aliveTime){


		if(addedTiles.Count == 0)yield break;
		
		TileActor[] actors = new TileActor[addedTiles.Count];
		
		for (int i = 0; i < addedTiles.Count; ++i){
			actors[i] = createTileActor(addedTiles[i]);
			actors[i].transform.localScale = Vector3.zero;
		}
		
		float timer = 0;
		
		if (aliveTime <= 0)aliveTime = Mathf.Epsilon;
	
		while (timer <= aliveTime){
			for (int i = 0; i < addedTiles.Count; ++i){
				
				float factor = timer / aliveTime;
				
				actors[i].transform.localScale = new Vector3(factor,factor,factor);
				
				
			}
			
			timer += Time.deltaTime;
			yield return null;

		}
		
		yield return null;
		
		//snapping
		for (int i = 0; i < addedTiles.Count; ++i){
				
			actors[i].transform.localScale = Vector3.one;
		
		}

	}

	
	
	IEnumerator _fallTiles(List<BoardModel.TileShift> [] shift){
		
		if(shift.Length == 0)yield break;
		
	
		float timer = 0;
		float settleTime = 0;

		for (int i = 0; i < shift.Length; ++i){

			for (int j = 0; j < shift[i].Count; ++j){
			
				settleTime = Mathf.Max(settleTime, shift[i][j].tileMovement);
			}
			
		}
		Debug.Log("max settle time is "+settleTime);


		List<Vector3> [] initPositions  = new List<Vector3>[shift.Length];
		for (int i = 0; i < shift.Length; ++i){
			initPositions[i] = new List<Vector3>();
			for (int j = 0; j < shift[i].Count; ++j){
				TileModel t = shift[i][j].t;
				Assert.IsTrue(t!=null && t.AttachedTileActor!=null);
				initPositions[i].Add(t.AttachedTileActor.transform.localPosition);
			}
		}
		
		
#if UNITY_EDITOR
		if (!boardModel.isStable()){
			Debug.Break();
		}
#endif
		if (settleTime > Mathf.Epsilon){
			while (timer <= settleTime){


				
//				float factor = timer/settleTime;
			
				for (int i = 0; i < shift.Length; ++i){

					for (int j = 0; j < shift[i].Count; ++j){

						TileModel t = shift[i][j].t;
						Assert.IsTrue(t!=null && t.AttachedTileActor!=null);
						t.AttachedTileActor.transform.localPosition -= new Vector3(0,(Time.deltaTime / settleTime) * currentData.tileSpacing * shift[i][j].tileMovement);
						
					}
			
				}
				        
				timer += Time.deltaTime;
				yield return null;
			}
			
			
		}
		
		
		//snapping positions
		for (int i = 0; i < shift.Length; ++i){
			for (int j = 0; j < shift[i].Count; ++j){
				TileModel t = shift[i][j].t;
				Assert.IsTrue(t!=null && t.AttachedTileActor!=null);
				t.AttachedTileActor.transform.localPosition = initPositions[i][j] - new Vector3(0,currentData.tileSpacing * shift[i][j].tileMovement);
	
			}
		}

		
		
		
		yield return null;

	}

	
	IEnumerator _deleteTileActors(TileModel [] modelsDeleted ,float deathTime){
		
		
	
		if(modelsDeleted.Length == 0)yield break;
	
		float timer = 0;
		if (deathTime <= 0) deathTime = Mathf.Epsilon;
	
		while (timer <= deathTime){
			for (int i = 0; i < modelsDeleted.Length; ++i){
				TileActor act = modelsDeleted[i].AttachedTileActor;
				Color c = act.getColor();
				act.setColor(new Color(c.r,c.g,c.b,1-timer/deathTime));				
			}
			timer += Time.deltaTime;
			yield return null;

		}
		yield return null;
		//actual deletion
		for (int i = 0; i < modelsDeleted.Length; ++i){
			Assert.IsTrue(modelsDeleted[i].isOffBorad());
			TileActor act = modelsDeleted[i].AttachedTileActor;
			Destroy(act.gameObject);
			modelsDeleted[i].AttachedTileActor = null;
			
		}
		
	}
	
	
	

	void onStabilizatonOver(){
		userInput.enabled = true;
	}
	

	
	//returns Vector2 to get rid of z component
	private Vector2 getpointerPosInBoardSpace(){
		return transform.InverseTransformPoint(userInput.getPointerPosInWorldSpace());
	}


	private void createTilesFromModel(BoardModel model){
		
		
		for (int i = 0; i < currentData.numRows; ++i){
			for (int j = 0; j < currentData.numCols; ++j){
				TileModel m = model.getTileModelAt(i,j);
				createTileActor(m);
			}	
		}
	}

	private void deleteTileActors(){
		if(boardModel == null)return;
		
		for (int i = 0; i < currentData.numRows; ++i){
			for (int j = 0; j < currentData.numCols; ++j){
				deleteTileActor(boardModel.getTileModelAt(i, j));
			}	
		}
		
	}


	private void deleteTileActor(TileModel tileModel){
		
		Assert.IsTrue(tileModel!=null && !tileModel.isOffBorad() && tileModel.AttachedTileActor!=null);

		
		Destroy(tileModel.AttachedTileActor.gameObject);
		tileModel.AttachedTileActor = null;
		

	}

		


	private TileActor createTileActor(TileModel tileModel){
		Assert.IsTrue(tileModel!=null && !tileModel.isOffBorad());


		float halfRows = (currentData.numRows-1)*currentData.tileSpacing/2.0f;
		float halfCols = (currentData.numCols-1)*currentData.tileSpacing/2.0f;
		
		
	

		TileActor currentTile = Instantiate(currentData.tileInstance,Vector3.zero, Quaternion.identity,myTransform);
		//updating visual 
		currentTile.tileModel = tileModel;
		currentTile.name = tileModel.row + "," + tileModel.col;
		currentTile.transform.localPosition = new Vector3(tileModel.col*currentData.tileSpacing-halfCols, tileModel.row * currentData.tileSpacing-halfRows);
		currentTile.setupFromConfig(currentData.tileConfigs[tileModel.type]);
		
		
		
		
		//linking visual and model
		Assert.IsNull(tileModel.AttachedTileActor);
		tileModel.AttachedTileActor = currentTile;
		return currentTile;
		
	}

	private void OnDestroy(){
		boardModel = null;
		if(userInput!=null)userInput.unregisterInputHandler(this);
		userInput = null;
		myTransform = null;
	}


	private void OnDrawGizmos(){


		
		float boardWidth = (currentData.numCols - 1) * currentData.tileSpacing+currentData.tileInstance.getWidth();
		float boardHeight =(currentData.numRows - 1) * currentData.tileSpacing+currentData.tileInstance.getHeight();
		

		Gizmos.color = Color.blue;
		//note assumes tiles are centered at this gameobject
		Gizmos.DrawWireCube(transform.position,new Vector3(boardWidth,boardHeight,10));
		
	}



	
	private LevelData currentData;

	
	private BoardModel boardModel;
	private UserInput userInput;	
	private Transform myTransform;

	private int scoreGui;
	private int movesRemaining;




}
