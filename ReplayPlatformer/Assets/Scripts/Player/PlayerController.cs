using UnityEngine;
using System.Collections;
using ReplaySystem.Core;
using ReplaySystem.Recording;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    
    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private bool _isGrounded;
    private bool _isDashing;
    private float _dashCooldownTimer;
    private bool _canDash = true;
    
    private Recorder _recorder;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _recorder = FindObjectOfType<Recorder>();
        
        if (transform.localScale == Vector3.zero)
        {
            transform.localScale = Vector3.one;
        }
    }
    
    void Update()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        if (_dashCooldownTimer > 0)
            _dashCooldownTimer -= Time.deltaTime;
        else
            _canDash = true;
    }
    
    void FixedUpdate()
    {
        if (_isDashing) return;
        _rb.linearVelocity = new Vector2(_moveInput.x * moveSpeed, _rb.linearVelocity.y);
    }
    
    public void ResetState()
    {
        _moveInput = Vector2.zero;
        
        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.isKinematic = false; 
        }
        
        _isDashing = false;
        _canDash = true;
        _dashCooldownTimer = 0f;
        
        StopAllCoroutines();

        if (transform.localScale == Vector3.zero)
        {
            transform.localScale = Vector3.one;
        }
        
    }
    
    public void Teleport(Vector3 newPosition)
    {
        if (_rb != null)
        {
            _rb.isKinematic = true;
            _rb.linearVelocity = Vector2.zero;
        }
        
        transform.position = newPosition;
        
        if (_rb != null)
        {
            _rb.isKinematic = false;
        }
    }
    
    public void SetMoveInput(Vector2 input)
    {
        _moveInput = input;
    }
    
    public void Jump()
    {
        if (!_isGrounded || _isDashing) return;
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
    }
    
    public void Dash(Vector2 direction)
    {
        if (!_canDash || _isDashing) return;
        StartCoroutine(DashCoroutine(direction.normalized));
    }
    
    private IEnumerator DashCoroutine(Vector2 direction)
    {
        _isDashing = true;
        _canDash = false;
        _dashCooldownTimer = dashCooldown;
        
        float startTime = Time.time;
        
        while (Time.time < startTime + dashDuration)
        {
            _rb.linearVelocity = direction * dashForce;
            yield return null;
        }
        
        _isDashing = false;
    }
    
    public void RecordMoveInput(Vector2 input)
    {
        if (_recorder != null && _recorder.IsRecording)
        {
            var payload = new MoveCommand.MovePayload { Horizontal = input.x, Vertical = input.y };
            _recorder.RecordCommand(new MoveCommand(_recorder.GetCurrentTick(), payload));
        }
        SetMoveInput(input);
    }
    
    public void RecordJump()
    {
        if (_recorder != null && _recorder.IsRecording)
        {
            var payload = new JumpCommand.JumpPayload { IsJumping = true };
            _recorder.RecordCommand(new JumpCommand(_recorder.GetCurrentTick(), payload));
        }
        Jump();
    }
    
    public void RecordDash(Vector2 direction)
    {
        if (_recorder != null && _recorder.IsRecording)
        {
            var payload = new DashCommand.DashPayload { Direction = direction };
            _recorder.RecordCommand(new DashCommand(_recorder.GetCurrentTick(), payload));
        }
        Dash(direction);
    }
}