using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VuonVietKyThu {
    public sealed class TileView : MonoBehaviour {
        public int Index {get;private set;} Image cell,fruit,special,jelly; Button button; RectTransform fruitRt;
        Action<int> click;
        public void Build(int index,Action<int> onClick){
            Index=index;click=onClick;var rt=gameObject.AddComponent<RectTransform>();
            cell=gameObject.AddComponent<Image>(); cell.color=new Color(.11f,.47f,.72f,.82f);
            button=gameObject.AddComponent<Button>();button.transition=Selectable.Transition.None;button.onClick.AddListener(()=>click?.Invoke(Index));
            jelly=Child("Jelly",new Color(.72f,.95f,1f,.42f));
            fruit=Child("Fruit",Color.white); fruit.preserveAspect=true;fruitRt=fruit.rectTransform; fruitRt.offsetMin=new Vector2(3,3);fruitRt.offsetMax=new Vector2(-3,-3);
            special=Child("Special",Color.white);special.preserveAspect=true;special.rectTransform.offsetMin=new Vector2(7,7);special.rectTransform.offsetMax=new Vector2(-7,-7);
        }
        Image Child(string n,Color c){var go=new GameObject(n);go.transform.SetParent(transform,false);var r=go.AddComponent<RectTransform>();r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;var i=go.AddComponent<Image>();i.color=c;i.raycastTarget=false;return i;}
        public void Set(TileData data,bool selected,QualityMode q){
            if(data==null){fruit.enabled=false;special.enabled=false;jelly.enabled=false;return;}
            fruit.enabled=true;fruit.sprite=SpriteCatalog.Fruit(data.fruit);fruit.color=Color.white;jelly.enabled=data.jelly;
            special.enabled=data.special!=SpecialType.None;
            if(special.enabled){int m=data.special==SpecialType.Bomb?3:data.special==SpecialType.Rainbow?0:data.special==SpecialType.RocketRow?1:2;special.sprite=SpriteCatalog.Magic(m);}
            cell.color=selected?new Color(.25f,.88f,1f,.98f):new Color(.08f,.42f,.72f,.84f);
            fruitRt.localScale=selected?Vector3.one*1.13f:Vector3.one; fruitRt.localEulerAngles=selected&&q==QualityMode.Max3D?new Vector3(0,12,-4):Vector3.zero;
        }
        public IEnumerator Pop(float strength=1.18f,float duration=.16f){
            float t=0;var start=fruitRt.localScale;while(t<duration){t+=Time.unscaledDeltaTime;float k=t/duration;float s=1+(strength-1)*Mathf.Sin(k*Mathf.PI);fruitRt.localScale=Vector3.one*s;yield return null;}fruitRt.localScale=Vector3.one;
        }
        public IEnumerator Vanish(float duration=.14f){
            float t=0;while(t<duration){t+=Time.unscaledDeltaTime;float k=t/duration;fruitRt.localScale=Vector3.one*(1-k);fruitRt.Rotate(0,0,420*Time.unscaledDeltaTime);yield return null;}fruitRt.localScale=Vector3.one;fruitRt.localEulerAngles=Vector3.zero;
        }
    }
}
