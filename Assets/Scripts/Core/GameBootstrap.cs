using UnityEngine;

namespace VuonVietKyThu {
    public sealed class GameBootstrap : MonoBehaviour {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] static void Boot(){if(FindFirstObjectByType<GameBootstrap>()==null)new GameObject("Vườn Việt Kỳ Thú").AddComponent<GameBootstrap>();}
        void Awake(){DontDestroyOnLoad(gameObject);gameObject.AddComponent<ImmersiveMode>();var db=new LevelDatabase();var save=new SaveSystem();AudioHaptics.HapticsEnabled=save.Data.haptic;var eventSystem=new EventSystem(save);var sfx=gameObject.AddComponent<SfxPlayer>();sfx.Init(save);var fx=gameObject.AddComponent<EffectsController>();fx.Quality=save.Data.quality;var game=gameObject.AddComponent<GameController>();var ui=gameObject.AddComponent<UIManager>();game.Init(db,save,ui,fx,eventSystem,sfx);ui.Init(save,db,game,fx,eventSystem,sfx);fx.Init(ui.Canvas);gameObject.AddComponent<LegacySaveMigrator>().Begin(save,ui);Application.targetFrameRate=60;QualitySettings.vSyncCount=0;}
    }
}
