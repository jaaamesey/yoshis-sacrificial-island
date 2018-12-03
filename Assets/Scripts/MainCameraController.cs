using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    [NotNull] [SerializeField] private PlayerController _player = null;
    [NotNull] [SerializeField] private Canvas _canvas = null;

    private float _xLimit = 0.6f;

    private Vector3 _scaryOffset = Vector3.zero;

    private Vector3 _finalOffset = new Vector3(0f, 0f, 0f);

    private float _scaryOffsetXLimit = 2.4f;

    private float _requiredVelocity = 4.1f;
    
    private bool _isShaking = false;
    private float _shakeAmount = 1f;

    private void Start()
    {
        // Special system stuff
        // Turn off HDR if on WebGL (apparently removes filtering)
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            GetComponent<Camera>().allowHDR = false;
    }

    private void FixedUpdate()
    {
        var prevPos = transform.position;
        
        transform.position -= _finalOffset;
        _finalOffset = new Vector3();
        // Handle screen shake
        var screenShakeVector = new Vector3();
        if (_isShaking)
        {
            screenShakeVector.x = Random.Range(-_shakeAmount, _shakeAmount);
            screenShakeVector.y = Random.Range(-_shakeAmount, _shakeAmount);
        }

        _finalOffset += screenShakeVector;


        // All the weird lerping stuff

        // Handle scary offset
        if (Mathf.Abs(_player.GetVelocity().x) > _requiredVelocity)
            _scaryOffset.x += _player.GetInputDirX() * 4f * Time.fixedDeltaTime;

        var newPos = transform.position;

        var relativePos = _player.transform.position - transform.position + _scaryOffset - _finalOffset;

        var insideSafeZone = relativePos.x > -_xLimit && relativePos.x < _xLimit;


        if (!insideSafeZone || Mathf.Abs(_player.GetVelocity().x) > _requiredVelocity)
        {
            newPos.x = _player.transform.position.x - (Mathf.Sign(relativePos.x) * _xLimit);

            _scaryOffset.x = Mathf.Clamp(_scaryOffset.x, -_scaryOffsetXLimit, _scaryOffsetXLimit);
            transform.position = newPos + _scaryOffset;
        }

        transform.position += _finalOffset;

        var posDelta = Mathf.Abs((transform.position - prevPos).magnitude) ;

        // Apply smoothing
        if (!_isShaking && posDelta >= 0.17f)
        {
            transform.position = 0.5f * (transform.position + prevPos);
            print("Camera jump fix");
        }
        
        UpdateUi();
    }

    public void UpdateUi()
    {
        var healthGreenRectTransform = _canvas.transform.Find("Health/Green").GetComponent<RectTransform>();
        healthGreenRectTransform.sizeDelta = new Vector2(_player.Health, healthGreenRectTransform.sizeDelta.y);
    }
    
    public void StartScreenShake(float amount, float duration = -1)
    {
        _shakeAmount = amount * 0.2f;
        _isShaking = true;

        if (duration >= 0)
            Invoke(nameof(StopScreenShake), duration);
    }

    public void StopScreenShake()
    {
        _isShaking = false;
    }
}