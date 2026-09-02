using UnityEngine;

namespace VuonVietKyThu {
    [ExecuteAlways] public sealed class SafeAreaFitter : MonoBehaviour {
        RectTransform rt; Rect last;
        void OnEnable(){rt=GetComponent<RectTransform>();Apply();}
        void Update(){if(Screen.safeArea!=last)Apply();}
        void Apply(){if(rt==null)return;last=Screen.safeArea;var min=last.position;var max=last.position+last.size;min.x/=Screen.width;min.y/=Screen.height;max.x/=Screen.width;max.y/=Screen.height;rt.anchorMin=min;rt.anchorMax=max;rt.offsetMin=rt.offsetMax=Vector2.zero;}
    }
}
