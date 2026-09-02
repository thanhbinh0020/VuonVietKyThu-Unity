using System;
using UnityEngine;

namespace VuonVietKyThu {
    public sealed class SaveSystem {
        const string Key="vvkt.unity.save.v2";
        public SaveData Data { get; private set; }
        public bool HadUnitySave { get; private set; }
        public SaveSystem(){ HadUnitySave=PlayerPrefs.HasKey(Key); Load(); }
        public void Load(){
            if(PlayerPrefs.HasKey(Key)) Data=JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(Key));
            if(Data==null) Data=new SaveData();
            Data.unlocked=Mathf.Clamp(Data.unlocked,1,50); Data.selected=Mathf.Clamp(Data.selected,1,Data.unlocked);
            Data.hearts=Mathf.Clamp(Data.hearts,0,5); Data.coins=Mathf.Max(0,Data.coins);
            if(Data.stars==null || Data.stars.Length!=50) Data.stars=new int[50];
            if(Data.failCounts==null || Data.failCounts.Length!=50) Data.failCounts=new int[50];
            if(Data.inventory==null) Data.inventory=new InventorySave();
            if(Data.events==null) Data.events=new EventSave();
            RefreshHearts();
        }
        public void Save(){ PlayerPrefs.SetString(Key,JsonUtility.ToJson(Data)); PlayerPrefs.Save(); }
        public void CompleteLevel(int level,int stars,int reward){
            var i=Mathf.Clamp(level,1,50)-1; Data.stars[i]=Mathf.Max(Data.stars[i],stars);
            Data.unlocked=Mathf.Max(Data.unlocked,Mathf.Min(50,level+1)); Data.selected=Data.unlocked;
            Data.coins+=reward; Data.friendship=Mathf.Min(100,Data.friendship+2+stars); Data.failCounts[i]=0; Save();
        }
        public bool SpendCoins(int amount){ if(Data.coins<amount)return false; Data.coins-=amount;Save();return true; }
        public void RefreshHearts(){
            if(Data.hearts>=5){Data.hearts=5;Data.heartRefillTicks=DateTime.UtcNow.Ticks;return;}
            if(Data.heartRefillTicks<=0)Data.heartRefillTicks=DateTime.UtcNow.Ticks;
            long step=TimeSpan.FromMinutes(30).Ticks,elapsed=DateTime.UtcNow.Ticks-Data.heartRefillTicks;int gained=(int)(elapsed/step);
            if(gained>0){Data.hearts=Mathf.Min(5,Data.hearts+gained);Data.heartRefillTicks+=gained*step;if(Data.hearts>=5)Data.heartRefillTicks=DateTime.UtcNow.Ticks;}
        }
        public void LoseHeart(){RefreshHearts();if(Data.hearts>0){if(Data.hearts==5)Data.heartRefillTicks=DateTime.UtcNow.Ticks;Data.hearts--;}Save();}
        public void RecordFail(int level){int i=Mathf.Clamp(level,1,50)-1;Data.failCounts[i]=Mathf.Min(9,Data.failCounts[i]+1);Save();}
    }
}
