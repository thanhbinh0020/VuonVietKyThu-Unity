using System;
using System.Collections.Generic;
using UnityEngine;

namespace VuonVietKyThu {
    [Serializable] public class GoalDefinition { public int fruitType; public int count; }
    [Serializable] public class LevelDefinition {
        public int id; public int region; public int localIndex; public int moves; public int typeCount=6;
        public int jelly; public float goalBias; public int tutorialStep; public int starterSpecials;
        public List<GoalDefinition> goals=new(); public int[] starScores;
    }
    [Serializable] public class RegionDefinition { public string name; public string subtitle; public int[] featuredFruits; }
    [Serializable] public class LevelCollection { public RegionDefinition[] regions; public LevelDefinition[] levels; }
    public enum SpecialType { None, RocketRow, RocketColumn, Bomb, Rainbow }
    public enum QualityMode { Smooth, Auto, Max3D }
    public sealed class TileData {
        public int fruit; public SpecialType special; public bool jelly;
        public TileData Clone()=>new(){fruit=fruit,special=special,jelly=jelly};
    }
    [Serializable] public class InventorySave { public int basket=5, fan=5, pinwheel=5, extraMoves=0; }

    [Serializable] public class EventSave {
        public string dailyLast="", missionDay="", weekKey=""; public int dailyStreak=0, festivalPoints=0; public long chestReadyTicks=0;
        public int missionPlayed=0, missionWins=0, missionSpecials=0; public bool claimPlayed=false, claimWins=false, claimSpecials=false; public bool[] milestoneClaims=new bool[4];
    }
    [Serializable] public class SaveData {
        public int unlocked=1, selected=1, coins=1250, hearts=5, friendship=0;
        public int[] stars=new int[50]; public int[] failCounts=new int[50]; public InventorySave inventory=new(); public EventSave events=new();
        public QualityMode quality=QualityMode.Auto; public bool haptic=true, sound=true;
        public string dailyGift=""; public string activeCosmetic=""; public bool lotusOwned=false, goldOwned=false; public long heartRefillTicks=0;
    }
}
