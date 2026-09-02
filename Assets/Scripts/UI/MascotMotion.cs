using System.Collections;
using UnityEngine;

namespace VuonVietKyThu {
    public sealed class MascotMotion : MonoBehaviour {
        RectTransform rt; Vector2 basePos; Coroutine action;
        void Awake(){rt=GetComponent<RectTransform>();basePos=rt.anchoredPosition;}
        void OnEnable(){if(rt==null)rt=GetComponent<RectTransform>();basePos=rt.anchoredPosition;StartCoroutine(Idle());}
        IEnumerator Idle(){float phase=Random.value*5f;while(enabled){phase+=Time.unscaledDeltaTime;rt.anchoredPosition=basePos+new Vector2(Mathf.Sin(phase*.75f)*3f,Mathf.Sin(phase*1.35f)*7f);rt.localEulerAngles=new Vector3(0,Mathf.Sin(phase*.65f)*2.2f,Mathf.Sin(phase*.8f)*1.1f);yield return null;}}
        public void Cheer(){if(action!=null)StopCoroutine(action);action=StartCoroutine(CheerRoutine());}
        IEnumerator CheerRoutine(){float t=0;while(t<.55f){t+=Time.unscaledDeltaTime;float k=t/.55f;rt.localScale=Vector3.one*(1+.12f*Mathf.Sin(k*Mathf.PI*3));rt.localEulerAngles=new Vector3(0,Mathf.Sin(k*Mathf.PI*2)*12,-Mathf.Sin(k*Mathf.PI*4)*5);yield return null;}rt.localScale=Vector3.one;rt.localEulerAngles=Vector3.zero;}
        public void Wave()=>Cheer();
    }
}
