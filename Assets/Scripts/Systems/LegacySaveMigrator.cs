using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace VuonVietKyThu {
    [Serializable] class LegacyBoosters { public int basket=5,fan=5,pinwheel=5; }
    [Serializable] class LegacyInventory { public int extraMoves=0; }
    [Serializable] class LegacyCosmetics { public bool lotusAura=false,goldFrame=false; }
    [Serializable] class LegacyWebSave {
        public int unlocked=1,selected=1,coins=1250,hearts=5; public string quality="auto",activeCosmetic=""; public bool haptic=true,sound=true;
        public LegacyBoosters boosters=new(); public LegacyInventory inventory=new(); public LegacyCosmetics cosmetics=new();
    }
    public sealed class LegacySaveMigrator : MonoBehaviour {
        SaveSystem save; UIManager ui;
        public void Begin(SaveSystem s,UIManager u){save=s;ui=u;if(save.HadUnitySave)return;
#if UNITY_ANDROID && !UNITY_EDITOR
            try{using var bridge=new AndroidJavaClass("vn.vuonviet.kythu.bridge.LegacySaveBridge");bridge.CallStatic("startRead",gameObject.name);}catch{}
#endif
        }
        public void OnLegacySave(string base64){
            if(string.IsNullOrWhiteSpace(base64)||save.HadUnitySave)return;
            try{
                string json=Encoding.UTF8.GetString(Convert.FromBase64String(base64));if(string.IsNullOrWhiteSpace(json))return;var legacy=JsonUtility.FromJson<LegacyWebSave>(json);if(legacy==null)return;
                var d=save.Data;d.unlocked=Mathf.Clamp(legacy.unlocked,1,50);d.selected=Mathf.Clamp(legacy.selected,1,d.unlocked);d.coins=Mathf.Max(0,legacy.coins);d.hearts=Mathf.Clamp(legacy.hearts,0,5);d.inventory.basket=Mathf.Max(0,legacy.boosters?.basket??5);d.inventory.fan=Mathf.Max(0,legacy.boosters?.fan??5);d.inventory.pinwheel=Mathf.Max(0,legacy.boosters?.pinwheel??5);d.inventory.extraMoves=Mathf.Max(0,legacy.inventory?.extraMoves??0);d.haptic=legacy.haptic;d.sound=legacy.sound;d.quality=legacy.quality=="smooth"?QualityMode.Smooth:legacy.quality=="max"?QualityMode.Max3D:QualityMode.Auto;d.activeCosmetic=legacy.activeCosmetic=="lotusAura"?"lotus":legacy.activeCosmetic=="goldFrame"?"gold":"";d.lotusOwned=legacy.cosmetics?.lotusAura??false;d.goldOwned=legacy.cosmetics?.goldFrame??false;
                var m=Regex.Match(json,"\\\"stars\\\"\\s*:\\s*\\{(?<body>[^}]*)\\}");if(m.Success)foreach(Match sm in Regex.Matches(m.Groups["body"].Value,"\\\"(?<id>\\d+)\\\"\\s*:\\s*(?<v>\\d+)")){int id=int.Parse(sm.Groups["id"].Value),v=int.Parse(sm.Groups["v"].Value);if(id>=1&&id<=50)d.stars[id-1]=Mathf.Clamp(v,0,3);}
                save.Save();AudioHaptics.HapticsEnabled=d.haptic;ui.RefreshHome();ui.Toast("Đã chuyển tiến trình từ bản cũ sang Unity");
            }catch(Exception e){Debug.LogWarning("Legacy save migration skipped: "+e.Message);}
        }
    }
}
