using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class Archer : MonoBehaviour
{
    [SerializeField] private ArcherVisual _visual;
    [SerializeField] private Transform _cameraPivotTransform;
    [SerializeField] private Transform _arrowSpawnPoint;
    [SerializeField] private Transform _shootTargetPoint;
    [SerializeField] private GameObject _arrowPF;
    [SerializeField] private CinemachineCamera _aimCamera;
    [SerializeField] private Canvas _aimUI;
    [SerializeField] private Rig _aimRig;
    private PlayerInput _playerInput;
    private Vector3 _moveDirection;
    private Vector2 _moveInput;
    private Quaternion _lookRotation;
    private float _animationBlend;
    private bool _isAiming;
    private float _cameraPitch;
    private float _cameraYaw;
    private readonly float _bottomCameraClamp = -30f;
    private readonly float _topCameraClamp = 70f;
    private readonly float _topAimCameraClamp = 30f;
    private Arrow _currentArrow;








    private Transform CameraPivotTransform { get => _cameraPivotTransform;}
    private ArcherVisual Visual { get => _visual;}
    private PlayerInput PlayerInput { get => _playerInput; set => _playerInput ??= value; }
    private CinemachineCamera AimCamera { get => _aimCamera;}
    private float BottomCameraClamp => _bottomCameraClamp;
    private float TopCameraClamp => _topCameraClamp;
    private float CameraYaw { get => _cameraYaw; set => _cameraYaw = value; }
    private float CameraPitch { get => _cameraPitch; set => _cameraPitch = value; }
    private Vector3 MoveDirection { get => _moveDirection; set => _moveDirection = value; }
    private Vector2 MoveInput { get => _moveInput; set => _moveInput = value; }
    private float AnimationBlend { get => _animationBlend; set => _animationBlend = value; }
    private bool IsAiming { get => _isAiming; set => _isAiming = value; }
    private Quaternion LookRotation { get => _lookRotation; set => _lookRotation = value; }
    private Canvas AimUI { get => _aimUI;}
    private float TopAimCameraClamp => _topAimCameraClamp;
    private Transform ArrowSpawnPoint { get => _arrowSpawnPoint;}
    private GameObject ArrowPF { get => _arrowPF;}
    private Arrow CurrentArrow { get => _currentArrow; set => _currentArrow = value; }
    public Transform ShootTargetPoint { get => _shootTargetPoint;}
    private Rig AimRig { get => _aimRig;}

    private void Awake()
    {
        Cursor.visible = false;

        PlayerInput = GetComponent<PlayerInput>();
        PlayerInput.actions["Look"].performed += OnLookChange;
        PlayerInput.actions["Look"].canceled += OnLookChange;
        PlayerInput.actions["Move"].performed += OnMoveChange;
        PlayerInput.actions["Move"].canceled += OnMoveChange;
        PlayerInput.actions["Aim"].performed += OnAimChange;
        PlayerInput.actions["Aim"].canceled += OnAimChange;
        PlayerInput.actions["Attack"].performed += Charge;
        PlayerInput.actions["Attack"].canceled += Shoot;
    }

    

    private void Update()
    {
        Move();
        Look();

        if (IsAiming)
        {
            ShootTargetPoint.position = GetTargetShootPoint();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(ShootTargetPoint.position, 0.1f);
    }



    private void OnLookChange(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();

        CameraYaw += lookInput.x;
        CameraPitch += lookInput.y;

        float topClamp = IsAiming ? TopAimCameraClamp : TopCameraClamp;

        CameraYaw = ClampAngle(CameraYaw, -360f, 360f);
        CameraPitch = ClampAngle(_cameraPitch, BottomCameraClamp, topClamp);
        // Apply the rotation to the camera pivot
        LookRotation = Quaternion.Euler(CameraPitch, CameraYaw, 0f);
    }



    private void OnMoveChange(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();

        if (MoveInput != Vector2.zero)
        {
            Transform cameraTransform = Camera.main.transform;

            
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0f;
            cameraForward.Normalize();

            Vector3 cameraRight = cameraTransform.right;
            cameraRight.y = 0f;
            cameraRight.Normalize();

            MoveDirection = (cameraForward * MoveInput.y + cameraRight * MoveInput.x).normalized;
        }
        else
        {
            MoveDirection = Vector3.zero;
        }
    }


    private void OnAimChange(InputAction.CallbackContext context)
    {
        IsAiming = context.ReadValueAsButton();

        if (IsAiming)
        {
            AimCamera.Priority = 20;
            Visual.AimVisual(true);
            AimRig.weight = 1f;
        }
        else
        {
            AimCamera.Priority = 0;
            Visual.AimVisual(false);
            AimRig.weight = 0f;
        }
    }

    private void Charge(InputAction.CallbackContext context)
    {
        if (IsAiming)
        {
            CurrentArrow = Instantiate(ArrowPF, ArrowSpawnPoint.position, ArrowSpawnPoint.rotation, ArrowSpawnPoint).GetComponent<Arrow>();
            CurrentArrow.Archer = this;
        }
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        if (IsAiming)
        {
            Visual.ShootVisual();

            CurrentArrow.StartShooting = true;
        }
    }




    private void Move()
    {
        float targetVelocity = MoveInput != Vector2.zero ? 1f : 0f;
        float rotationSpeed = 10f;
        float speedChangeRate = 10f;

        if (transform.forward != MoveDirection && MoveDirection != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, MoveDirection, Time.deltaTime * rotationSpeed);

        }

        AnimationBlend = Mathf.Lerp(AnimationBlend, targetVelocity, Time.deltaTime * speedChangeRate);
        Visual.MoveVisual(AnimationBlend);
    }

    private void Look()
    {
        CameraPivotTransform.rotation = LookRotation;
        
        if (IsAiming)
        {
            ShowAimUI();
            Quaternion aimRotation = Quaternion.Euler(0, LookRotation.eulerAngles.y, 0);
            transform.rotation = aimRotation;
        }
        else
        {
            if (AimUI.isActiveAndEnabled)
            {
                HideAimUI();
            }
        }
    }

    
    private Vector3 GetTargetShootPoint()
    {
        Vector2 sceenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(sceenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        else
        {
            return ray.origin + ray.direction * 30f;
        }
    }


    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void ShowAimUI()
    {
        AimUI.gameObject.SetActive(true);
    }

    private void HideAimUI()
    {
        AimUI.gameObject.SetActive(false);
    }


}
