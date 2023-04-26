using Cinemachine;
using System.Collections;
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
    private Transform _followTransform;
    [SerializeField]
    private Transform _pauseScreen;


    // This will contain basic controls based on out key ipnuts
    private Vector2 _direction;
    private bool _isMoving;
    private bool _isSprinting;
    private bool _isWaving;

    // Private fields
    private float _speed;
    private Vector2 _playerVelocity;
    private Vector2 _smoothedPlayerVelocity;
    private Vector3 _rotation = Vector3.zero;
    private Vector3 _movement;
    private float _jumpVelocity = 0;
    private bool _isPaused = false;

    // Public fields
    public float movementSpeed = 1f;
    public float runSpeed = 2f;
    public float smoothTime = 1f;
    public float rotationSpeed = 10f;
    public bool isCursorLocked = true;
    public Vector2 gravity = new Vector2(0, -9.8f);

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
        if (_direction == Vector2.zero)
            _isMoving = false;
        else
            _isMoving = true;
        _movement.x = _direction.x;
        _movement.z = _direction.y;
    }

    public void OnWave(InputAction.CallbackContext _value)
    {
        if (!_isWaving)
            StartCoroutine(Waving_CR());
        
    }

    public void OnSprint(InputAction.CallbackContext _value)
    {
        _isSprinting = _value.ReadValueAsButton();

        if(_value.started)
            _isSprinting = true;
        else if(_value.canceled)
            _isSprinting = false;
    }

    public void OnJump(InputAction.CallbackContext _value)
    {
        if (_value.started && _characterController.isGrounded)
        {
            _animator.SetBool("Jumping", true);
            _jumpVelocity = Mathf.Sqrt(-2 * Physics.gravity.y);
        }
        else if (_value.canceled)
            _animator.SetBool("Jumping", false);
    }

    public void OnPause(InputAction.CallbackContext _value)
    {
        _isPaused = !_isPaused;

        _pauseScreen.gameObject.SetActive(_isPaused);
        SetCursorState(!_isPaused);
    }

    private void Update()
    {
        if (_isPaused)
            return;

        if(!_isWaving)
        { 
            SetAnimationActiveLayer(_animator, 1, 0, Time.deltaTime, 10);
        }
        else
        {
            SetAnimationActiveLayer(_animator, 1, 1, Time.deltaTime, 10);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_isPaused)
            return;

        _speed = _isSprinting ? runSpeed : !_isMoving ? 0 : movementSpeed;

        _playerVelocity = Vector2.SmoothDamp(_playerVelocity, _direction * _speed, ref _smoothedPlayerVelocity, smoothTime);

        Vector3 move = new Vector3(_playerVelocity.x, 0, _playerVelocity.y);
        move = move.x * _camera.transform.right.normalized + move.z * _camera.transform.forward.normalized;
        move.y = _jumpVelocity;

        _characterController.Move(move * Time.fixedDeltaTime);

        _jumpVelocity += gravity.y * Time.deltaTime;

        Quaternion rotation = Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.fixedDeltaTime * rotationSpeed);

    }

    private void LateUpdate()
    {
        if (_isPaused)
            return;

        _animator.SetFloat("Forward", _playerVelocity.y);
        _animator.SetFloat("Right", _playerVelocity.x);

        _animator.SetBool("IsMoving", _isMoving);

        if (_isSprinting)
            _animator.SetFloat("SpeedMultiplier", runSpeed);       
        else
            _animator.SetFloat("SpeedMultiplier", _speed);
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
        if (_isPaused)
            return;

        Vector2 input = _context.ReadValue<Vector2>();

        _rotation.x += (input.x * Time.deltaTime * 2f);
        _rotation.y += (-input.y * Time.deltaTime * 2f);

        _rotation.y = Mathf.Clamp(_rotation.y, -45, 45);

        transform.forward = new Vector3(transform.forward.x + input.x, 0, transform.forward.z + input.y);
        transform.rotation = Quaternion.Euler(0, _rotation.x, 0);
        _followTransform.localRotation = Quaternion.Euler(_rotation.y, 0, 0);

    }

    IEnumerator Waving_CR()
    {
        _isWaving = true;
        _animator.SetTrigger("IsWaving");
        yield return new WaitForSeconds(3.5f);
        _isWaving = false;

    }
}
