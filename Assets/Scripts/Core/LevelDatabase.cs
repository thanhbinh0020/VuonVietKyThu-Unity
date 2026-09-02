using System;
using UnityEngine;

namespace VuonVietKyThu {
    public sealed class LevelDatabase {
        public LevelCollection Data { get; private set; }
        public LevelDatabase(){
            var asset=Resources.Load<TextAsset>("Data/levels");
            if(asset==null) throw new Exception("Missing Resources/Data/levels.json");
            Data=JsonUtility.FromJson<LevelCollection>(asset.text);
            if(Data?.levels==null || Data.levels.Length!=50) throw new Exception("Level database must contain 50 levels");
        }
        public LevelDefinition Get(int id)=>Data.levels[Mathf.Clamp(id,1,50)-1];
        public RegionDefinition Region(int index)=>Data.regions[Mathf.Clamp(index,0,Data.regions.Length-1)];
    }
}
