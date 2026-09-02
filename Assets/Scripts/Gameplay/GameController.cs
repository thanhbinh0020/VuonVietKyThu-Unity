using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VuonVietKyThu {
    public sealed class GameController : MonoBehaviour {
        LevelDatabase db; SaveSystem save; UIManager ui; EffectsController fx; EventSystem events; SfxPlayer sfx;
        LevelDefinition level; TileData[] board; bool[] jellies; Dictionary<int,int> goals=new();
        System.Random rng; int score,moves,selected=-1,fever; bool busy,ended; QualityMode Quality=>save.Data.quality;
        public void Init(LevelDatabase d,SaveSystem s,UIManager u,EffectsController e,EventSystem ev,SfxPlayer audio){db=d;save=s;ui=u;fx=e;events=ev;sfx=audio;}
        public void StartLevel(int id){
            id=Mathf.Clamp(id,1,50);save.RefreshHearts();if(save.Data.hearts<=0){ui.ShowModal("HẾT TIM","Mỗi tim hồi sau 30 phút. Bạn có thể mua hồi đầy tim trong Cửa hàng.",new[]{("CỬA HÀNG",(Action)ui.ShowShop),("ĐÓNG",(Action)ui.ShowHome)});return;}level=db.Get(id);rng=new System.Random(id*7919+DateTime.Now.Millisecond);board=Match3Logic.Generate(level,id*7919);jellies=new bool[64];goals=level.goals.ToDictionary(g=>g.fruitType,g=>g.count);score=0;moves=level.moves+(id<=10?Mathf.Min(5,save.Data.failCounts[id-1]*2):0);selected=-1;fever=0;busy=false;ended=false;
            PlaceJellies(level.jelly,id);PlaceStarterSpecials(level.starterSpecials);save.Data.selected=id;save.Save();events.RecordPlayed();ui.ShowGame();Render();ui.ShowTutorial(level.tutorialStep);
        }
        void PlaceJellies(int count,int seed){var rr=new System.Random(seed*104729);var ids=Enumerable.Range(0,64).OrderBy(_=>rr.Next()).Take(Mathf.Clamp(count,0,64));foreach(var i in ids)jellies[i]=true;}
        void PlaceStarterSpecials(int count){if(count<=0)return;int[] preferred={27,36,28,35};for(int i=0;i<count&&i<preferred.Length;i++){int p=preferred[i];board[p].special=i%2==0?SpecialType.RocketRow:SpecialType.Bomb;}}
        public void OnTilePressed(int index){if(busy||ended||board==null)return;if(selected<0){selected=index;Render();StartCoroutine(ui.Tiles[index].Pop(1.08f,.12f));return;}if(index==selected){selected=-1;Render();return;}if(!Match3Logic.Adjacent(selected,index)){selected=index;Render();return;}int a=selected;selected=-1;StartCoroutine(TrySwap(a,index));}
        IEnumerator TrySwap(int a,int b){
            busy=true;Match3Logic.Swap(board,a,b);Render();yield return new WaitForSecondsRealtime(.08f);
            bool special=board[a].special!=SpecialType.None||board[b].special!=SpecialType.None;var info=Match3Logic.FindMatches(board);
            if(!special&&info.cells.Count==0){Match3Logic.Swap(board,a,b);Render();AudioHaptics.Vibrate(8);busy=false;yield break;}
            moves=Mathf.Max(0,moves-1);AudioHaptics.Vibrate(12);
            if(special){var clear=ActivateSwapSpecials(a,b);yield return ClearAndCollapse(clear,1,null,-1);}else yield return ResolveMatches(info,b,1);
            busy=false;CheckEnd();
        }
        HashSet<int> ActivateSwapSpecials(int a,int b){
            var clear=new HashSet<int>{a,b};
            if(board[a].special==SpecialType.Rainbow){int type=board[b].fruit;for(int i=0;i<64;i++)if(board[i].fruit==type)clear.Add(i);}
            if(board[b].special==SpecialType.Rainbow){int type=board[a].fruit;for(int i=0;i<64;i++)if(board[i].fruit==type)clear.Add(i);}
            return ExpandSpecials(clear);
        }
        IEnumerator ResolveMatches(MatchInfo info,int preferred,int combo){
            while(info.cells.Count>0){
                int creationIndex;var creation=Match3Logic.FindSpecial(info,preferred,out creationIndex);var clear=ExpandSpecials(new HashSet<int>(info.cells));
                if(creation!=SpecialType.None&&creationIndex>=0){clear.Remove(creationIndex);if(board[creationIndex]!=null){board[creationIndex].special=creation;score+=500;events.RecordSpecial();StartCoroutine(ui.Tiles[creationIndex].Pop(1.3f,.22f));}}
                yield return ClearAndCollapse(clear,combo,creation,creationIndex);
                Render();yield return new WaitForSecondsRealtime(Quality==QualityMode.Smooth?.05f:.10f);info=Match3Logic.FindMatches(board);combo++;
                if(combo>1&&info.cells.Count>0){GainFever(Mathf.Min(22,6+combo*3));fx.ComboPulse(ui.BoardRect,combo);}
            }
        }
        HashSet<int> ExpandSpecials(HashSet<int> seed){
            var clear=new HashSet<int>(seed);var queue=new Queue<int>(seed);var used=new HashSet<int>();
            while(queue.Count>0){int i=queue.Dequeue();if(i<0||i>=64||board[i]==null||used.Contains(i))continue;used.Add(i);var s=board[i].special;if(s==SpecialType.None)continue;
                if(s==SpecialType.RocketRow){int r=i/8;for(int c=0;c<8;c++)Add(r*8+c);}else if(s==SpecialType.RocketColumn){int c=i%8;for(int r=0;r<8;r++)Add(r*8+c);}else if(s==SpecialType.Bomb){int rr=i/8,cc=i%8;for(int y=-1;y<=1;y++)for(int x=-1;x<=1;x++){int r=rr+y,c=cc+x;if(r>=0&&r<8&&c>=0&&c<8)Add(r*8+c);}}else if(s==SpecialType.Rainbow){int type=board[i].fruit;for(int k=0;k<64;k++)if(board[k]!=null&&board[k].fruit==type)Add(k);}
            }
            return clear;
            void Add(int k){if(clear.Add(k))queue.Enqueue(k);}
        }
        IEnumerator ClearAndCollapse(HashSet<int> clear,int combo,SpecialType? creation,int creationIndex){
            if(clear.Count==0)yield break;Collect(clear);sfx.Play(combo>1?"special":"match");int points=clear.Count*100*Mathf.Max(1,combo);score+=points;var anim=new List<Coroutine>();int cap=Quality==QualityMode.Max3D?18:Quality==QualityMode.Auto?10:6;int n=0;foreach(int i in clear){if(n++<cap)anim.Add(StartCoroutine(ui.Tiles[i].Vanish(Quality==QualityMode.Smooth?.09f:.15f)));}fx.Flash(combo>=3?new Color(1f,.35f,.76f):new Color(.35f,.88f,1f),Quality==QualityMode.Max3D?.22f:.13f);Render();yield return new WaitForSecondsRealtime(Quality==QualityMode.Smooth?.10f:.16f);
            foreach(int i in clear)board[i]=null;Collapse();Refill();Render();yield return new WaitForSecondsRealtime(Quality==QualityMode.Smooth?.06f:.12f);if(!Match3Logic.HasMove(board)){Match3Logic.Shuffle(board,rng);Render();ui.Toast("Đã xáo bàn để tạo nước đi mới");}
        }
        void Collect(HashSet<int> clear){foreach(int i in clear){var t=board[i];if(t==null)continue;if(goals.ContainsKey(t.fruit)&&goals[t.fruit]>0)goals[t.fruit]--;if(jellies[i])jellies[i]=false;}GainFever(Mathf.Min(18,clear.Count+2));}
        void Collapse(){for(int c=0;c<8;c++){int write=7;for(int r=7;r>=0;r--){int i=r*8+c;if(board[i]!=null){int dst=write*8+c;if(dst!=i){board[dst]=board[i];board[i]=null;}write--;}}}}
        int PickFruit(){float assist=level.id<=10?Mathf.Min(.15f,save.Data.failCounts[level.id-1]*.04f):0f;if(goals.Count>0&&rng.NextDouble()<Mathf.Min(.55f,level.goalBias+assist)){var pending=goals.Where(k=>k.Value>0).Select(k=>k.Key).ToArray();if(pending.Length>0)return pending[rng.Next(pending.Length)];}return rng.Next(Mathf.Clamp(level.typeCount,4,6));}
        void Refill(){for(int i=0;i<64;i++)if(board[i]==null)board[i]=new TileData{fruit=PickFruit()};}
        void GainFever(int value){fever=Mathf.Clamp(fever+value,0,100);if(fever>=100){fever=0;score+=1500;fx.Flash(new Color(1f,.76f,.16f),.28f);ui.Toast("BÙNG NỔ VƯỜN +1.500!");AudioHaptics.Vibrate(30);}}
        void CheckEnd(){Render();bool won=goals.All(k=>k.Value<=0)&&jellies.All(x=>!x);if(won){ended=true;int stars=EarnedStars();int reward=120+level.id*8+stars*60;save.CompleteLevel(level.id,stars,reward);events.RecordWin(stars);sfx.Play("win");fx.Flash(new Color(1f,.8f,.22f),.32f);ui.ShowResult(true,level.id,stars,score,reward,()=>StartLevel(Mathf.Min(50,level.id+1)),()=>StartLevel(level.id));return;}if(moves<=0){ended=true;save.LoseHeart();save.RecordFail(level.id);sfx.Play("lose");ui.RefreshHome();ui.ShowResult(false,level.id,0,score,0,()=>{},()=>StartLevel(level.id));}}
        int EarnedStars(){if(level.starScores==null||level.starScores.Length<3)return 1;if(score>=level.starScores[2])return 3;if(score>=level.starScores[1])return 2;return 1;}
        int JellyLeft()=>jellies.Count(x=>x);
        void Render(){if(board==null)return;ui.RenderTiles(board,jellies,selected,Quality);ui.UpdateGame(level,score,moves,goals,JellyLeft(),fever);}
        public void UseBooster(string id){if(busy||ended||board==null)return;int count=id=="basket"?save.Data.inventory.basket:id=="fan"?save.Data.inventory.fan:save.Data.inventory.pinwheel;if(count<=0){ui.Toast("Hết trợ lực — mở Cửa hàng để mua thêm");return;}if(id=="basket")save.Data.inventory.basket--;else if(id=="fan")save.Data.inventory.fan--;else save.Data.inventory.pinwheel--;save.Save();sfx.Play("special");AudioHaptics.Vibrate(14);
            if(id=="pinwheel"){Match3Logic.Shuffle(board,rng);Render();fx.Flash(new Color(.75f,.35f,1f));return;}
            int target=FindGoalTarget();if(target<0)target=rng.Next(64);var clear=new HashSet<int>();if(id=="basket"){clear.Add(target);int r=target/8,c=target%8;foreach(var d in new[]{-8,8,-1,1}){int x=target+d;if(x>=0&&x<64&&(d==1||d==-1?x/8==r:true))clear.Add(x);}}else{int row=target/8;for(int c=0;c<8;c++)clear.Add(row*8+c);}StartCoroutine(BoosterClear(clear));
        }
        IEnumerator BoosterClear(HashSet<int> clear){busy=true;yield return ClearAndCollapse(ExpandSpecials(clear),1,null,-1);busy=false;CheckEnd();}
        int FindGoalTarget(){var pending=goals.Where(k=>k.Value>0).Select(k=>k.Key).ToHashSet();for(int i=0;i<64;i++)if(pending.Contains(board[i].fruit))return i;return -1;}
        public void UseExtraMoves(){if(save.Data.inventory.extraMoves<=0)return;save.Data.inventory.extraMoves--;save.Save();moves=5;ended=false;busy=false;ui.HideModal();Render();ui.Toast("+5 lượt — cố lên!");}
    }
}
