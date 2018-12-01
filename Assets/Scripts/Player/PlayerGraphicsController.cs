using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerGraphicsController : MonoBehaviour
{
    [NotNull]
    private PlayerController _playerController = null;
    private SpriteRenderer _sprite;
    
    // Start is called before the first frame update
    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _playerController = GetComponentInParent<PlayerController>();
    }

    private void FixedUpdate()
    {
        // Change direction sprite is facing
        if (_playerController.GetInputDirX() > 0)
            _sprite.flipX = false;
        else if (_playerController.GetInputDirX() < 0)
            _sprite.flipX = true;
    }
}
