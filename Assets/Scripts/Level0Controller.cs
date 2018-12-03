using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level0Controller : MonoBehaviour
{
    public float xGoal = 45f;
    
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
        if (_player.transform.position.x >= xGoal)
        {
            var bigBlackColor = _bigBlack.color;
            bigBlackColor.a += Time.fixedDeltaTime;
            _bigBlack.color = bigBlackColor;
            if (bigBlackColor.a >= 0.99f)
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    
}
