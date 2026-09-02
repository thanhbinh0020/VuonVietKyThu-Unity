using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VuonVietKyThu {
    public sealed class EffectsController : MonoBehaviour {
        Canvas canvas; public QualityMode Quality=QualityMode.Auto;
        public void Init(Canvas c){canvas=c;}
        public void Flash(Color color,float duration=.18f){if(Quality==QualityMode.Smooth)return;StartCoroutine(FlashRoutine(color,duration));}
        IEnumerator FlashRoutine(Color color,float duration){
            var go=new GameObject("FX Flash");go.transform.SetParent(canvas.transform,false);var rt=go.AddComponent<RectTransform>();rt.anchorMin=Vector2.zero;rt.anchorMax=Vector2.one;rt.offsetMin=rt.offsetMax=Vector2.zero;var im=go.AddComponent<Image>();im.color=new Color(color.r,color.g,color.b,.0f);im.raycastTarget=false;
            float t=0;while(t<duration){t+=Time.unscaledDeltaTime;float a=Mathf.Sin(Mathf.Clamp01(t/duration)*Mathf.PI)*.22f;im.color=new Color(color.r,color.g,color.b,a);yield return null;}Destroy(go);
        }
        public void ComboPulse(RectTransform target,int combo){if(Quality==QualityMode.Smooth)return;StartCoroutine(Pulse(target,combo>=4?1.08f:1.045f,.22f));}
        IEnumerator Pulse(RectTransform rt,float max,float d){float t=0;while(t<d){t+=Time.unscaledDeltaTime;float s=1+(max-1)*Mathf.Sin(t/d*Mathf.PI);rt.localScale=Vector3.one*s;if(Quality==QualityMode.Max3D)rt.localEulerAngles=new Vector3(2*Mathf.Sin(t*25),-3*Mathf.Sin(t*18),0);yield return null;}rt.localScale=Vector3.one;rt.localEulerAngles=Vector3.zero;}
    }
}
