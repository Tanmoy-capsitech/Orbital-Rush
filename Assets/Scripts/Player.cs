using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private AudioClip _moveClip, _loseClip, _pointClip;

    [SerializeField] private GameplayManager _gm;
    [SerializeField] private GameObject _explosionPrefab, _scoreParticlePrefab;

    [Header("SPEED PROGRESSION")]
    [SerializeField] private float _baseRotateSpeed = 100f;
    [SerializeField] private float _speedPerScore = 5f;

    [SerializeField] private int _speedUpdateInterval = 5;
    [SerializeField] private float _maxRotateSpeed = 500f;

    private float _currentRotateSpeed;
    private int _lastSpeedUpdateScore = 0;



    private PlayerShield _playerShield;

    private bool canClick;

    private void Awake()
    {
        canClick = true;
        level = 0;
        currentRadius = _startRadius;
        _playerShield = GetComponent<PlayerShield>();
        _currentRotateSpeed = _baseRotateSpeed;
    }
     
     private void UpdateRotateSpeed()
    {
            // Get current score from GameManager
            int currentScore = GameManager.Instance.CurrentScore;
            
            // Only recalculate every X points (performance + smooth steps)
            if (currentScore >= _lastSpeedUpdateScore + _speedUpdateInterval)
            {
                _lastSpeedUpdateScore = currentScore / _speedUpdateInterval * _speedUpdateInterval;
                
                // Calculate new speed: BASE + (SCORE/INTERVAL) * SPEED_PER_SCORE
                _currentRotateSpeed = Mathf.Min(
                    _baseRotateSpeed + (_lastSpeedUpdateScore * _speedPerScore), 
                    _maxRotateSpeed
                );
                
                Debug.Log($"⚡ SPEED UPDATE! Score: {_lastSpeedUpdateScore} → RotateSpeed: {_currentRotateSpeed:F0}");
            }
    }
    private void Update()
    {
        UpdateRotateSpeed();
        
        if(canClick && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ChangeRadius());
            SoundManager.Instance.PlaySound(_moveClip);
        }
    }

    [SerializeField] private float _rotateSpeed;
    [SerializeField] private Transform _rotateTransform;

    private void FixedUpdate()
    {
        transform.localPosition = Vector3.up * currentRadius;
        float rotateValue = _currentRotateSpeed * Time.fixedDeltaTime * _startRadius / currentRadius;
        _rotateTransform.Rotate(0, 0, rotateValue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            if (collision.CompareTag("Obstacle"))
            {
                // Check if shield is active
                if (_playerShield != null && _playerShield.IsShieldActive())
                {
                    Debug.Log("🛡️ Shield absorbed obstacle! No game over.");
                    // Optional: play hit sound, or make a flash
                    return; // Do nothing, shield protects
                }

                // Shield is not active -> normal game over
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                SoundManager.Instance.PlaySound(_loseClip);
                _gm.GameEnded();
                return;
            }

        }

        // if (collision.CompareTag("Score"))
        // {
        //     Destroy(Instantiate(_scoreParticlePrefab, transform.position, Quaternion.identity), 1f);
        //     // SoundManager.Instance.PlaySound(_pointClip);
        //     if (SoundManager.Instance != null)
        //     {
        //         SoundManager.Instance.PlaySound(_pointClip);
        //     }
        //     _gm.UpdateScore();
        //     collision.gameObject.GetComponent<Score>().ScoreAdded();
        //     // if (GameManager.Instance != null)
        //     // {
        //     //     GameManager.Instance.AddScore(1);
        //     // }
        //     // else
        //     // {
        //     //     Debug.LogWarning("GameManager.Instance is null when trying to AddScore");
        //     // }
        //     return;
        // }
                if (collision.CompareTag("Score") || 
                    collision.CompareTag("Score_2x") || 
                    collision.CompareTag("Score_3x"))
                {
                    // 🔥 Get MULTIPLIER
                    int multiplier = collision.CompareTag("Score_2x") ? 2 : 
                                    collision.CompareTag("Score_3x") ? 3 : 1;

                    // 🔥 Apply MULTIPLIER (calls UpdateScore() X times)
                    for (int i = 0; i < multiplier; i++)
                    {
                        _gm.UpdateScore();  // Each call: score++ AND individualPoints++
                    }
                    
                    // Visuals
                    Destroy(Instantiate(_scoreParticlePrefab, transform.position, Quaternion.identity), 1f);
                    SoundManager.Instance?.PlaySound(_pointClip);
                    collision.gameObject.GetComponent<Score>().ScoreAdded();
                    
                    Debug.Log($"🎯 x{multiplier} HIT! Individual Points: {_gm.GetIndividualPoints()}");
                    return;
                }


    }
    

    

    [SerializeField] private float _startRadius;
    [SerializeField] private float _moveTime;

    [SerializeField] private List<float> _rotateRadius;
    private float currentRadius;

    private int level;


    private IEnumerator ChangeRadius()
    {
        canClick = false;
        
        float moveStartRadius = _rotateRadius[level];
        float moveEndRadius = _rotateRadius[(level + 1) % _rotateRadius.Count];
        float moveOffset = moveEndRadius - moveStartRadius;
        float speed = 1 / _moveTime;
        float timeElasped = 0f;
        while(timeElasped < 1f)
        {
            // timeElasped += speed * Time.fixedDeltaTime;
            timeElasped += speed * Time.deltaTime;
            currentRadius = moveStartRadius + timeElasped * moveOffset;
            // yield return new WaitForFixedUpdate();
            yield return null;
        }

        canClick = true;
        level = (level + 1) % _rotateRadius.Count;
        currentRadius = _rotateRadius[level];
    }
}