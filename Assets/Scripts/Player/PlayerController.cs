using System;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public vars
    public float Health = 100f;

    // Constants
    private const float SinglePixel = 3.125f / 32f;

    // Timer lengths
    private const float JumpSafetyTimerTime = 0.3f;
    private const float JumpButtonLeewayTimerTime = 0.15f;
    private const float FlutterJumpTimerTime = 1.0f;
    private const float FlutterJumpCooldownTimerTime = 0.191f;
    private const float HurtTimerTime = 0.2f;

    [NotNull] [SerializeField] private GroundCheck _groundCheck = null;
    [NotNull] [SerializeField] private PlayerGraphicsController _graphicsController = null;
    [NotNull] private Rigidbody2D _rb;

    [NotNull] private MainCameraController _cameraController = null;

    private float _movementSpd = 50.0f * SinglePixel;
    private float _accelSpd = 1.5f;
    private float _jumpHeightImpulse = 9.0f;
    private float _flutterJumpAmount = 2.4f;
    private float _fastFallImpulse = 2.0f;
    private float _gravitySpd = 5.0f * SinglePixel;

    private int _inputDirX = 0;
    private float _xSpd = 0.0f;

    private Vector2 _knockbackVector = Vector2.zero;

    private Vector2 _relativeVelocity = new Vector2();

    private int _flutterJumpsBeforeLandingCount = 0;
    private float _flutterJumpIncidentYVelocity = 0;

    private bool _isJumpHeldDown = false;
    private bool _isJumpJustPressed = false;
    private bool _isJumpJustReleased = false;

    private bool _invincible = false;

    // Timers
    private float _jumpSafetyTimer = 0.0f;
    private float _jumpButtonLeewayTimer = 0.0f;
    private float _flutterJumpTimer = 0.0f;
    private float _flutterJumpCooldownTimer = 0.0f;
    private float _hurtTimer = 0.0f;

    // Stopwatches
    private float _jumpHoldTime = -1.0f;
    private float _speedRampUpTime = 0f;

    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        Debug.Assert(Camera.main != null, "Camera.main != null");
        _cameraController = Camera.main.GetComponent<MainCameraController>();

        // Special system stuff
        // Turn off interpolation if on WebGL
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            _rb.interpolation = RigidbodyInterpolation2D.None;
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

        _hurtTimer -= Time.fixedDeltaTime;
        _hurtTimer = Mathf.Clamp(_hurtTimer, 0.0f, HurtTimerTime);
    }

    // Update is called once per frame
    private void Update()
    {
        ProcessInput();
    }

    private void FixedUpdate()
    {
        if (_hurtTimer > 0.01f)
        {
            _inputDirX = 0;
        }


        // Store relative velocity of whatever is being stood on
        _relativeVelocity = new Vector2();
        Rigidbody2D objectBeingStoodOn = null;

        // Store force vector that can be used later
        var pendingForceVector = new Vector2();

        // Handle stomping on things
        foreach (var col in _groundCheck.GetOverlappingColliders())
        {
            // Generic rigidbody check
            var otherRb = col.gameObject.GetComponent<Rigidbody2D>();
            if (otherRb != null)
            {
                objectBeingStoodOn = otherRb;
                _relativeVelocity = otherRb.velocity;
            }

            // Shy guy check
            var shyGuy = col.gameObject.GetComponent<ShyGuy>();
            if (shyGuy != null)
            {
                // Kill ShyGuy
                if (!shyGuy.IsDead && _hurtTimer < 0.1f)
                {
                    // Force jump (lil' trick)

                    if (_flutterJumpTimer > 0.0f)
                        _flutterJumpTimer = 0.0f;
                    _jumpButtonLeewayTimer = 0.1f;
                    // Shitton of effects
                    PlaySound("punch");
                    PlaySound("hit");
                    PlayParticleEffect("white_hit", shyGuy.transform.position);
                    PlayParticleEffect("red_hit", shyGuy.transform.position);
                    _cameraController.StartScreenShake(0.8f, 0.4f);
                    shyGuy.Kill();
                }
            }
        }


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

        if (_flutterJumpTimer <= 0.0f)
            _speedRampUpTime += _accelSpd * Time.fixedDeltaTime;
        else
            _speedRampUpTime += 0.5f * _accelSpd * Time.fixedDeltaTime;

        _xSpd = _inputDirX * rampUp * _movementSpd;

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

            // Also play jump sound
            PlaySound("yoshi_jump");
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
        if ((_jumpButtonLeewayTimer > 0.0f ||
             (_isJumpHeldDown && _flutterJumpsBeforeLandingCount < 1) && _rb.velocity.y < -0.5f) &&
            _rb.velocity.y < -0.05f)
        {
            _flutterJumpIncidentYVelocity = _rb.velocity.y;
            _flutterJumpsBeforeLandingCount++;
            // If not already flutter jumping and cooldown is not active, make Yoshi flutter jump
            if (_flutterJumpTimer <= 0.0f && _flutterJumpCooldownTimer <= 0.0f)
            {
                _flutterJumpTimer = FlutterJumpTimerTime;
                // Also play flutter jump sound
                Invoke(nameof(PlayFlutterSound), 0.3f);
            }
        }

        if (!_isJumpHeldDown && _flutterJumpTimer > 0.2f)
            _flutterJumpTimer = 0.1f;


        // Do flutter jump stuff if flutter jumping
        if (_flutterJumpTimer > 0.0f)
        {
            var mod = 1.0f;
            if (_flutterJumpsBeforeLandingCount > 1)
            {
                mod = 0.5f;
            }

            var YVelocityChange = _flutterJumpAmount *
                                  (1.15f * (Mathf.Sin(2 * (_flutterJumpTimer) * Mathf.PI + 0.25f) + 0.2f * mod) +
                                   0.2f * mod);
            _rb.velocity = new Vector2(_rb.velocity.x,
                (0.7f * _flutterJumpIncidentYVelocity) + YVelocityChange + _gravitySpd);
            _flutterJumpCooldownTimer = FlutterJumpCooldownTimerTime;

            movementSpdModifier *= 0.55f;
        }

        if (Input.GetButton("Tongue") && GetComponentInChildren<Tongue>().TongueTime <= 0.25f && !CanJump())
            movementSpdModifier *= 0.5f;

        // Handle movement 
        var newVelocity = _rb.velocity;

        newVelocity.x = _xSpd * movementSpdModifier;

        if (_inputDirX == 0 && Mathf.Abs(_rb.velocity.x) > 0.15f)
            newVelocity.x = (_rb.velocity.x - _relativeVelocity.x) * 0.9f;

        newVelocity.y -= _gravitySpd;

        // Add relative x velocity of whatever is being stood on
        newVelocity.x += _relativeVelocity.x;

        _rb.velocity = newVelocity;

        // Normalise rotation in the air
        if (_jumpHoldTime > 0.0f)
            _rb.rotation = Mathf.Lerp(_rb.rotation, 0f, 0.4f);


        // Prevent falling into the ground too much when not moving
        if (Math.Abs(newVelocity.x) < 0.5f && _inputDirX == 0 && _jumpButtonLeewayTimer <= 0.0f &&
            _flutterJumpTimer <= 0.0f && IsGrounded())
        {
            _rb.velocity = new Vector2(0f, _rb.velocity.y);
        }

        // Limit falling velocity
        if (_rb.velocity.y < -10f)
            _rb.velocity = new Vector2(_rb.velocity.x, -10f);

        _rb.velocity += _knockbackVector;
        _knockbackVector *= 0.9f;

        // Clamp rotation
        _rb.angularVelocity = Mathf.Clamp(_rb.angularVelocity, -100f, 100f);
        _rb.rotation = Mathf.Clamp(_rb.rotation, -20f, 20f);


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

    public Vector2 GetVelocity()
    {
        return _rb.velocity;
    }

    public Vector2 GetRelativeVelocity()
    {
        return _relativeVelocity;
    }


    public float GetHurtTimer()
    {
        return _hurtTimer;
    }

    public float GetFlutterJumpTime()
    {
        return _flutterJumpTimer;
    }

    public bool IsGrounded()
    {
        return _groundCheck.IsColliding();
    }

    public void PlaySound(string soundName)
    {
        var sounds = transform.Find("Sounds");
        var sound = sounds.transform.Find(soundName);
        sound.GetComponent<AudioSource>().enabled = true;
        sound.GetComponent<AudioSource>().Play();
    }

    public void PlayParticleEffect(string effectName, Vector2 pos)
    {
        var effects = transform.Find("Effects");
        var effect = effects.transform.Find(effectName);

        effect.transform.position = pos;

        effect.gameObject.SetActive(true);
        effect.GetComponent<ParticleSystem>().Play();
    }

    public void PlayFlutterSound()
    {
        if (_flutterJumpTimer <= 0.0f)
            return;
        PlaySound("yoshi_flutter");
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1.0f;
    }

    public void Hurt(Vector2 knockbackDir = default(Vector2))
    {
        if (_invincible)
            return;

        _graphicsController.GetComponent<SpriteRenderer>().color = Color.red;
        PlaySound("yoshi_hurt");
        _invincible = true;
        _hurtTimer = HurtTimerTime;


        knockbackDir.y = 0f;

        _knockbackVector = new Vector2();
        _knockbackVector += 3f * knockbackDir;

        Health -= 10f;

        Invoke(nameof(SetNotInvincible), 0.45f);
    }

    public void SetNotInvincible()
    {
        _invincible = false;
        _hurtTimer = 0.0f;
    }
}