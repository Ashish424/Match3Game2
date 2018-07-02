using UnityEngine;
using UnityEngine.Assertions;

namespace Match3{
    
    
    public interface UserInputEventHandler{

        void onInputBegin();
        void onInputHold();
        void onInputEnd();
        void onInputDisable();
        void onInputEnable();
        
    }
    
    [DisallowMultipleComponent]
    public class UserInput:MonoBehaviour{
        protected virtual void Awake(){
            
            mainCamera = Camera.main;
            
            
        }

        protected virtual void Update(){
           
                
                //process input
                if (Input.GetMouseButtonDown(0)){

                    if(handler!=null)
                        handler.onInputBegin();

                }
                else if (Input.GetMouseButtonUp(0)){
                    if(handler!=null)
                        handler.onInputEnd();


                }
                else if (Input.GetMouseButton(0)){
                    if(handler!=null)
                        handler.onInputHold();
                    
                }

        }
        

        private void OnEnable(){
            if(handler!=null)handler.onInputEnable();
        }
        private void OnDisable(){
            if(handler!=null)handler.onInputDisable();
        }
        


        public void registerInputHandler(UserInputEventHandler i){

            Assert.IsNull(handler,"cannot register more than one handler");
            handler = i;

        }
        public void unregisterInputHandler(UserInputEventHandler i ){
            Assert.IsTrue(handler == i || handler == null,"no handler registered");
            handler = null;
            

        }
        public virtual Vector2 getPointerPosInWorldSpace(){
            return mainCamera.ScreenToWorldPoint(Input.mousePosition);

        }

        public virtual TileActor getActorUnderPointer(float rayDistance){
            
		
            Ray r = mainCamera.ScreenPointToRay(Input.mousePosition);
	        RaycastHit2D hit = Physics2D.Raycast(r.origin,r.direction,rayDistance);

            if (hit){
                TileActor hitted = hit.transform.GetComponent<TileActor>();
                return hitted;
            }

            return null;
        }

        protected virtual void OnDestroy(){
            handler = null;
            
        }
        
        protected UserInputEventHandler handler;
        protected Camera mainCamera;
    }
    
    
}