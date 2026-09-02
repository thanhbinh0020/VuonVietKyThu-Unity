using System.Collections.Generic;
using UnityEngine;

namespace VuonVietKyThu {
    public sealed class SfxPlayer : MonoBehaviour {
        SaveSystem save; AudioSource source; readonly Dictionary<string,AudioClip> clips=new();
        public void Init(SaveSystem s){save=s;source=gameObject.AddComponent<AudioSource>();source.playOnAwake=false;source.volume=.48f;}
        public void Play(string id){if(save!=null&&!save.Data.sound)return;if(!clips.TryGetValue(id,out var clip)){clip=Build(id);clips[id]=clip;}source.PlayOneShot(clip);}
        AudioClip Build(string id){int rate=44100;float dur=id=="win"?.34f:id=="lose"?.28f:id=="special"?.16f:.10f;int n=Mathf.CeilToInt(rate*dur);float[] data=new float[n];
            for(int i=0;i<n;i++){float t=(float)i/rate,f= id=="win"?(t<.11f?660:t<.22f?880:1100):id=="lose"?Mathf.Lerp(330,190,t/dur):id=="special"?920:id=="match"?620:520;float env=Mathf.Sin(Mathf.Clamp01(t/dur)*Mathf.PI);data[i]=Mathf.Sin(2*Mathf.PI*f*t)*env*.32f;}
            var c=AudioClip.Create("vvkt_"+id,n,1,rate,false);c.SetData(data,0);return c;}
    }
}
