using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VuonVietKyThu {
    public sealed class MatchRun { public bool horizontal; public int fruit; public List<int> cells=new(); }
    public sealed class MatchInfo { public HashSet<int> cells=new(); public List<MatchRun> runs=new(); }

    public static class Match3Logic {
        public const int Size=8;
        public static bool Adjacent(int a,int b){int ar=a/Size,ac=a%Size,br=b/Size,bc=b%Size;return Mathf.Abs(ar-br)+Mathf.Abs(ac-bc)==1;}
        public static void Swap(TileData[] b,int a,int c){(b[a],b[c])=(b[c],b[a]);}
        public static MatchInfo FindMatches(TileData[] board){
            var info=new MatchInfo();
            for(int r=0;r<Size;r++){
                int start=0;
                for(int c=1;c<=Size;c++){
                    int first=board[r*Size+start].fruit; bool same=c<Size && board[r*Size+c].fruit==first;
                    if(same)continue; int len=c-start;
                    if(len>=3){var run=new MatchRun{horizontal=true,fruit=first};for(int x=start;x<c;x++){int i=r*Size+x;run.cells.Add(i);info.cells.Add(i);}info.runs.Add(run);} start=c;
                }
            }
            for(int c=0;c<Size;c++){
                int start=0;
                for(int r=1;r<=Size;r++){
                    int first=board[start*Size+c].fruit; bool same=r<Size && board[r*Size+c].fruit==first;
                    if(same)continue; int len=r-start;
                    if(len>=3){var run=new MatchRun{horizontal=false,fruit=first};for(int y=start;y<r;y++){int i=y*Size+c;run.cells.Add(i);info.cells.Add(i);}info.runs.Add(run);} start=r;
                }
            }
            return info;
        }
        public static SpecialType FindSpecial(MatchInfo info,int preferred,out int index){
            index=-1;
            for(int a=0;a<info.runs.Count;a++)for(int b=a+1;b<info.runs.Count;b++){
                var x=info.runs[a];var y=info.runs[b];if(x.horizontal==y.horizontal||x.fruit!=y.fruit)continue;
                int cross=x.cells.FirstOrDefault(i=>y.cells.Contains(i)); if(y.cells.Contains(cross)){index=cross;return SpecialType.Bomb;}
            }
            var five=info.runs.FirstOrDefault(r=>r.cells.Count>=5); if(five!=null){index=five.cells.Contains(preferred)?preferred:five.cells[five.cells.Count/2];return SpecialType.Rainbow;}
            var four=info.runs.FirstOrDefault(r=>r.cells.Count==4); if(four!=null){index=four.cells.Contains(preferred)?preferred:four.cells[1];return four.horizontal?SpecialType.RocketRow:SpecialType.RocketColumn;}
            return SpecialType.None;
        }
        public static bool HasMove(TileData[] board){
            for(int r=0;r<Size;r++)for(int c=0;c<Size;c++){
                int a=r*Size+c;
                if(c+1<Size){int b=a+1;Swap(board,a,b);bool ok=FindMatches(board).cells.Count>0;Swap(board,a,b);if(ok)return true;}
                if(r+1<Size){int b=a+Size;Swap(board,a,b);bool ok=FindMatches(board).cells.Count>0;Swap(board,a,b);if(ok)return true;}
            }return false;
        }
        static int PickFruit(LevelDefinition level,System.Random rng){
            if(level.goals!=null && level.goals.Count>0 && rng.NextDouble()<level.goalBias) return level.goals[rng.Next(level.goals.Count)].fruitType;
            return rng.Next(Mathf.Clamp(level.typeCount,4,6));
        }
        public static TileData[] Generate(LevelDefinition level,int seed){
            var rng=new System.Random(seed);
            for(int attempt=0;attempt<400;attempt++){
                var board=new TileData[Size*Size];
                for(int r=0;r<Size;r++)for(int c=0;c<Size;c++){
                    int fruit,guard=0;
                    do{fruit=PickFruit(level,rng);guard++;}while(guard<30 && ((c>=2&&board[r*Size+c-1].fruit==fruit&&board[r*Size+c-2].fruit==fruit)||(r>=2&&board[(r-1)*Size+c].fruit==fruit&&board[(r-2)*Size+c].fruit==fruit)));
                    board[r*Size+c]=new TileData{fruit=fruit};
                }
                if(HasMove(board))return board;
            }
            throw new Exception("Không tạo được bàn hợp lệ");
        }
        public static void Shuffle(TileData[] board,System.Random rng){
            for(int attempt=0;attempt<120;attempt++){
                for(int i=board.Length-1;i>0;i--){int j=rng.Next(i+1);Swap(board,i,j);}
                if(FindMatches(board).cells.Count==0 && HasMove(board))return;
            }
        }
    }
}
