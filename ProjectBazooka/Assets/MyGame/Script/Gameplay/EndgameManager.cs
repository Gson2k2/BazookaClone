using System;
using System.Linq;
using MyGame.Script.CoreGame;
using MyGame.Script.Gameplay.Controller;
using MyGame.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace MyGame.Script.Gameplay
{
    public class EndgameManager : MonoBehaviour
    {
        void OnEndGameCompleteConfig()
        {
            Debug.Log("Result Task: " + $"<color=#FF00FF>{RewardBannerType.LevelComplete}</color>");
            ResultManager.Instance.OnQueueResultTask(RewardBannerType.LevelComplete);
            if (UIController.Instance.bulletHolderList.Count(x => !x.activeSelf) < 2)
            {
                Debug.Log("Result Task: " + $"<color=#FF00FF>{RewardBannerType.FirstTry}</color>");
                ResultManager.Instance.OnQueueResultTask(RewardBannerType.FirstTry);
            }
            ResultManager.Instance.OnResultCalculated(false);
            ResultManager.Instance.OnReward();
            
        }

        void OnEndGameFailedConfig()
        {
            Debug.Log("Result Task: " +$"<color=#FF00A2>Level Failed</color>");
            ResultManager.Instance.OnLevelFailOpen();
        }
        

        public void OnEndGame(bool isFailed)
        {
            if (!isFailed)
            {
                OnEndGameCompleteConfig();
            }
            else
            {
                OnEndGameFailedConfig();
            }
            
        }
    }
}
