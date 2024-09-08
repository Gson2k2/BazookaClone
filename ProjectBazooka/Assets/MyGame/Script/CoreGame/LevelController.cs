using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyGame.Script.Gameplay;
using MyGame.Script.Gameplay.Controller;
using MyGame.Script.TestCode;
using MyGame.Utilities;
using Sirenix.Utilities;
using UnityEngine;

namespace MyGame.Script.CoreGame
{
    public class LevelController : MonoBehaviour
    {
        public List<RagdollCharacter> _enemyControllers;
        public List<NewCharacter> _playerControllers;

        [SerializeField] public int cameraDefSize;
        [HideInInspector] public bool _isComplete;
        [HideInInspector] public bool _isFailed;

        private int _bulletRemain;
        private async void Awake()
        {
            OnLevelConfig();
            await CheckEnemiesAsync();
        }

        private void Start()
        {
            Camera.main.orthographicSize = 1;
            Debug.Log("Cam Size: " + cameraDefSize);
            Camera.main.DOOrthoSize(cameraDefSize, 0.25f).SetEase(Ease.OutExpo);
        }

        private async UniTask CheckEnemiesAsync()
        {
            while (!_isComplete)
            {
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
                if (_enemyControllers.IsNullOrEmpty())
                {
                    _isComplete = true;
                    OnLevelComplete();
                }
            }
        }

        public void OnLevelConfig()
        {
            _levelFailCancelToken = new CancellationTokenSource();
            _bulletRemain = 3;
            UIController.Instance.bulletHolderList.OnSortFromFirstToLastIndex();
            UIController.Instance.bulletHolder.transform.DOScale(Vector2.one,0.25f);
        }

        public async void OnBulletFire()
        {
            _bulletRemain--;
            if (OnBulletIsEmptyOrNull())
            {
                if(_isFailed) return;
                _isFailed = true;
                await UniTask.Delay(TimeSpan.FromSeconds(5f));
                OnLevelFail();
            }
            else
            {
                Debug.Log(_bulletRemain);
                var bullet = UIController.Instance.bulletHolderList.Last(x => x.GetComponent<CanvasGroup>().alpha >= 1);
                bullet.GetComponent<CanvasGroup>().DOFade(0.25f, 0.25f);
            }
        }
        
        private CancellationTokenSource _killCounterCts;
        private int _killCount;

        public async void OnAwaitForKillCounter(EnemyController enemyController)
        {
            if (_killCounterCts != null)
            {
                _killCounterCts.Cancel();
            }
            _killCounterCts = new CancellationTokenSource();
            _killCount++;
            OnEnemyRemainCheck(enemyController);
            await DOTween.Sequence().AppendInterval(5f)
                .OnComplete(() =>
                {
                    //TODO check for kill counter enable or not
                    if (_killCount >= 2)
                    {
                        OnEnemyMultiKill();
                    }

                    _killCount = 0;
                }).WithCancellation(_killCounterCts.Token);
        }
            

        bool OnBulletIsEmptyOrNull()
        {
            if (_bulletRemain <= 0)
            {
                if(_isComplete) return false;
                return true;
            }
            return false;
        }

        public void OnEnemyRemainCheck(EnemyController enemyController)
        {
            Debug.Log("Remove Enemy" + enemyController);
            if (_enemyControllers.IsNullOrEmpty())
            {
                return;
            }
            _enemyControllers.Remove(enemyController);
        }

        public void OnEnemyMultiKill()
        {
            ResultManager.Instance.OnQueueResultTask(RewardBannerType.MultiKill);
        }
        

        public async void OnLevelComplete()
        {
            //TODO set up level complete panel
            foreach (var item in _playerControllers)
            {
                item.anim.Play("Player_Win");
                item.GetComponent<RagdollController>().OnHolsterWeapon();
            }
            Debug.Log("End Game");
            _killCounterCts?.Cancel();
            UIController.Instance.bulletHolder.transform.DOScale(Vector2.zero,0.25f);

            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            GameSettingManager.Instance.endgameManager.OnEndGame(false);
        }

        private CancellationTokenSource _levelFailCancelToken;
        public async void OnLevelFail()
        {
            
            foreach (var item in _enemyControllers)
            {
                item.anim.Play("Hunter_Win");
            }
            Debug.Log("Fail Game");
            //TODO set up level fail panel
            _killCounterCts?.Cancel();
            await UniTask.Delay(TimeSpan.FromSeconds(2f));
            if(this.GetCancellationTokenOnDestroy().IsCancellationRequested) return;
            UIController.Instance.bulletHolder.transform.DOScale(Vector2.zero,0.25f);
            GameSettingManager.Instance.endgameManager.OnEndGame(true);

        }
    }
}

