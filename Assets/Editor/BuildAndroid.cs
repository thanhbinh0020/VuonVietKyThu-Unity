#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VuonVietKyThu.Editor {
    public static class BuildAndroid {
        const string ScenePath="Assets/Scenes/Main.unity";
        [MenuItem("Vườn Việt/Build Android APK")]
        public static void PerformBuild(){
            EnsureScene();
            PlayerSettings.companyName="Vườn Việt Studio";PlayerSettings.productName="Vườn Việt Kỳ Thú";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android,"vn.vuonviet.kythu");
            PlayerSettings.bundleVersion="2.0.0-unity-alpha";PlayerSettings.Android.bundleVersionCode=20;
            PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;PlayerSettings.Android.targetSdkVersion=AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.defaultInterfaceOrientation=UIOrientation.Portrait;PlayerSettings.Android.useCustomKeystore=true;
            string root=Directory.GetParent(Application.dataPath).FullName;
            string keystorePath=Environment.GetEnvironmentVariable("VVKT_KEYSTORE_PATH");
            if(!string.IsNullOrWhiteSpace(keystorePath)){
                PlayerSettings.Android.useCustomKeystore=true;
                PlayerSettings.Android.keystoreName=keystorePath;
                PlayerSettings.Android.keystorePass=Environment.GetEnvironmentVariable("VVKT_KEYSTORE_PASS") ?? "";
                PlayerSettings.Android.keyaliasName=Environment.GetEnvironmentVariable("VVKT_KEY_ALIAS") ?? "";
                PlayerSettings.Android.keyaliasPass=Environment.GetEnvironmentVariable("VVKT_KEY_ALIAS_PASS") ?? "";
            }else{
                PlayerSettings.Android.useCustomKeystore=false;
            }
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64|AndroidArchitecture.ARMv7;
            PlayerSettings.stripEngineCode=true;PlayerSettings.runInBackground=false;
            Directory.CreateDirectory(Path.Combine(root,"Builds"));string output=Path.Combine(root,"Builds","VuonVietKyThu-2.0.0-unity-alpha.apk");
            var opts=new BuildPlayerOptions{scenes=new[]{ScenePath},locationPathName=output,target=BuildTarget.Android,options=BuildOptions.CompressWithLz4HC};
            BuildReport report=BuildPipeline.BuildPlayer(opts);if(report.summary.result!=BuildResult.Succeeded)throw new System.Exception("Android build failed: "+report.summary.result);
            Debug.Log($"VVKT Unity APK: {output} ({report.summary.totalSize} bytes)");
        }
        static void EnsureScene(){
            if(File.Exists(ScenePath)){EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};return;}
            Directory.CreateDirectory("Assets/Scenes");var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);new GameObject("Runtime Bootstrap Marker");EditorSceneManager.SaveScene(scene,ScenePath);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};AssetDatabase.SaveAssets();
        }
    }
}
#endif
