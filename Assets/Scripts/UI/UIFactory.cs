using System;
using UnityEngine;
using UnityEngine.UI;

namespace VuonVietKyThu {
    public static class UIFactory {
        static Font font;
        public static Font Font=>font??=(Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf"));
        public static GameObject Go(string name,Transform parent){var g=new GameObject(name);g.transform.SetParent(parent,false);return g;}
        public static RectTransform Rect(GameObject g,Vector2 min,Vector2 max,Vector2 offMin,Vector2 offMax){var r=g.GetComponent<RectTransform>()??g.AddComponent<RectTransform>();r.anchorMin=min;r.anchorMax=max;r.offsetMin=offMin;r.offsetMax=offMax;return r;}
        public static Image Image(string name,Transform parent,Color color,Sprite sprite=null){var g=Go(name,parent);var r=g.AddComponent<RectTransform>();var i=g.AddComponent<Image>();i.color=color;i.sprite=sprite;i.preserveAspect=sprite!=null;return i;}
        public static Text Text(string name,Transform parent,string value,int size,TextAnchor anchor=TextAnchor.MiddleCenter,Color? color=null){var g=Go(name,parent);g.AddComponent<RectTransform>();var t=g.AddComponent<Text>();t.font=Font;t.text=value;t.fontSize=size;t.fontStyle=FontStyle.Bold;t.alignment=anchor;t.color=color??Color.white;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Overflow;return t;}
        public static Button Button(string name,Transform parent,string label,Color color,Action action,int fontSize=26){
            var g=Go(name,parent);g.AddComponent<RectTransform>();var im=g.AddComponent<Image>();im.color=color;var sh=g.AddComponent<Shadow>();sh.effectDistance=new Vector2(0,-5);sh.effectColor=new Color(0,0,0,.28f);var b=g.AddComponent<Button>();b.targetGraphic=im;b.onClick.AddListener(()=>action?.Invoke());
            var t=Text("Label",g.transform,label,fontSize);Rect(t.gameObject,Vector2.zero,Vector2.one,new Vector2(8,5),new Vector2(-8,-5));return b;
        }
        public static void Stretch(GameObject g,float left=0,float bottom=0,float right=0,float top=0)=>Rect(g,Vector2.zero,Vector2.one,new Vector2(left,bottom),new Vector2(-right,-top));
        public static void Anchor(RectTransform r,Vector2 anchor,Vector2 pos,Vector2 size){r.anchorMin=r.anchorMax=anchor;r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=pos;r.sizeDelta=size;}
        public static void Panel3D(GameObject g,Color color){var im=g.GetComponent<Image>()??g.AddComponent<Image>();im.color=color;var s=g.GetComponent<Shadow>()??g.AddComponent<Shadow>();s.effectColor=new Color(.18f,.03f,.26f,.46f);s.effectDistance=new Vector2(0,-7);}
    }
}
