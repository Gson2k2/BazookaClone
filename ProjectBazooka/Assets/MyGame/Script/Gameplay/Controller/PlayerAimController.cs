using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyGame.Script.CoreGame;
using Sirenix.Utilities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

namespace MyGame.Script.Gameplay.Controller
{
    public class PlayerAimController : RagdollCharacter,ICharacterInteract
    {
        [SerializeField] private float _bombSpeed = 5;
        [SerializeField] private float _coolDown = 3;
        [SerializeField] private Bomb _bombPf;
        [SerializeField] private GameObject _gunGo;
        [SerializeField] private Transform _shooter;
        [SerializeField] private Transform _holder;
        [SerializeField] private GunTrajectory _simulation;
        [SerializeField] private LevelController _levelController;

        private Camera _cam;
        private float _shootTime;
        private float defCamsize;

        Vector3 velocity;

        private void Awake()
        {
            OnInit();
        }

        private void Start()
        {
            defCamsize = _levelController.cameraDefSize;
        }

        public void OnInit()
        {
            isDead = false;
            ActiveRagdoll(false);
            _cam = Camera.main;
            _shootTime = Time.time - _coolDown;
        }

        public void OnDamageReceive()
        {
            //TODO
            if(isDead) return;
            _levelController.OnLevelFail();
        }

        public void OnFallDamageReceive()
        {
            throw new NotImplementedException();
        }

        public override void HitBomb(Vector3 pos)
        {
            base.HitBomb(pos);
            // _gunGo.transform.SetParent(null);
            Rigidbody gunRG = _gunGo.GetComponent<Rigidbody>();

            // gunRG.isKinematic = false;
            //gunRG.AddForce(force);
            
            ActiveRagdoll(true);
            if (ragdoll != null)
            {
                ragdoll.Force(pos);
            }


            _gunGo.GetComponent<Collider>().isTrigger = false;
            _simulation.OnHide();

        }
        
        public bool IsPointerOverUI()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.NameToLayer("UI")))
            {
                Debug.Log("UI Element Hit: " + hit.collider.gameObject.name);
                return true;
            }

            return false;
        }

        private CancellationTokenSource _camCts;
        private bool _isAwaitShooting;
        private void LateUpdate()
        {
            if (isDead || _levelController._isComplete||_levelController._isFailed || UIController.Instance.currentUiIsEnable > 0) return;
            if (DevBuildUtil.Instance.devPanelEnable)return;
            
            
            if (Time.time - _shootTime > _coolDown)
            {
                //TODO Use Touch Input later on mobile build
                GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
#if UNITY_EDITOR
                
                if (Input.GetMouseButtonUp(0) && _isAwaitShooting)
                {
                    _simulation.OnHide();
                    OnGunShot();

                    if (_camCts != null)
                    {
                        _camCts.Cancel();
                    }
                    _camCts = new CancellationTokenSource();
                    _cam.DOOrthoSize(20,0.2f).WithCancellation(_camCts.Token);
                    _isAwaitShooting = false;
                }
                if(Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    _isAwaitShooting = true;
                    var clampDistance = Mathf.Clamp(Vector2.Distance(transform.position,
                        _cam.ScreenToWorldPoint(Input.mousePosition)),0,2);
                    _cam.orthographicSize = 20 + clampDistance;
                    
                    UIController.Instance.OnButtonHide();
                    startPoint = transform.position;
                    _simulation.OnShow();
                    OnGunUpdate();
                    ControlGun();
                    // _simulation.SimulatePath(_shooter, velocity,_bombPf.transform.localScale.x);
                }
                else
                {
                    _holder.localRotation = Quaternion.Euler(60, _lastConfirmAngel, 0);
                }
#endif
#if UNITY_ANDROID
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Ended && _isAwaitShooting)
                    {
                        _simulation.OnHide();
                        OnGunShot();

                        if (_camCts != null)
                        {
                            _camCts.Cancel();
                        }
                        _camCts = new CancellationTokenSource();
                        _cam.DOOrthoSize(20,0.2f).WithCancellation(_camCts.Token);
                        _isAwaitShooting = false;
                    }
                    if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        if(!EventSystem.current.IsPointerOverGameObject())
                        {
                            _isAwaitShooting = true;
                            var clampDistance = Mathf.Clamp(Vector2.Distance(transform.position,
                                _cam.ScreenToWorldPoint(Input.mousePosition)),0,2);
                            _cam.orthographicSize = 20 + clampDistance;
                    
                            UIController.Instance.OnButtonHide();
                            startPoint = transform.position;
                            _simulation.OnShow();
                            OnGunUpdate();
                            ControlGun();
                            // _simulation.SimulatePath(_shooter, velocity,_bombPf.transform.localScale.x);
                        }

                    }
                    else
                    {
                        _holder.localRotation = Quaternion.Euler(60, _lastConfirmAngel, 0);
                    }
                }
#endif
            }

        }

        [SerializeField] private float _lastConfirmAngel;

        private void ShootGun()
        {
            _shootTime = Time.time;
            Bomb clone = Instantiate(_bombPf);
            clone._levelController = _levelController;
            clone._levelController.OnBulletFire();
            clone.transform.position = _shooter.position;
            //clone.Rb.velocity = velocity;
        }
        private void ControlGun()
        {
            // Vector3 direction =mouseWorldPosition - _holder.position;
            Vector3 direction =(startPoint - endPoint).normalized;

            float angle = Mathf.Atan2(-direction.y, direction.x) * Mathf.Rad2Deg;

            angle = Mathf.Clamp(angle, -180, 180);
            var tempAngle = angle;
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            if (angle is < -90 or > 90)
            {
                transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
                tempAngle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
            }

            _lastConfirmAngel = tempAngle;
            _holder.localRotation = Quaternion.Euler(60, tempAngle, 0);

            // Vector3 dir = _shooter.position - _holder.position;
            // dir.z = 0;

        }

        private Vector2 startPoint;
        private Vector2 endPoint;
        private Vector2 direction;
        private Vector2 force;
        private float distance;
        async void OnGunShot()
        {
            await UniTask.DelayFrame(1);
            Bomb clone = Instantiate(_bombPf);
            clone.transform.localPosition = _shooter.position;
            clone._levelController = _levelController;
            clone._levelController.OnBulletFire();
            var clampForce = force.Clamp(new Vector2(-15f,-15f), new Vector2(15f,15f));
            clone.Shoot(clampForce);
        }

        void OnGunUpdate()
        {
            endPoint = _cam.ScreenToWorldPoint(Input.mousePosition);
            distance = Vector2.Distance(startPoint, endPoint);
            distance.Clamp(1, 1);
            direction = (startPoint - endPoint).normalized;
            force = direction * distance * 1.5f;

            // Debug.DrawLine(startPoint,distance);
            
            _simulation.UpdateTrajectory(_holder.position, force);
        }
    }
}
