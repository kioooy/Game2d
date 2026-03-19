using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tower : MonoBehaviour
{
    public static event Action<Tower> OnTowerClicked;
    public static void InvokeTowerClicked(Tower t) => OnTowerClicked?.Invoke(t);


    [SerializeField] private TowerData data;
    public TowerData Data => data;

    /// <summary>World position where this tower was placed (used for swapping).</summary>
    public Vector3 PlacedPosition { get; set; }

    /// <summary>The original Platform GameObject, hidden when tower was placed.</summary>
    public GameObject OriginalPlatform { get; set; }

    // --- Upgrade system ---
    private int _upgradeLevel = 0;
    public int UpgradeLevel => _upgradeLevel;
    private const float DAMAGE_PER_LEVEL = 0.25f; // +25% per upgrade
    private const float UPGRADE_COST_MULTIPLIER = 0.5f; // upgrade costs 50% of base cost per level
    private const int MAX_UPGRADE_LEVEL = 5;

    /// <summary>Total coins invested in this tower (base cost + all upgrade costs).</summary>
    private int _totalInvested = 0;
    public void SetInitialCost(int baseCost) { _totalInvested = baseCost; }

    public float GetCurrentDamage() => data.damage * (1f + _upgradeLevel * DAMAGE_PER_LEVEL);
    public int GetSellRefund() => Mathf.RoundToInt(_totalInvested * 0.5f);
    public int GetUpgradeCost() => Mathf.RoundToInt(data.cost * UPGRADE_COST_MULTIPLIER * (_upgradeLevel + 1));
    public bool CanUpgrade() => _upgradeLevel < MAX_UPGRADE_LEVEL;

    public void Upgrade()
    {
        if (!CanUpgrade()) return;
        int cost = GetUpgradeCost();
        _totalInvested += cost;
        _upgradeLevel++;
    }

    private CircleCollider2D _circleCollider;

    private List<Enemy> _enemiesInRange;
    private ObjectPooler _projectilePool;

    private float _shootTimer;




    private void OnEnable()
    {
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
    }
    private void OnDisable()
    {
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
    }


    private LineRenderer _rangeRenderer;

    private void SetupRangeIndicator()
    {
        _rangeRenderer = gameObject.AddComponent<LineRenderer>();
        _rangeRenderer.startWidth = 0.05f;
        _rangeRenderer.endWidth = 0.05f;
        _rangeRenderer.positionCount = 51; // 50 points + 1 to close loop
        _rangeRenderer.useWorldSpace = false;
        _rangeRenderer.loop = true;
        
        // Semi-transparent white
        _rangeRenderer.startColor = new Color(1, 1, 1, 0.4f);
        _rangeRenderer.endColor = new Color(1, 1, 1, 0.4f);

        // Simple line material from Unity
        _rangeRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _rangeRenderer.sortingOrder = 10; // Over most sprites

        float radius = data.range;
        float angle = 0f;
        for (int i = 0; i <= 50; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            _rangeRenderer.SetPosition(i, new Vector3(x, y, 0));
            angle += 360f / 50;
        }

        _rangeRenderer.enabled = false;
    }

    public void ShowRange(bool show)
    {
        if (_rangeRenderer != null) _rangeRenderer.enabled = show;
    }


    private void Start()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        _circleCollider.radius = data.range;
        _enemiesInRange = new List<Enemy>();
        _projectilePool = GetComponent<ObjectPooler>();
        _shootTimer = data.shootinterval;

        SetupRangeIndicator();

        // Tower Visual Fix: Increase sorting order of all parts to ensure base is visible over Ground
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sr in renderers)
        {
            sr.sortingOrder += 1;
        }
    }

    private void Update()
    {
        _shootTimer -= Time.deltaTime;
        if (_shootTimer <= 0)
        {
            _shootTimer = data.shootinterval;
            Shoot();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, data.range);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            //if (enemy != null && !_enemiesInRange.Contains(enemy))
            //{
            _enemiesInRange.Add(enemy);
            //}
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (_enemiesInRange.Contains(enemy))
            {
                _enemiesInRange.Remove(enemy);
            }
        }
    }

    private void Shoot()
    {
        if (_enemiesInRange.Count > 0)
        {
            GameObject projectile = _projectilePool.GetPooledObject();
            projectile.transform.position = transform.position;
            projectile.SetActive(true);
            Vector2 _shootDirection = (_enemiesInRange[0].transform.position - transform.position).normalized;
            projectile.GetComponent<Projectile>().Shoot(data, _shootDirection, GetCurrentDamage());

            // Play shoot sound
            if (AudioManager.Instance != null) AudioManager.Instance.PlayTowerShoot();
        }
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        _enemiesInRange.Remove(enemy);
    }
}
