using UnityEngine;

namespace VuonVietKyThu {
    public sealed class ImmersiveMode : MonoBehaviour {
        void Awake(){ Screen.orientation=ScreenOrientation.Portrait; Screen.fullScreenMode=FullScreenMode.FullScreenWindow; Apply(); }
        void OnApplicationFocus(bool focus){ if(focus)Apply(); }
        void Apply(){
#if UNITY_ANDROID && !UNITY_EDITOR
            try{
                using var up=new AndroidJavaClass("com.unity3d.player.UnityPlayer"); using var a=up.GetStatic<AndroidJavaObject>("currentActivity");
                a.Call("runOnUiThread",new AndroidJavaRunnable(()=>{ using var w=a.Call<AndroidJavaObject>("getWindow"); using var d=w.Call<AndroidJavaObject>("getDecorView"); d.Call("setSystemUiVisibility",5894); }));
            }catch{}
#endif
        }
    }
}
