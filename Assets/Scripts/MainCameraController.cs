using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    [NotNull] [SerializeField] private PlayerController _player = null;

    private float _xLimit = 0.6f;

    private Vector3 _scaryOffset = Vector3.zero;
    
    [SerializeField]
    private Vector3 _finalOffset = new Vector3(0f, 0f, 0f);

    private float _scaryOffsetXLimit = 2.4f;

    private void FixedUpdate()
    {
        // Handle scary offset
        if (Mathf.Abs(_player.GetVelocity().x) > 4.5f)
            _scaryOffset.x += _player.GetInputDirX() * 4f * Time.fixedDeltaTime;

        var newPos = transform.position;

        var relativePos = _player.transform.position - transform.position + _scaryOffset + _finalOffset;

        var insideSafeZone = relativePos.x > -_xLimit && relativePos.x < _xLimit;


        if (!insideSafeZone || Mathf.Abs(_player.GetVelocity().x) > 4.5f)
        {
            newPos.x = _player.transform.position.x - (Mathf.Sign(relativePos.x) * _xLimit);

            _scaryOffset.x = Mathf.Clamp(_scaryOffset.x, -_scaryOffsetXLimit, _scaryOffsetXLimit);
            transform.position = newPos + _scaryOffset + _finalOffset;
        }
    }


    /*private float _xLimit = 1.4f;
    private float _offsetLimit = 1.6f;
    
    private float _offset = 0.0f;
    
    private float _incidentRelativePosX = 0.0f;

    private bool _wasNotTracking = false;
    
    
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        
        var newPos = transform.position;
        var relativePos = _player.transform.position - transform.position;

        //(relativePos.x >= _xLimit || relativePos.x <= -_xLimit)
        
        if (Mathf.Abs(_player.GetVelocity().x) >= 4.5f)
        {
            
            _offset += 1.5f * _player.GetVelocity().x * Time.deltaTime;
            _offset = Mathf.Clamp(_offset, -_offsetLimit, _offsetLimit);
            newPos.x = _player.transform.position.x + _offset;
            _wasNotTracking = false;
            newPos.x = _player.transform.position.x + _offset;

            if (_wasNotTracking)
            {
                var amountNotTracked = newPos.x - transform.position.x;
                _offset -= amountNotTracked * Time.deltaTime;
                newPos.x -= amountNotTracked;
            }
        }
        else
        {
            _wasNotTracking = true;
        }
        
        
        

        transform.position = newPos;
    }*/
}