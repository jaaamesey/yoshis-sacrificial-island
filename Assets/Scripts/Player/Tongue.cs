using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tongue : MonoBehaviour
{
    public Vector3 TonguePlayerOffset = new Vector3(0.4f, 0.08f, 0f);
    public float TongueTime = 0.0f;
    
    private PlayerController _player = null;
    private PlayerGraphicsController _playerGraphicsController = null;
    private LineRenderer _tongueLineRenderer = null;
    private TongueBall _tongueBall = null;
    private SpriteRenderer _tongueBehindSprite = null;

    // Start is called before the first frame update
    private void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _playerGraphicsController =
            GameObject.FindWithTag("PlayerGraphicsController").GetComponent<PlayerGraphicsController>();
        _tongueLineRenderer = GetComponentInChildren<LineRenderer>();
        _tongueBall = GetComponentInChildren<TongueBall>();
        _tongueBehindSprite = transform.Find("TongueBehind").GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (!Input.GetButton("Tongue"))
        {
            TongueTime = 0;
            _tongueLineRenderer.gameObject.SetActive(false);
            _tongueBall.gameObject.SetActive(false);
            _tongueBehindSprite.gameObject.SetActive(false);
            
            _tongueBall.TongueToObjectDistanceJoint.enabled = false;
            _tongueBall.TongueToObjectDistanceJoint.connectedBody = null;
            return;
        }

        _tongueLineRenderer.gameObject.SetActive(true);
        _tongueBall.gameObject.SetActive(true);
        _tongueBehindSprite.gameObject.SetActive(true);

        if (Input.GetButtonDown("Tongue"))
        {
            // Do initial tongue stuff
            
            // Get offset
            var flip = 1;
            if (_playerGraphicsController.Sprite.flipX)
                flip = -1;

            var offset = new Vector3(flip * TonguePlayerOffset.x, TonguePlayerOffset.y, TonguePlayerOffset.z);

            
            _tongueBall.transform.position = _player.transform.position + offset;
            _tongueBall.Rb.velocity = (7f * flip * Vector2.right) + _player.GetVelocity();
            _tongueBall.TongueToObjectDistanceJoint.enabled = false;
            _tongueBall.TongueToObjectDistanceJoint.connectedBody = null;
            
            _player.PlaySound("yoshi_tongue");
        }
        
        RenderTongue();
        TongueTime += Time.fixedDeltaTime;
    }

    private void RenderTongue()
    {
        var flip = 1;
        if (_playerGraphicsController.Sprite.flipX)
            flip = -1;

        var offset = new Vector3(flip * TonguePlayerOffset.x, TonguePlayerOffset.y, TonguePlayerOffset.z);
        _tongueLineRenderer.SetPosition(0, _player.transform.position + offset);
        _tongueLineRenderer.SetPosition(1, _tongueBall.transform.position);

        var tongueBehindPos = _player.transform.position + offset;
        tongueBehindPos.z = 2;
        _tongueBehindSprite.transform.position = tongueBehindPos;
    }
}