using System;
using UnityEngine;

namespace VuonVietKyThu {
    public sealed class EventSystem {
        readonly SaveSystem save; static readonly int[] Milestones={40,100,200,350};
        public EventSystem(SaveSystem s){save=s;Normalize();}
        string Day()=>DateTime.Now.ToString("yyyy-MM-dd");
        string Week(){var now=DateTime.Now;var d=(int)now.DayOfWeek;var monday=now.Date.AddDays(d==0?-6:1-d);return monday.ToString("yyyy-MM-dd");}
        public EventSave Data{get{Normalize();return save.Data.events;}}
        public void Normalize(){
            if(save.Data.events==null)save.Data.events=new EventSave();var e=save.Data.events;string day=Day(),week=Week();
            if(e.missionDay!=day){e.missionDay=day;e.missionPlayed=e.missionWins=e.missionSpecials=0;e.claimPlayed=e.claimWins=e.claimSpecials=false;}
            if(e.weekKey!=week){e.weekKey=week;e.festivalPoints=0;e.milestoneClaims=new bool[4];}
            if(e.milestoneClaims==null||e.milestoneClaims.Length!=4)e.milestoneClaims=new bool[4];
        }
        public void RecordPlayed(){Normalize();save.Data.events.missionPlayed++;save.Data.events.festivalPoints+=2;save.Save();}
        public void RecordWin(int stars){Normalize();save.Data.events.missionWins++;save.Data.events.festivalPoints+=5+stars*2;save.Save();}
        public void RecordSpecial(){Normalize();save.Data.events.missionSpecials++;save.Data.events.festivalPoints+=1;save.Save();}
        public bool ClaimDaily(out string message){Normalize();var e=save.Data.events;string day=Day();if(e.dailyLast==day){message="Hôm nay đã điểm danh";return false;}var yesterday=DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd");e.dailyStreak=e.dailyLast==yesterday?(e.dailyStreak%7)+1:1;e.dailyLast=day;int[] coins={100,150,200,220,250,300,500};int reward=coins[e.dailyStreak-1];save.Data.coins+=reward;if(e.dailyStreak==4)save.Data.inventory.basket++;if(e.dailyStreak==6)save.Data.inventory.fan++;if(e.dailyStreak==7){save.Data.inventory.basket++;save.Data.inventory.fan++;save.Data.inventory.pinwheel++;}e.festivalPoints+=5;save.Save();message=$"Ngày {e.dailyStreak}: +{reward} xu";return true;}
        public bool ClaimChest(out string message){Normalize();var e=save.Data.events;long now=DateTime.UtcNow.Ticks;if(e.chestReadyTicks>now){var left=new TimeSpan(e.chestReadyTicks-now);message=$"Rương mở lại sau {left.Hours:00}:{left.Minutes:00}";return false;}e.chestReadyTicks=DateTime.UtcNow.AddHours(4).Ticks;int pick=(DateTime.Now.DayOfYear+DateTime.Now.Hour/4)%3;save.Data.coins+=150;if(pick==0)save.Data.inventory.basket++;else if(pick==1)save.Data.inventory.fan++;else save.Data.inventory.pinwheel++;e.festivalPoints+=4;save.Save();message="Rương: +150 xu + 1 trợ lực";return true;}
        public bool ClaimMission(string id,out string message){Normalize();var e=save.Data.events;bool ok=false;if(id=="played"&&e.missionPlayed>=3&&!e.claimPlayed){e.claimPlayed=true;save.Data.coins+=120;ok=true;message="+120 xu";}else if(id=="wins"&&e.missionWins>=1&&!e.claimWins){e.claimWins=true;save.Data.coins+=180;ok=true;message="+180 xu";}else if(id=="specials"&&e.missionSpecials>=3&&!e.claimSpecials){e.claimSpecials=true;save.Data.inventory.pinwheel++;ok=true;message="+1 Chong chóng";}else{message="Chưa đủ điều kiện hoặc đã nhận";}if(ok){e.festivalPoints+=5;save.Save();}return ok;}
        public bool ClaimMilestone(int index,out string message){Normalize();index=Mathf.Clamp(index,0,3);var e=save.Data.events;if(e.festivalPoints<Milestones[index]||e.milestoneClaims[index]){message="Mốc chưa đạt hoặc đã nhận";return false;}e.milestoneClaims[index]=true;if(index==0){save.Data.coins+=200;message="+200 xu";}else if(index==1){save.Data.inventory.basket+=2;message="+2 Giỏ tre";}else if(index==2){save.Data.coins+=450;save.Data.inventory.fan++;message="+450 xu +1 Quạt";}else{save.Data.coins+=800;save.Data.inventory.basket+=2;save.Data.inventory.fan+=2;save.Data.inventory.pinwheel+=2;message="+800 xu + bộ trợ lực";}save.Save();return true;}
        public string ChestLabel(){Normalize();long left=save.Data.events.chestReadyTicks-DateTime.UtcNow.Ticks;if(left<=0)return "RƯƠNG 4 GIỜ - NHẬN NGAY";var t=new TimeSpan(left);return $"RƯƠNG 4 GIỜ - {t.Hours:00}:{t.Minutes:00}";}
        public int[] MilestoneValues()=>Milestones;
    }
}
