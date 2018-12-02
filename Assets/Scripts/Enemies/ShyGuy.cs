using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ShyGuy : MonoBehaviour
{
    public bool IsDead;

    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _sprite;
    
    private float _gravitySpd = 1.0f;

    [SerializeField] private float _moveSpd = 1.2f;
    [SerializeField] private float _curveSpd = 2.0f;
    
    private Vector2 _prevVelocity = Vector2.zero;
    
    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (IsDead)
        {
            _rb.constraints = RigidbodyConstraints2D.None;
            _sprite.color = Color.Lerp(_sprite.color, Color.gray, 0.1f);
            return;
        }
        
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        var newVelocity = CosineVelocity();
        _rb.velocity = newVelocity; //- _prevVelocity;
        
        _animator.speed = Mathf.Abs(1f * _rb.velocity.x);
        
        if (Mathf.Abs(_rb.velocity.x) >= 0.3f )
            _sprite.flipX = newVelocity.x < 0;

    }

    private Vector2 CosineVelocity()
    {
        return new Vector2(_moveSpd * Mathf.Cos(_curveSpd * Time.realtimeSinceStartup), _rb.velocity.y -_gravitySpd);
    }

    private void OnBecameVisible()
    {
        enabled = true;
    }
    
    private void OnBecameInvisible()
    {
        enabled = false;
        if (IsDead)
        {
            _rb.constraints = RigidbodyConstraints2D.None;
            return;
        }
    }

    public void Kill()
    {
        if (IsDead)
            return;
        IsDead = true;
        _animator.speed = 0;
        _sprite.color *= Color.red;
        _rb.constraints = RigidbodyConstraints2D.None;
        _rb.angularVelocity = UnityEngine.Random.Range(-200f, 200f);
    }
}
