using System.Collections.Generic;
using UnityEngine;

namespace VuonVietKyThu {
    public static class SpriteCatalog {
        static readonly Dictionary<string,Sprite> Cache=new();
        public static Sprite Get(string path){
            if(Cache.TryGetValue(path,out var s) && s) return s;
            var tex=Resources.Load<Texture2D>(path); if(!tex) return null;
            s=Sprite.Create(tex,new Rect(0,0,tex.width,tex.height),new Vector2(.5f,.5f),100f);
            Cache[path]=s; return s;
        }
        public static Sprite Fruit(int type)=>Get($"Art/Fruits/fruit-{Mathf.Clamp(type,0,5)}");
        public static Sprite Magic(int index)=>Get($"Art/Magic/magic-{Mathf.Clamp(index,0,3)}");
        public static Sprite Home()=>Get("Art/Backgrounds/home-garden");
        public static Sprite Game()=>Get("Art/Backgrounds/game-garden");
        public static Sprite Mascot()=>Get("Art/Characters/mascot-mai");
        public static Sprite MascotPortrait()=>Get("Art/Characters/mascot-mai-portrait");
        public static Sprite Region(int r){string[] n={"mekong","north","tea","coast","highland"};return Get("Art/Regions/region-"+n[Mathf.Clamp(r,0,4)]);}
    }
}
