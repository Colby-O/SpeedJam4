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
    [SerializeField] private Vector2 _wallJumpingPower= new Vector2(8.0f, 16.0f);

    [Header("References")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private BoxCollider2D _coll;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Animator _anim;

    private InputAction _moveAction;
    private InputAction _jumpAction;

    [SerializeField] private LayerMask _jumpableGroundLayer;
    [SerializeField] private LayerMask _wallLayer;

    private Vector2 _rawMovementInput;
    [SerializeField] private Vector2 _force;

    [SerializeField] private bool _isGrounded;

    [SerializeField] private bool _isFacingRight;

    [SerializeField] private bool _isWallSliding;

    [SerializeField] private bool _isWallJumping;
    [SerializeField] private float _wallJumpDirection;
    [SerializeField] private float _wallJumpCounter;

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(_coll.bounds.center, _coll.bounds.size, 0.0f, Vector2.down, 0.1f, _jumpableGroundLayer);
    }

    private bool isTouchingWall()
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

    private void StopWallJump()
    {
        _isWallJumping = false;
    }

    private void WallJump()
    {
        if (_isWallSliding)
        {
            _isWallJumping = false;
            _wallJumpDirection = -transform.localScale.x;
            _wallJumpCounter = _wallJumpingTime;

            CancelInvoke(nameof(StopWallJump));
        } 
        else
        {
            _wallJumpCounter -= Time.deltaTime;
        }
    }

    private void WallSlide()
    {
        if (isTouchingWall() && !IsGrounded() && _force.x != 0.0f)
        {
            _isWallSliding = true;
            _force = new Vector2(_force.x, Mathf.Clamp(_force.y, -_wallSlidingSpeed, float.MaxValue));
        } 
        else
        {
            _isWallSliding = false;
        }
    }

    private void Jump()
    {
        if (_wallJumpCounter > 0.0f)
        {
            _isWallJumping = true;
            _rb.velocity = new Vector2(_wallJumpDirection * _wallJumpingPower.x, _wallJumpingPower.y);
            _wallJumpCounter = 0.0f;
        } 
        else
        {
            _rb.velocity = IsGrounded() ? new Vector2(_rb.velocity.x, _jumpForce) : _rb.velocity;
        }

        if (transform.localScale.x != _wallJumpDirection)
        {
            _isFacingRight = !_isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _coll = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();

        _moveAction = _playerInput.actions["Movement"];
        _jumpAction = _playerInput.actions["Jump"];

        _moveAction.performed += e => _rawMovementInput = e.ReadValue<Vector2>();
        _jumpAction.performed += e => Jump();
    }


    private void Update()
    {
        _isGrounded = IsGrounded();
        _force = new Vector2(_rawMovementInput.x * _moveSpeed, _rb.velocity.y);

         _anim.SetBool("IsWalking", Mathf.Abs(_force.x) > 0.01f);

        WallSlide();
        WallJump();
        if (!_isWallJumping)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        _rb.velocity = _force;
    }
}
