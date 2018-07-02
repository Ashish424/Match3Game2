namespace Match3{
    public class TileModel{




        public TileModel(int row, int col, int type){
            this.row = row;
            this.col = col;
            this.type = type;
        }



        public bool isOffBorad(){
            return row == -1 && col == -1 && type == -1;
        }

        public void clearBoardData(){
            row = col = type = -1;
        }

        public int row = -1;
        public int col = -1;
        public int type = -1;



        public TileActor AttachedTileActor{
            get{ return tileActor; }

            set{ tileActor = value; }
        }

        private TileActor tileActor;



        public static void swapBoardInfo(TileModel t1, TileModel t2){


            int temp = t1.row;
            t1.row = t2.row;
            t2.row = temp;
            temp = t1.col;
            t1.col = t2.col;
            t2.col = temp;


        }

        public override string ToString(){
            return "row is " + row + " col is " + col + " type is " + type;
        }



    }
}