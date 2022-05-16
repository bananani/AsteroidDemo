using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private CharacterController _characterController;
    [Header("Camera")]
    [SerializeField]
    private Transform _cameraFollowTarget;
    [SerializeField]
    private float _cameraRotationSpeedX = 0.5f;
    [SerializeField]
    private float _cameraRotationSpeedY = 0.2f;
    private Vector3 _cameraPositionRef;
    [SerializeField]
    private float _cameraSmoothTime = 0.4f;

    [Header("Movement")]
    [SerializeField]
    private float _speed = 7;
    [SerializeField]
    private float _rotationSpeed = 7;
    [SerializeField]
    private float _inputSmoothTime = 0.3f;
    [SerializeField]
    private Camera _mainCamera;

    private Vector3 _processedInputDirectionRef;
    private Vector3 _unprocessedInputDirection;
    private Vector3 _lastInputDirection;
    private Vector2 _inputDirection = Vector2.zero;
    private Vector2 _lookDirection = Vector2.zero;
    private Vector3 _movement;

    [Header("fluff")]
    [SerializeField]
    private ParticleSystem _mainBoosterFX;
    [SerializeField]
    private TrailRenderer _trailRenderer;
    [SerializeField]
    private RectTransform _canvasRect;

    [Header("Asteroid")]
    [SerializeField]
    private LayerMask _asteroidLayermask;
    [SerializeField]
    private RectTransform _targetUI;
    private Asteroid _targetedAsteroid;

    [Header("Missile")]
    [SerializeField]
    private Missile _missile;
    [SerializeField]
    private Transform _missileSpawnPoint;

    private void Start()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        Cursor.visible = false;
        Screen.lockCursor = true;


        _trailRenderer.emitting = false;

    }

    private void Update()
    {
        _inputDirection = Vector3.SmoothDamp(_inputDirection, _unprocessedInputDirection, ref _processedInputDirectionRef, _inputSmoothTime);

        HandleMovement();
        HandleCamera();
        HandleTargetPosition();
    }

    private void HandleMovement()
    {
        transform.rotation *= Quaternion.AngleAxis(_lookDirection.y * _rotationSpeed, Vector3.right) *
                              Quaternion.AngleAxis(-_lookDirection.x * _rotationSpeed, Vector3.up);
        _characterController.Move(transform.rotation * new Vector3(_inputDirection.x, 0, _inputDirection.y) * _speed * Time.deltaTime);
    }

    private void HandleCamera()
    {
        _cameraFollowTarget.rotation *= Quaternion.AngleAxis(_lookDirection.y * _cameraRotationSpeedY, Vector3.right) *
                                        Quaternion.AngleAxis(-_lookDirection.x * _cameraRotationSpeedX, Vector3.up);
        _cameraFollowTarget.transform.position =
            Vector3.SmoothDamp(_cameraFollowTarget.transform.position, transform.position, ref _cameraPositionRef, _cameraSmoothTime);
    }

    private void HandleTargetPosition()
    {
        if (_targetedAsteroid != null)
        {
            _targetUI.anchoredPosition = WorldToCanvas(_targetedAsteroid.transform.position);

            if (!_targetedAsteroid.IsVisible && _targetUI.gameObject.activeSelf)
            {
                _targetUI.gameObject.SetActive(false);
            }
            else if (_targetedAsteroid.IsVisible && !_targetUI.gameObject.activeSelf)
            {
                _targetUI.gameObject.SetActive(true);
            }
        }
        else
        {
            _targetUI.gameObject.SetActive(false);
        }
    }

    private void Shoot()
    {
        var missile = Instantiate(_missile, _missileSpawnPoint.position, _missileSpawnPoint.rotation);

        if (_targetedAsteroid != null)
        {
            missile.Setup(_targetedAsteroid);
        }
    }

    private void SetTarget(Transform target)
    {
        if (target != null)
        {
            _targetedAsteroid = target.GetComponent<Asteroid>();
        }
    }

    private void ClearTarget()
    {
        _targetUI.gameObject.SetActive(false);
        _targetedAsteroid = null;
    }

    private void TryTarget()
    {
        if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out RaycastHit hit, 500f, _asteroidLayermask))
        {
            SetTarget(hit.transform);
            return;
        }

        ClearTarget();
    }

    public Vector2 WorldToCanvas(Vector3 worldPosition)
    {
        Vector3 viewportPosition = _mainCamera.WorldToViewportPoint(worldPosition);

        return new Vector2(viewportPosition.x * _canvasRect.sizeDelta.x - _canvasRect.sizeDelta.x * 0.5f,
            viewportPosition.y * _canvasRect.sizeDelta.y - _canvasRect.sizeDelta.y * 0.5f);
    }

#region InputHandling

    public void OnMove(InputAction.CallbackContext callbackContext)
    {
        switch (callbackContext.phase)
        {
            case InputActionPhase.Canceled:
            case InputActionPhase.Waiting:
            case InputActionPhase.Disabled:
                _unprocessedInputDirection = Vector2.zero;

                if (_mainBoosterFX.isPlaying)
                {
                    _mainBoosterFX.Stop();
                    _trailRenderer.emitting = false;
                }

                break;
            case InputActionPhase.Started:
            case InputActionPhase.Performed:
                _unprocessedInputDirection = callbackContext.ReadValue<Vector2>();

                if (!_mainBoosterFX.isPlaying && _unprocessedInputDirection.y is > .1f or < -.1f)
                {
                    _mainBoosterFX.Play();
                    _trailRenderer.emitting = true;
                }

                break;
        }
    }

    public void OnLook(InputAction.CallbackContext callbackContext)
    {
        _lookDirection = callbackContext.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext callbackContext)
    {
        switch (callbackContext.phase)
        {
            case InputActionPhase.Disabled:
                break;
            case InputActionPhase.Waiting:
                break;
            case InputActionPhase.Started:
                break;
            case InputActionPhase.Performed:
                Shoot();
                break;
            case InputActionPhase.Canceled:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnAim(InputAction.CallbackContext callbackContext)
    {
        switch (callbackContext.phase)
        {
            case InputActionPhase.Disabled:
                break;
            case InputActionPhase.Waiting:
                break;
            case InputActionPhase.Started:
                break;
            case InputActionPhase.Performed:
                TryTarget();
                break;
            case InputActionPhase.Canceled:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

#endregion
}
