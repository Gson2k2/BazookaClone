using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using MyGame.Script.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGame.Script.CoreGame
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;
        [SerializeField] private LevelData _levelData;
        [SerializeField] private int hardCapLv;

        private LevelController _currentLevel;

        private void Awake()
        {
            var levelStage = _levelData.levelDataLists.First(x =>
                x.levelStageName.Equals(GameSettingManager.Instance.OnDataLoad().
                    playerResources.PlayerCurrentStage,StringComparison.OrdinalIgnoreCase));
            hardCapLv = levelStage.levelArr.Count;
            Instance = this;
        }

        private void Start()
        {
            OnLevelLoad();
        }

        int OnUserGetLevel()
        {
            var userInitData = GameSettingManager.Instance.OnDataLoad();
            return userInitData.playerResources.PlayerCurrentLevel;
        }
        string OnUserGetStage()
        {
            var userInitData = GameSettingManager.Instance.OnDataLoad();
            return userInitData.playerResources.PlayerCurrentStage;
        }

        public async void OnLevelLoad()
        {
            if (_currentLevel != null)
            {
                Destroy(_currentLevel.gameObject);
            }

            UIController.Instance.OnBulletReset();

            if (GameSettingManager.Instance.OnDataLoad().playerResources.PlayerCurrentLevel > hardCapLv)
            {
                GameSettingManager.Instance.OnPlayerLevelReset();
            }
            var levelStage = _levelData.levelDataLists.FirstOrDefault(x => x.levelStageName.Equals(OnUserGetStage(),StringComparison.OrdinalIgnoreCase));
            var level = levelStage.levelArr.FirstOrDefault(x => x.level == OnUserGetLevel());
            await UniTask.DelayFrame(1);
            var levelClone = Instantiate(level.levelObject);
            levelClone.cameraDefSize = level.levelCamSize;
            _currentLevel = levelClone;
        }
    }
}
