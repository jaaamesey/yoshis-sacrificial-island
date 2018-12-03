using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level0Controller : MonoBehaviour
{
    private PlayerController _player = null;
    private MainCameraController _mainCamera = null;
    private SpriteRenderer _bigBlack = null;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _mainCamera = Camera.main.GetComponent<MainCameraController>();
        _bigBlack = GameObject.FindWithTag("BigBlack").GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (_player.transform.position.x >= 45)
        {
            var bigBlackColor = _bigBlack.color;
            bigBlackColor.a += Time.fixedDeltaTime;
            _bigBlack.color = bigBlackColor;
        }
    }
}
