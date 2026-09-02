using UnityEngine;

namespace VuonVietKyThu {
    public static class AudioHaptics {
        public static bool HapticsEnabled=true;
        public static void Vibrate(long ms=18){
#if UNITY_ANDROID && !UNITY_EDITOR
            if(!HapticsEnabled)return;
            try{
                using var unityPlayer=new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity=unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                using var vibrator=activity.Call<AndroidJavaObject>("getSystemService","vibrator");
                if(vibrator!=null) vibrator.Call("vibrate",ms);
            }catch{}
#endif
        }
    }
}
