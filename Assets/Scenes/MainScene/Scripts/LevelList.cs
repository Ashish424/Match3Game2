using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu]
public class LevelList:ScriptableObject{
    private void OnValidate(){
        
    }

    public List<LevelData> levels;
    

}
