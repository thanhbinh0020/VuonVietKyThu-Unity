#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
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
            ConfigureSigning();
            Directory.CreateDirectory(Path.Combine(root,"Builds"));
            string output=Path.Combine(root,"Builds","VuonVietKyThu-2.0.2-build-ready.apk");
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
            PlayerSettings.bundleVersion="2.0.2-build-ready";
            PlayerSettings.Android.bundleVersionCode=22;
            PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion=AndroidSdkVersions.AndroidApiLevelAuto;
            // Unity 6 defaults new projects to GameActivity. Our custom manifest and legacy WebView bridge use UnityPlayerActivity.
            PlayerSettings.Android.applicationEntry=AndroidApplicationEntry.Activity;
            EditorUserBuildSettings.buildAppBundle=false;
            PlayerSettings.defaultInterfaceOrientation=UIOrientation.Portrait;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64|AndroidArchitecture.ARMv7;
            PlayerSettings.stripEngineCode=true;
            PlayerSettings.runInBackground=false;
        }

        static void ConfigureSigning(){
            // CI can pass a project-relative properties file without exposing passwords in command-line logs.
            string propertiesArg=CommandLineValue("-vvktSigningProperties");
            if(!string.IsNullOrWhiteSpace(propertiesArg)){
                string root=Directory.GetParent(Application.dataPath).FullName;
                string propertiesPath=Path.IsPathRooted(propertiesArg)?propertiesArg:Path.Combine(root,propertiesArg);
                if(File.Exists(propertiesPath)){
                    var cfg=ReadProperties(propertiesPath);
                    string ks=Get(cfg,"keystore");
                    if(!Path.IsPathRooted(ks)) ks=Path.Combine(root,ks);
                    ApplySigning(ks,Get(cfg,"storePass"),Get(cfg,"alias"),Get(cfg,"aliasPass"));
                    return;
                }
            }

            string keystorePath=Environment.GetEnvironmentVariable("VVKT_KEYSTORE_PATH");
            if(string.IsNullOrWhiteSpace(keystorePath)){
                PlayerSettings.Android.useCustomKeystore=false;
                return;
            }
            ApplySigning(
                keystorePath,
                Environment.GetEnvironmentVariable("VVKT_KEYSTORE_PASS") ?? "",
                Environment.GetEnvironmentVariable("VVKT_KEY_ALIAS") ?? "",
                Environment.GetEnvironmentVariable("VVKT_KEY_ALIAS_PASS") ?? ""
            );
        }

        static void ApplySigning(string keystorePath,string storePass,string alias,string aliasPass){
            if(!File.Exists(keystorePath))
                throw new FileNotFoundException("Signing keystore does not exist",keystorePath);
            if(string.IsNullOrEmpty(storePass)||string.IsNullOrEmpty(alias)||string.IsNullOrEmpty(aliasPass))
                throw new InvalidOperationException("Signing configuration is incomplete.");

            PlayerSettings.Android.useCustomKeystore=true;
            PlayerSettings.Android.keystoreName=keystorePath;
            PlayerSettings.Android.keystorePass=storePass;
            PlayerSettings.Android.keyaliasName=alias;
            PlayerSettings.Android.keyaliasPass=aliasPass;
        }

        static Dictionary<string,string> ReadProperties(string path){
            var result=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
            foreach(string raw in File.ReadAllLines(path)){
                string line=raw.Trim();
                if(line.Length==0||line.StartsWith("#"))continue;
                int eq=line.IndexOf('=');
                if(eq<=0)continue;
                result[line.Substring(0,eq).Trim()]=line.Substring(eq+1);
            }
            return result;
        }

        static string Get(Dictionary<string,string> cfg,string key)=>cfg.TryGetValue(key,out var value)?value:"";

        static string CommandLineValue(string key){
            string[] args=Environment.GetCommandLineArgs();
            for(int i=0;i<args.Length-1;i++)if(string.Equals(args[i],key,StringComparison.OrdinalIgnoreCase))return args[i+1];
            return "";
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
