using Cinemachine;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator)), RequireComponent(typeof(PlayerInput)), RequireComponent(typeof(CharacterController))]

public class Player : MonoBehaviour
{
    // Set on awake
    private Animator _animator;
    private PlayerInput _playerInput;
    private CharacterController _characterController;
    private Camera _camera;

    // Set in inpsector
    [SerializeField] 
    private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] 
    private LayerMask _aimingLayerMask = new LayerMask();
    [SerializeField]
    private Transform _followTransform;


    // This will contain basic controls based on out key ipnuts
    private Vector2 _direction;
    private bool _isMoving;
    private bool _isSprinting;
    private bool _isAiming;

    // Private fields
    private float _speed;
    private Vector2 _playerVelocity;
    private Vector2 _smoothedPlayerVelocity;
    private Vector3 _aimTarget;
    private Vector3 _rotation = Vector3.zero;
    private Vector3 _movement;

    // Public fields
    public float movementSpeed = 1f;
    public float runSpeed = 3f;
    public float smoothTime = 1f;
    public float rotationSpeed = 10f;
    public bool isCursorLocked = true;


    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _characterController = GetComponent<CharacterController>();
        _camera = Camera.main;
    }
    private void Start()
    {
        SetCursorState(isCursorLocked);
    }

    public void OnMove(InputAction.CallbackContext _value)
    {
        _direction = _value.ReadValue<Vector2>();
        Debug.Log("Move: " + _direction);
        if (_direction == Vector2.zero)
            _isMoving = false;
        else
            _isMoving = true;
        _movement.x = _direction.x;
        _movement.z = _direction.y;
    }

    public void OnSprint(InputAction.CallbackContext _value)
    {
        _isSprinting = _value.ReadValueAsButton();

        if(_value.started)
            _isSprinting = true;
        else if(_value.canceled)
            _isSprinting = false;
    }
    
    public void OnAim(InputAction.CallbackContext _value)
    {
        _isAiming = _value.ReadValueAsButton();
    }

    private void Update()
    {
        if(!_isAiming)
        {
            CasualMovement(Time.fixedDeltaTime);
            SetAnimationActiveLayer(_animator, 1, 0, Time.deltaTime, 10);
            //SetAnimationActiveLayer(_animator, 2, 0, Time.deltaTime, 10);
            _virtualCamera.Priority = 9;

        }
        else
        {
            SetAnimationActiveLayer(_animator, 1, 1, Time.deltaTime, 10);
            //SetAnimationActiveLayer(_animator, 2, 1, Time.deltaTime, 10);
            _virtualCamera.Priority = 11;

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _speed = _isSprinting ? runSpeed : !_isMoving ? 0 : movementSpeed;

        _playerVelocity = Vector2.SmoothDamp(_playerVelocity, _direction, ref _smoothedPlayerVelocity, smoothTime);

        Vector3 move = new Vector3(_playerVelocity.x, 0, _playerVelocity.y);
        move = move.x * _camera.transform.right.normalized + move.z * _camera.transform.forward.normalized;
        move.y = 0;

        _characterController.Move(move * Time.fixedDeltaTime * _speed);
        
        Quaternion rotation = Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.fixedDeltaTime * rotationSpeed);
        
    }

    private void LateUpdate()
    {
        _animator.SetFloat("Forward", Mathf.Lerp(_playerVelocity.y, _playerVelocity.y * (_isSprinting ? runSpeed : _speed), Time.deltaTime * 50));
        _animator.SetFloat("Right", _playerVelocity.x * (_isSprinting ? runSpeed : _speed));

        _animator.SetBool("IsMoving", _isMoving);

        if (_isSprinting)
            _animator.SetFloat("SpeedMultiplier", runSpeed);       
        else
            _animator.SetFloat("SpeedMultiplier", _speed);
    }

    void CasualMovement(float dt)
    {
        if(_isMoving)
            _animator.SetFloat("Speed", _speed, 0.1f, dt);
        else
            _animator.SetFloat("Speed", 0);
        
    }

    private void SetAnimationActiveLayer(Animator animator, int layer, int transitionValue, float dt, float rateOfChange)
    {
        animator.SetLayerWeight(layer, Mathf.Lerp(animator.GetLayerWeight(layer), transitionValue, rateOfChange * dt));
    }

    private void SetCursorState(bool state)
    {
        Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void OnLookPerformed(InputAction.CallbackContext _context)
    {
        Vector2 input = _context.ReadValue<Vector2>();

        _rotation.x += (input.x * Time.deltaTime * 2f);
        _rotation.y += (-input.y * Time.deltaTime * 2f);

        _rotation.y = Mathf.Clamp(_rotation.y, -45, 45);

        transform.forward = new Vector3(transform.forward.x + input.x, 0, transform.forward.z + input.y);
        transform.rotation = Quaternion.Euler(0, _rotation.x, 0);
        _followTransform.localRotation = Quaternion.Euler(_rotation.y, 0, 0);
    }
}
