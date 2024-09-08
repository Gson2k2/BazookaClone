using System;
using System.Collections.Generic;
using System.IO;
using MyGame.Script.Gameplay;
using MyGame.Utilities;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace MyGame.Script.CoreGame
{
    public enum ItemStatus
    {
        Unavailable,CanBuy,ADReward
    }

    public enum ItemRarity
    {
        Common,Rare,Epic,Limited
    }
    public class GameSettingManager : MonoBehaviour
    {
        public static GameSettingManager Instance;
        public static string _saveFilePath;
        
        [Header("Main Component")]
        public AdsManager adsManager;
        public AudioManager audioManager;
        public EndgameManager endgameManager;

        [Header("End Game Events")] 
        public UnityEvent endGameEvents;
        
        private GameData _gameData;
        

        #region EditorOnly
        #if UNITY_EDITOR
        [Button("Manual Validate")]
        private void OnValidate()
        {
            if (gameObject.HasComponents<AdsManager>())
            {
                adsManager = GetComponent<AdsManager>();
            }
            if (gameObject.HasComponents<AudioManager>())
            {
                audioManager = GetComponent<AudioManager>();
            }
            if (gameObject.HasComponents<EndgameManager>())
            {
                endgameManager = GetComponent<EndgameManager>();
                // if (endGameEvents.EventNotNull())
                // {
                //     UnityEventTools.RemovePersistentListener(endGameEvents,endgameManager.OnEndGame());
                // }
                // UnityEventTools.AddPersistentListener(endGameEvents,endgameManager.OnEndGame);
            }
            
             
        }
        #endif
        #endregion

        private void OnEnable()
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        private void Awake()
        {
            OnInit();
            audioManager.OnAudioInit();
            OnSystemConfig();
        }

        void OnSystemConfig()
        {
            OnDataLoad();
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = _gameData.systemData.AppTargetFramerate;
            audioManager.bgAudioSource.volume = _gameData.systemData.AudioVolumeLevel;
            audioManager.effAudioSource.volume = _gameData.systemData.AudioVolumeLevel;
        }

        public void OnSystemSettingUpdate(ToggleButtonFunc func,ToggleCondition condition)
        {
            OnDataLoad();
            if (func == ToggleButtonFunc.Vsync)
            {
                switch (condition)
                {
                    case ToggleCondition.Off:
                        _gameData.systemData.AppTargetFramerate = 60;
                        break;
                    case ToggleCondition.On:
                        _gameData.systemData.AppTargetFramerate = (int) Screen.currentResolution.refreshRateRatio.value;
                        break;
                }
            }

            if (func == ToggleButtonFunc.ShowAds)
            {
                switch (condition)
                {
                    case ToggleCondition.Off:
                        _gameData.systemData.ShowADS = false;
                        adsManager.OnDestroyBanner();
                        if (DevBuildUtil.Instance != null)
                        {
                            DevBuildUtil.Instance.adsPanel.SetActive(false);
                        }
                        break;
                    case ToggleCondition.On:
                        _gameData.systemData.ShowADS = true;
                        if (DevBuildUtil.Instance != null)
                        {
                            DevBuildUtil.Instance.adsPanel.SetActive(true);
                        }
                        break;
                }
            }

            if (func == ToggleButtonFunc.Sound)
            {
                switch (condition)
                {
                    case ToggleCondition.Off:
                        _gameData.systemData.AudioVolumeLevel = 0;
                        break;
                    case ToggleCondition.On:
                        _gameData.systemData.AudioVolumeLevel = 75;
                        break;
                }
            }
            if (func == ToggleButtonFunc.Vibrate)
            {
                switch (condition)
                {
                    case ToggleCondition.Off:
                        _gameData.systemData.SystemVibrate = false;
                        break;
                    case ToggleCondition.On:
                        _gameData.systemData.SystemVibrate = true;
                        Handheld.Vibrate();
                        break;
                }
            }
            if (func == ToggleButtonFunc.InternetReq)
            {
                switch (condition)
                {
                    case ToggleCondition.Off:
                        PlayerPrefs.SetInt("InternetReq",0);
                        break;
                    case ToggleCondition.On:
                        PlayerPrefs.SetInt("InternetReq",1);
                        break;
                }
            }
            OnDataSave();
            OnSystemConfig();
        }

        [Button("Data Init")]
        void OnInit()
        {
            _saveFilePath = Application.persistentDataPath + "/UserData.json";
            if (!File.Exists(_saveFilePath))
            {
                Debug.Log("First Time Init: " + $"<color=green>{Application.persistentDataPath}</color>" + 
                          "<color=green>/UserData.json</color>");
                DataFirstTimeInit();
            }
            else
            {
                Debug.Log("Data Load: " + $"<color=green>{Application.persistentDataPath}</color>" + 
                          "<color=green>/UserData.json</color>");
                OnDataLoad();
            }
        }


        void DataFirstTimeInit()
        {
            _gameData = new GameData
            {
                systemData =
                {
                    AppTargetFramerate = 30,
                    AudioVolumeLevel = 75,
                    ShowADS = true,
                    SystemVibrate = true
                },
                itemData =
                {
                    itemID = new List<int>(),
                },
                playerResources =
                {
                    PlayerCash = 0,
                    PlayerCurrentLevel = 1,
                    PlayerCurrentStage = "DebugStage",
                    PlayerCurrentGunEquipmentID = 0,
                    PlayerCurrentClothEquipmentID = 0
                }
            };
            OnDataSave();
        }
        

        [Button("Delete Save File")]
        public void OnDataReset()
        {
            _saveFilePath = Application.persistentDataPath + "/UserData.json";
            if (File.Exists(_saveFilePath))
            {
                Debug.Log("Data Delete at: " + $"<color=red>{Application.persistentDataPath}</color>" + 
                          "<color=red>/UserData.json</color>");
                File.Delete(_saveFilePath);
            }
        }
        [Button("Check User Save File")]
        public void OnCheckUserData()
        {
            _saveFilePath = Application.persistentDataPath + "/UserData.json";
            if (File.Exists(_saveFilePath))
            {
                Debug.Log("Data found at: " + $"<color=green>{Application.persistentDataPath}</color>" + 
                          "<color=green>/UserData.json</color>");
                #if UNITY_EDITOR
                System.Diagnostics.Process.Start("C:/Users/giang/AppData/LocalLow/DefaultCompany/ProjectA/UserData.json");
                #endif
            }
            else
            {
                Debug.Log("<color=red>Data not found</color>");
            }
        }
        
        public GameData OnDataLoad()
        {
            if (File.Exists(_saveFilePath))
            {
                string loadUserData = File.ReadAllText(_saveFilePath);
                _gameData = JsonUtility.FromJson<GameData>(loadUserData);
            }
            else
            {
                DataFirstTimeInit();
            }

            return _gameData;
        }

        void OnDataSave()
        {
            string saveUserData = JsonUtility.ToJson(_gameData);
            File.WriteAllText(_saveFilePath, saveUserData);
        }
        public void OnDataCashUpdate(int cashValue)
        {
            _gameData.playerResources.PlayerCash += cashValue;
            OnDataSave();
            if (UIController.Instance != null)
            {
                UIController.Instance.OnUIUpdate(_gameData);
            }
        }

        public void OnItemObtainUpdate(int itemID)
        {
            _gameData.itemData.itemID.Add(itemID);
            OnDataSave();
        }
        public void OnPlayerLevelUpdate()
        {
            _gameData.playerResources.PlayerCurrentLevel++;
            OnDataSave();
        }
        public void OnPlayerLevelReset()
        {
            _gameData.playerResources.PlayerCurrentLevel = 1;
            OnDataSave();
        }
    }
}
