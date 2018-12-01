using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Experimental.UIElements;

public class PlayerController : MonoBehaviour
{
    private const float SinglePixel = 3.125f / 32f;

    // Timer lengths
    private const float JumpSafetyTimerTime = 0.3f;
    private const float JumpButtonLeewayTimerTime = 0.15f;
    private const float FlutterJumpTimerTime = 1.0f;
    private const float FlutterJumpCooldownTimerTime = 0.151f;

    [NotNull] [SerializeField] private GroundCheck _groundCheck;
    [NotNull] private Rigidbody2D _rb;

    private float _movementSpd = 30.0f * SinglePixel;
    private float _accelSpd = 3.0f;
    private float _jumpHeightImpulse = 9.0f;
    private float _flutterJumpAmount = 2.4f;
    private float _fastFallImpulse = 2.0f;
    private float _gravitySpd = 5.0f * SinglePixel;

    private int _inputDirX = 0;
    private float _xSpd = 0.0f;

    private int _flutterJumpsBeforeLandingCount = 0;
    private float _flutterJumpIncidentYVelocity = 0;

    private bool _isJumpHeldDown = false;
    private bool _isJumpJustPressed = false;
    private bool _isJumpJustReleased = false;

    // Timers
    private float _jumpSafetyTimer = 0.0f;
    private float _jumpButtonLeewayTimer = 0.0f;
    private float _flutterJumpTimer = 0.0f;
    private float _flutterJumpCooldownTimer = 0.0f;

    // Stopwatches
    private float _jumpHoldTime = -1.0f;
    private float _speedRampUpTime = 0f;

    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void ProcessInput()
    {
        _inputDirX = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        _isJumpHeldDown = Input.GetButton("Jump");
        _isJumpJustPressed = Input.GetButtonDown("Jump");
        _isJumpJustReleased = Input.GetButtonUp("Jump");

        if (Input.GetButtonDown("Jump"))
            _jumpButtonLeewayTimer = JumpButtonLeewayTimerTime;

        if (Input.GetButtonUp("Jump"))
            _jumpButtonLeewayTimer = 0.0f;
    }

    private void HandleTimers()
    {
        _jumpSafetyTimer -= Time.fixedDeltaTime;
        _jumpSafetyTimer = Mathf.Clamp(_jumpSafetyTimer, 0.0f, JumpSafetyTimerTime);

        _jumpButtonLeewayTimer -= Time.fixedDeltaTime;
        _jumpButtonLeewayTimer = Mathf.Clamp(_jumpButtonLeewayTimer, 0.0f, JumpButtonLeewayTimerTime);

        _flutterJumpTimer -= Time.fixedDeltaTime;
        _flutterJumpTimer = Mathf.Clamp(_flutterJumpTimer, 0.0f, FlutterJumpTimerTime);

        _flutterJumpCooldownTimer -= Time.fixedDeltaTime;
        _flutterJumpCooldownTimer = Mathf.Clamp(_flutterJumpCooldownTimer, 0.0f, FlutterJumpCooldownTimerTime);
    }

    // Update is called once per frame
    private void Update()
    {
        ProcessInput();
    }

    private void FixedUpdate()
    {
        var is_braking = false;
        
        // Determine if braking
        // If input direction is different from the last x speed direction, the player is braking.
        if ((int) Mathf.Sign(_xSpd) != (int) Mathf.Sign(_inputDirX) || _inputDirX == 0)
        {
            is_braking = true;
            _speedRampUpTime = 0f;
        }
        
        var movementSpdModifier = 1.0f;
        var rampUp = Mathf.Clamp(_speedRampUpTime, 0.1f, 1f);
        
        _speedRampUpTime += _accelSpd * Time.fixedDeltaTime;
        
        _xSpd = _inputDirX * rampUp *_movementSpd;

        // Handle jumping
        var canJump = CanJump();

        // Jump if jump button is pressed within leeway amount
        if (_jumpButtonLeewayTimer > 0.0f && canJump)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, 0.0f);
            _rb.AddForce(_jumpHeightImpulse * Vector2.up, ForceMode2D.Impulse);
            _jumpSafetyTimer = JumpSafetyTimerTime;
            // Reset flutter jump count
            _flutterJumpsBeforeLandingCount = 0;
        }

        if (_isJumpHeldDown)
        {
            if (_jumpHoldTime < 0.0f)
                _jumpHoldTime = 0.0f;
            _jumpHoldTime += Time.fixedDeltaTime;
        }
        else if (_isJumpJustReleased || CanJump())
        {
            _jumpHoldTime = -1.0f;
        }

        if (_isJumpJustReleased)
        {
            //_rb.velocity = new Vector2(_rb.velocity.x, 0.0f);
            _rb.AddForce(_fastFallImpulse * Vector2.down, ForceMode2D.Impulse);
        }

        // Handle flutter jump

        // If Yoshi is falling and jump is held down...
        if ((_jumpButtonLeewayTimer > 0.0f || (_isJumpHeldDown && _flutterJumpsBeforeLandingCount < 1) && _rb.velocity.y < -0.5f) && _rb.velocity.y < -0.05f)
        {
            _flutterJumpIncidentYVelocity = _rb.velocity.y;
            _flutterJumpsBeforeLandingCount++;
            // If not already flutter jumping and cooldown is not active, make Yoshi flutter jump
            if (_flutterJumpTimer <= 0.0f && _flutterJumpCooldownTimer <= 0.0f)
            {
                _flutterJumpTimer = FlutterJumpTimerTime;
            }
        }

        if (!_isJumpHeldDown && _flutterJumpTimer > 0.2f)
            _flutterJumpTimer = 0.1f;


        // Do flutter jump stuff if flutter jumping
        if (_flutterJumpTimer > 0.0f)
        {
            var YVelocityChange = _flutterJumpAmount *
                                  (1.15f * (Mathf.Sin(2 * (_flutterJumpTimer) * Mathf.PI + 0.25f) + 0.2f) + 0.2f);
            _rb.velocity = new Vector2(_rb.velocity.x, (0.7f * _flutterJumpIncidentYVelocity) + YVelocityChange + _gravitySpd);
            _flutterJumpCooldownTimer = FlutterJumpCooldownTimerTime;

            movementSpdModifier *= 0.55f;
        }

        // Handle movement 
        var newVelocity = _rb.velocity;

        newVelocity.x = _xSpd * movementSpdModifier;
        newVelocity.y -= _gravitySpd;

        _rb.velocity = newVelocity;

        // Clamp rotation
        
        _rb.rotation = Mathf.Clamp(_rb.rotation, -10f, 10f);
        
        if (_jumpHoldTime > 0.0f)
            _rb.rotation = Mathf.Lerp(_rb.rotation, 0f, 0.4f);
        
        HandleTimers();
    }

    private bool CanJump()
    {
        if (!_groundCheck.IsColliding())
            return false;

        if (_jumpSafetyTimer > 0.001)
            return false;

        return true;
    }

    public float GetInputDirX()
    {
        return _inputDirX;
    }
}