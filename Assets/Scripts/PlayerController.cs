using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 7.0f;
    [SerializeField] private float _jumpForce = 14.0f;
    [SerializeField] private float _wallSlidingSpeed = 2.0f;
    [SerializeField] private float _wallJumpingTime = 0.2f;
    [SerializeField] private float _wallJumpingDuration = 0.4f;
    [SerializeField] private Vector2 _wallJumpingPower = new Vector2(8.0f, 16.0f);

    [Header("References")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private BoxCollider2D _coll;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Animator _anim;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _pauseAction;

    private GameManager _gameManager;

    [SerializeField] private LayerMask _jumpableGroundLayer;
    [SerializeField] private LayerMask _wallLayer;

    private Vector2 _rawMovementInput;
    private Vector2 _force;

    private bool _isGrounded;

    private bool _isFacingRight;

    private bool _isWallSliding;

    private bool _canWallJump;
    private float _wallJumpDirection;
    private float _wallJumpCounter;
    private float _lastTriedJump;

    private bool _isPaused = false;

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(_coll.bounds.center, _coll.bounds.size - new Vector3(0.05f, 0.0f, 0.0f), 0.0f, Vector2.down, 0.1f, _jumpableGroundLayer)
            || Physics2D.BoxCast(_coll.bounds.center, _coll.bounds.size - new Vector3(0.05f, 0.0f, 0.0f), 0.0f, Vector2.down, 0.1f, _wallLayer);
    }

    private bool IsTouchingWall()
    {
        return Physics2D.BoxCast(_coll.bounds.center, _coll.bounds.size, 0.0f, Vector2.right, 0.1f, _wallLayer) ||
            Physics2D.BoxCast(_coll.bounds.center, _coll.bounds.size, 0.0f, -Vector2.right, 0.1f, _wallLayer);
    }

    private void Flip()
    {
        if (_isFacingRight && _rawMovementInput.x < 0.0f || !_isFacingRight && _rawMovementInput.x > 0.0f)
        {
            _isFacingRight = !_isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    private void BlockWallJump()
    {
        _canWallJump = false;
        _wallJumpCounter = 0.0f;
        Debug.Log("STOP");
    }

    private void WallSlide()
    {
        if (IsTouchingWall() && !_isGrounded && _rawMovementInput.x != 0.0f)
        {
            if (!_isWallSliding)
            {
                _canWallJump = false;
                _wallJumpDirection = -transform.localScale.x;
                _wallJumpCounter = _wallJumpingTime;
                _isWallSliding = true;
                CancelInvoke(nameof(BlockWallJump));
            }
        }
        else if (_isWallSliding)
        {
            _isWallSliding = false;
            if (Time.time < _lastTriedJump + _wallJumpingDuration)
            {
                _lastTriedJump -= _wallJumpingDuration * 2;
                DoJump();
            }
            else
            {
                CancelInvoke(nameof(BlockWallJump));
                Invoke(nameof(BlockWallJump), _wallJumpingDuration);
            }
        }
    }

    private void DoJump()
    {
        _canWallJump = true;
        _rb.velocity = new Vector2(_wallJumpDirection * _wallJumpingPower.x, _wallJumpingPower.y);
        _wallJumpCounter = 0.0f;

        if (transform.localScale.x != _wallJumpDirection)
        {
            _isFacingRight = !_isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    private void Jump()
    {
        if (!_isGrounded)
        {
            _lastTriedJump = Time.time;
        }
        if (_wallJumpCounter > 0.0f && _wallJumpCounter != _wallJumpingTime)
        {
            DoJump();
            _gameManager.PlaySound(1);
        }
        else if (_isGrounded)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
            _gameManager.PlaySound(1);
        }
    }

    private void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _anim = GetComponent<Animator>();
        _coll = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();

        _moveAction = _playerInput.actions["Movement"];
        _jumpAction = _playerInput.actions["Jump"];
        _pauseAction = _playerInput.actions["Pause"];

        _moveAction.performed += e => _rawMovementInput = e.ReadValue<Vector2>();
        _jumpAction.performed += e => Jump();
        _pauseAction.performed += e => 
        {
            if (_isPaused) _gameManager.ClosePauseMenu();
            else _gameManager.OpenPauseMenu();

            _isPaused = !_isPaused;
        };
    }


    private void Update()
    {
        _isGrounded = IsGrounded();
        if (_isGrounded)
        {
            _canWallJump = false;
            _wallJumpCounter = 0.0f;
        }
        WallSlide();
        if (!_canWallJump)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        _isGrounded = IsGrounded();

        _force = new Vector2(_rawMovementInput.x * _moveSpeed, _rb.velocity.y);

        _anim.SetBool("IsWalking", Mathf.Abs(_force.x) > 0.01f);

        if (!_isWallSliding)
        {
            _wallJumpCounter -= Time.deltaTime;
        }

        if (_isWallSliding)
        {
            _force = new Vector2(_force.x, Mathf.Clamp(_force.y, -_wallSlidingSpeed, float.MaxValue));
        }

        _rb.velocity = _force;
    }
}