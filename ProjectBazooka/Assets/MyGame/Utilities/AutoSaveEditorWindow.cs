#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


namespace MyGame.Utilities
{
    public class AutoSaveEditorWindow : EditorWindow
    {
        private static bool _autoSaveEnabled;
        private static float _time = 0f;
        private static float _lastTime = 0f;
        private static int _interval = 5;

        [MenuItem("MyCustomMenu/AutoSaveConfig")]
        private static void ShowWindow()
        {
            GetWindow<AutoSaveEditorWindow>("AutoSave Config");
        }
        
        private static void UpdateConfig()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode || !EditorApplication.isCompiling)
            {
                float currentTime = (float)EditorApplication.timeSinceStartup;
                _time += currentTime - _lastTime;
                _lastTime = currentTime;
                
                if (_time >= _interval)
                {
                    EditorSceneManager.SaveOpenScenes();
                    AssetDatabase.SaveAssets();
                    _time = 0f;
                }
            }
        }
        

        [InitializeOnLoadMethod]
        static void SaveConfig()
        {
            if (PlayerPrefs.GetInt("AutoSave") == 1)
            {
                Debug.Log("Registering UpdateConfig");
                foreach (var d in EditorApplication.update.GetInvocationList())
                {
                    if (d.Method.Name == "UpdateConfig")
                    {
                        EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update,
                            new EditorApplication.CallbackFunction(UpdateConfig));
                    }
                }
                
                EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update,
                    new EditorApplication.CallbackFunction(UpdateConfig));
                EditorApplication.playModeStateChanged += SaveOnPlay;
            }
            else
            {
                Debug.Log("Unregistering UpdateConfig");
                EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update,
                    new EditorApplication.CallbackFunction(UpdateConfig));
                EditorApplication.playModeStateChanged -= SaveOnPlay;
            }
            
        }
        
        
        
        private static void SaveOnPlay(PlayModeStateChange state)
        {
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            Debug.Log("Editor Saving...");
        }

        private void OnEnable()
        {
            _autoSaveEnabled = PlayerPrefs.GetInt("AutoSave") == 1;
        }

        private void OnGUI()
        {
            GUILayout.Label("AutoSave Configuration", EditorStyles.boldLabel);

            _autoSaveEnabled = EditorGUILayout.Toggle("Enable AutoSave", _autoSaveEnabled);
            _interval = EditorGUILayout.IntField("Auto Save Interval", _interval);
            
            if (GUILayout.Button("Save Config"))
            {
                PlayerPrefs.SetInt("AutoSave", _autoSaveEnabled ? 1 : 0);
                SaveConfig();
            }
            
        }
    }
}
#endif

