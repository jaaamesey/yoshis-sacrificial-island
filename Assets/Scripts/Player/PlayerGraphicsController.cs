using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerGraphicsController : MonoBehaviour
{
    [NotNull]
    private PlayerController _playerController = null;
    private SpriteRenderer _sprite = null;
    private Animator _animator = null;
    
    // Start is called before the first frame update
    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _playerController = GetComponentInParent<PlayerController>();
    }

    private void FixedUpdate()
    {
        // Change direction sprite is facing
        if (_playerController.GetInputDirX() > 0)
            _sprite.flipX = false;
        else if (_playerController.GetInputDirX() < 0)
            _sprite.flipX = true;

        var xSpeed = Mathf.Abs(_playerController.GetVelocity().x) - Mathf.Abs(_playerController.GetRelativeVelocity().x);
        _animator.SetFloat("xSpeed", xSpeed);
        _animator.SetFloat("xInput", Mathf.Abs(_playerController.GetInputDirX()));
        _animator.SetFloat("FlutterTimer", Mathf.Abs(_playerController.GetFlutterJumpTime()));
        _animator.SetBool("OnGround", _playerController.IsGrounded());
        
    }
}
