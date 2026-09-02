#if UNITY_EDITOR
using System;
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

        [MenuItem("Vườn Việt/Chuẩn bị Project")]
        public static void PrepareProject(){
            EnsureScene();
            ApplyPlayerSettings();
            AssetDatabase.SaveAssets();
            Debug.Log("VVKT project setup ready: "+ScenePath);
        }

        [MenuItem("Vườn Việt/Build Android APK")]
        public static void PerformBuild(){
            PrepareProject();
            string root=Directory.GetParent(Application.dataPath).FullName;
            ConfigureSigningFromEnvironment();
            Directory.CreateDirectory(Path.Combine(root,"Builds"));
            string output=Path.Combine(root,"Builds","VuonVietKyThu-2.0.1-fix.apk");
            var opts=new BuildPlayerOptions{
                scenes=new[]{ScenePath},
                locationPathName=output,
                target=BuildTarget.Android,
                options=BuildOptions.CompressWithLz4HC
            };
            BuildReport report=BuildPipeline.BuildPlayer(opts);
            if(report.summary.result!=BuildResult.Succeeded)
                throw new Exception("Android build failed: "+report.summary.result);
            Debug.Log($"VVKT Unity APK: {output} ({report.summary.totalSize} bytes)");
        }

        static void ApplyPlayerSettings(){
            PlayerSettings.companyName="Vườn Việt Studio";
            PlayerSettings.productName="Vườn Việt Kỳ Thú";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android,"vn.vuonviet.kythu");
            PlayerSettings.bundleVersion="2.0.1-fix";
            PlayerSettings.Android.bundleVersionCode=21;
            PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion=AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.defaultInterfaceOrientation=UIOrientation.Portrait;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64|AndroidArchitecture.ARMv7;
            PlayerSettings.stripEngineCode=true;
            PlayerSettings.runInBackground=false;
        }

        static void ConfigureSigningFromEnvironment(){
            string keystorePath=Environment.GetEnvironmentVariable("VVKT_KEYSTORE_PATH");
            if(string.IsNullOrWhiteSpace(keystorePath)){
                PlayerSettings.Android.useCustomKeystore=false;
                return;
            }
            if(!File.Exists(keystorePath))
                throw new FileNotFoundException("VVKT_KEYSTORE_PATH does not exist",keystorePath);

            string storePass=Environment.GetEnvironmentVariable("VVKT_KEYSTORE_PASS") ?? "";
            string alias=Environment.GetEnvironmentVariable("VVKT_KEY_ALIAS") ?? "";
            string aliasPass=Environment.GetEnvironmentVariable("VVKT_KEY_ALIAS_PASS") ?? "";
            if(string.IsNullOrEmpty(storePass)||string.IsNullOrEmpty(alias)||string.IsNullOrEmpty(aliasPass))
                throw new InvalidOperationException("Signing env is incomplete. Set VVKT_KEYSTORE_PASS, VVKT_KEY_ALIAS and VVKT_KEY_ALIAS_PASS.");

            PlayerSettings.Android.useCustomKeystore=true;
            PlayerSettings.Android.keystoreName=keystorePath;
            PlayerSettings.Android.keystorePass=storePass;
            PlayerSettings.Android.keyaliasName=alias;
            PlayerSettings.Android.keyaliasPass=aliasPass;
        }

        static void EnsureScene(){
            if(File.Exists(ScenePath)){
                EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
                return;
            }
            Directory.CreateDirectory("Assets/Scenes");
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            new GameObject("Runtime Bootstrap Marker");
            EditorSceneManager.SaveScene(scene,ScenePath);
            EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
        }
    }
}
#endif
