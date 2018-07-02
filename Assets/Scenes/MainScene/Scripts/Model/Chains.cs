using System.Collections.Generic;
using System.Linq;

namespace Match3{
    public class Chains{
        
        public Chains(HashSet<TileModel> chainedTiles,int score){
            this.chainedTiles = chainedTiles;
            this.score = score;
        }


        public TileModel[] toArray(){
            return chainedTiles.ToArray();
            
        }

        public void merge(Chains other){
            chainedTiles.UnionWith(other.chainedTiles);
            score += other.score;
        }

        public int getScore(){
            return score;
        }

        private  int score;
        private HashSet<TileModel> chainedTiles;

    }
}