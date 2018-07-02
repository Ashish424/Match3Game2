using UnityEngine;




namespace Match3{

    
    
    public enum Dir{		
        Up,Left,Right,Down,NoDir
    }
    
    
    public static class Utils{
        
        
        

    
        public static float smoothStop2(float factor){
            return 1-(1-factor)*(1-factor);

        }
    
    
        public static float smoothStop4(float factor){

            return 1-(1-factor)*(1-factor)*(1-factor)*(1-factor);

        }
    
        public static float smoothStop8(float factor){
            return 1-(1-factor)*(1-factor)*(1-factor)*(1-factor)*(1-factor)*(1-factor)*(1-factor)*(1-factor);

        }


        public static Dir getSwipeDirection(Vector3 diff,float swipeDelta,float angle){
		            
            
            
            
            if (diff.sqrMagnitude < swipeDelta) return Dir.NoDir;
	
		
            if (angle >= 45 && angle < 135.0f){
                return Dir.Up;
            }
            if (angle >= 135.0f && angle < 225.0f){
		
                return Dir.Left;
            }
            if (angle >= 225.0f && angle < 315.0f){
                return Dir.Down;
            }
            return Dir.Right;
	
        }
        
        
        



    }


}



