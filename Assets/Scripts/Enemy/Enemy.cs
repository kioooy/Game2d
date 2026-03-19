using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;
<<<<<<< Updated upstream
=======
    public EnemyData Data => data;
    [SerializeField] public bool defaultFacesLeft; // Toggle this in Inspector if moonwalking
>>>>>>> Stashed changes
    public static event Action<EnemyData> OnEnemyReachedEnd;

    private Path _currentPath;

    private Vector3 _targetPosition;
    private int _currentWaypoint;

    //code de tham chieu den Path trong scene
    private void Awake()
    {
        _currentPath = GameObject.Find("Path").GetComponent<Path>();
<<<<<<< Updated upstream
=======
        _healthBarOriginalScale = healthBar.localScale;

        // Ensure health bar is visible over towers (sorting order 5)
        SpriteRenderer sr = healthBar.GetComponent<SpriteRenderer>();
        if (sr != null) sr.sortingOrder = 5;
>>>>>>> Stashed changes
    }

    // Reset bien ve vi tri dau tien tren path moi khi ke dich dc kich hoat
    private void OnEnable()
    {
        _currentWaypoint = 0;
        _targetPosition = _currentPath.GetPosition(_currentWaypoint);
    }


    private float _distanceMoved = 0f;
    private Vector3 _lastPosition;

    // khi di chuyen den target position, tiep tuc di chuyen den waypoint tiep theo
    void Update()
    {
<<<<<<< Updated upstream
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition,data.speed * Time.deltaTime);
    
=======
        _lastPosition = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, data.speed * Time.deltaTime);

        // --- Move Sound Logic ---
        _distanceMoved += Vector3.Distance(transform.position, _lastPosition);
        if (_distanceMoved >= 2.0f) // Play move sound every 2 units
        {
            _distanceMoved = 0f;
            if (AudioManager.Instance != null) AudioManager.Instance.PlayEnemyMove();
        }

        // --- Sprite Flipping Logic ---
        Vector2 direction = (_targetPosition - transform.position).normalized;
        if (Mathf.Abs(direction.x) > 0.1f)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                // If default faces right: flipX is true when moving left (x < 0)
                // If default faces left: flipX is true when moving right (x > 0)
                sr.flipX = defaultFacesLeft ? direction.x > 0 : direction.x < 0;
            }
        }

>>>>>>> Stashed changes
        float relativeDistance = (transform.position - _targetPosition).magnitude;
        if (relativeDistance < 0.1f)
        {
            if (_currentWaypoint < _currentPath.Waypoints.Length - 1)
            {
                _currentWaypoint++;
                _targetPosition = _currentPath.GetPosition(_currentWaypoint);
            }
            else // neu di chuyen den duoc waypoint cuoi cung
<<<<<<< Updated upstream
        
=======
>>>>>>> Stashed changes
            {
                OnEnemyReachedEnd?.Invoke(data);
                gameObject.SetActive(false);
            }
        }
    }
<<<<<<< Updated upstream
=======

    public void TakeDamage(float damage)
    {
        _lives -= damage;
        _lives = Math.Max(_lives, 0);
        UpdateHealthBar();

        if (_lives <= 0)
        {
            // Play death sound
            if (AudioManager.Instance != null) AudioManager.Instance.PlayEnemyDeath();

            OnEnemyDestroyed?.Invoke(this);
            gameObject.SetActive(false);

        }
    }


    private void UpdateHealthBar()
    {
        float healthPercent = _lives / _maxLives;
        Vector3 scale = _healthBarOriginalScale;
        scale.x = _healthBarOriginalScale.x * healthPercent;
        healthBar.localScale = scale;
    }

    public void Initialize(float healthMultiplication)
    {
        _maxLives = data.lives * healthMultiplication;
        _lives = _maxLives;
        UpdateHealthBar();
    }
>>>>>>> Stashed changes
}
