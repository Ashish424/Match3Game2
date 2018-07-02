using Match3;
using UnityEngine;
using UnityEngine.Assertions;


[DisallowMultipleComponent]
public class TileActor:MonoBehaviour{




    public TileModel tileModel;


    private void OnValidate(){
        
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    

    public float getWidth(){

            
        return mySpriteRenderer.bounds.size.x;
        
    }


    public float getHeight(){
        return mySpriteRenderer.bounds.size.y;
    }
    public void setHighlight(Color selectedColor){
        
        
        Assert.IsTrue(!highlighted,"Clear prev highlight");
        
        mySpriteRenderer.color = selectedColor;

        highlighted = true;
       
    }
    
    public void restoreHighlight(){
        
        Assert.IsTrue(highlighted,"Restoring color without highlighting");
        
        
        mySpriteRenderer.color = myColor;
        highlighted = false;
        
        
    }
    
    
    
    private void Awake(){


        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myColor = mySpriteRenderer.color;

    }


    private void Start(){
        Assert.IsNotNull(tileModel,"attach a tile model");
    }


    public void setColor(Color color){


        myColor = mySpriteRenderer.color = color;
       

    }





    
    //only for display
    private Color myColor;
    private SpriteRenderer mySpriteRenderer;
    
    
    
    //state variables
    public bool highlighted = false;


    public Color getColor(){
        return myColor;
    }

    private void OnDestroy(){
        tileModel = null;
        
    
    }

    public void setupFromConfig(LevelData.TileConfig tileConfig){
        setColor(tileConfig.color);

        mySpriteRenderer.sprite = tileConfig.sprite;
        

    }
}
