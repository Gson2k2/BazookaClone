using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyGame.Script.CoreGame;
using MyGame.Script.Gameplay;
using MyGame.Script.Gameplay.Controller;
using UnityEngine;
using UnityEngine.UI.Extensions;


namespace MyGame.Script.TestCode
{
    public class NewPlayerAimController : NewCharacter,ICharacterInteract
    {
        public LevelController _levelController;
        [SerializeField] float bombSpeed = 5;
        [SerializeField] float coolDown = 3;
        [SerializeField] Transform bombPf;
        [SerializeField] GameObject gunGo;
        [SerializeField] Transform shooter;
        [SerializeField] Transform holder;
        [SerializeField] Transform fireAimPos;
        [SerializeField] NewGunTrajectory simulation;

        private Camera _cam;
        private float _shootTime;
        private float _defCamSize;

        private Vector3 _velocity;
        private float _camVelocity;

        protected override void OnInit()
        {
            base.OnInit();
            _cam = Camera.main;
            _shootTime = Time.time - coolDown;
            ragdoll.OnPreConfig();
        }

        public void OnDamageReceive()
        {
            if(isDead) return;
            ResultManager.Instance.isFail = true;
            isDead = true;
            _levelController.OnLevelFail();
        }

        private async void Start()
        {
            _defCamSize = _levelController.cameraDefSize;
        }

        public void OnFallDamageReceive()
        {
            if(isDead) return;
            ResultManager.Instance.isFail = true;
            isDead = true;
            _levelController.OnLevelFail();
            FallDamage();
        }

        public override void HitBomb(Vector3 force)
        {
            base.HitBomb(force);
            simulation.Hide();
        }
        float CustomSmoothStep(float t)
        {
            return t * t * (3 - 2 * t);
        }


        private CancellationTokenSource _camCts;
        private float _time;
        private void LateUpdate()
        {
            if (isDead || _levelController._isComplete || _levelController._isFailed ||
                UIController.Instance.currentUiIsEnable > 0)
            {
                simulation.Hide();
                return;
            }
            if (DevBuildUtil.Instance.devPanelEnable)return;
            
            #if UNITY_EDITOR
            ControllGun();
            if (Input.GetMouseButton(0))
            {
                UIController.Instance.OnButtonHide();
                if (_camCts != null)
                {
                    _camCts.Cancel();
                }
                var clampDistance = Mathf.Clamp(Vector2.Distance(transform.position,
                    _cam.ScreenToWorldPoint(Input.mousePosition)),0,4);
                var size = _cam.orthographicSize;
                _cam.orthographicSize = Mathf.SmoothDamp(size, _defCamSize + clampDistance,ref _camVelocity, 0.1f);
                simulation.SimulatePath(shooter, _velocity, bombPf.transform.localScale.x);
                return;
            }
            if (Input.GetMouseButtonUp(0))
            {
                ShootGun();
            
                simulation.Hide();
                _camCts = new CancellationTokenSource();
                _cam.DOOrthoSize(_defCamSize,0.25f).WithCancellation(_camCts.Token);
            }
#endif
#if  UNITY_ANDROID && !UNITY_EDITOR
            ControllGun();
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                UIController.Instance.OnButtonHide();
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (_camCts != null)
                    {
                        _camCts.Cancel();
                    }
                    var clampDistance = Mathf.Clamp(Vector2.Distance(transform.position,
                        _cam.ScreenToWorldPoint(Input.mousePosition)),0,4);
                    var size = _cam.orthographicSize;
                    _cam.orthographicSize = Mathf.SmoothDamp(size, _defCamSize + clampDistance,ref _camVelocity, 0.1f);
                    simulation.SimulatePath(shooter, _velocity, bombPf.transform.localScale.x);
                    return;
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    ShootGun();

                    simulation.Hide();
                    _camCts = new CancellationTokenSource();
                    _cam.DOOrthoSize(_defCamSize,0.25f).WithCancellation(_camCts.Token);
                }
            }
#endif
        }

        private void ShootGun()
        {
            _shootTime = Time.time;
            Transform clone = Instantiate(bombPf);
            clone.transform.position = shooter.position;
            Bomb bomb = clone.GetComponent<Bomb>();
            _levelController.OnBulletFire();
            bomb._rigidbody.velocity = _velocity;
        }

        [Obsolete("Old Method")]
        private void ControllGun()
        {
            #if UNITY_EDITOR
            Vector3 mouseWorldPosition =
                _cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

            #endif
            #if UNITY_ANDROID && !UNITY_EDITOR
            if (Input.touchCount == 0) return;
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = touch.position;

            Vector3 mouseWorldPosition = _cam.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, 10));
            #endif

            Vector3 direction = mouseWorldPosition - transform.position;
            float angle = Mathf.Atan2(direction.y, -direction.x) * Mathf.Rad2Deg;
            
            if (transform.localScale.x < 0)
            {
                angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            }
            
            holder.localRotation = Quaternion.Euler(-90, -180,angle);

            Debug.Log(angle);
            Vector3 dir = shooter.position - fireAimPos.position;
            dir.z = 0;
            _velocity = bombSpeed * dir.normalized;
            
            if (angle < -90 || angle >= 90)
            {
                holder.localRotation = Quaternion.Euler(-90, -180,angle);
                if (transform.localScale.x > 0)
                {
                    transform.localScale = new Vector3(-0.5f, 0.5f, 1f);
                    return;
                }
                if (transform.localScale.x < 0)
                {
                    transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                    return;
                }
            }

        }
        void ICharacterInteract.OnInit()
        {
            OnInit();
        }
    }
}
