using UnityEngine;

public class Projectile : MonoBehaviour
{

    private TowerData _data;
    private Vector3 _shootDirection;
    private float _damage;
    private float _projectileDuration;
    private float _actualDamage;

    void Update()
    {
        if (_projectileDuration <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            _projectileDuration -= Time.deltaTime;
            transform.position += new Vector3(_shootDirection.x, _shootDirection.y) *
            _data.projectileSpeed * Time.deltaTime;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
<<<<<<< Updated upstream
            enemy.TakeDamage(_actualDamage);
=======
            if (enemy != null)
            {
                enemy.TakeDamage(_damage);
            }
>>>>>>> Stashed changes
            gameObject.SetActive(false);
        }
    }

<<<<<<< Updated upstream
    public void Shoot(TowerData data, Vector3 shootDirection, float actualDamage = -1f)
=======
    public void Shoot(TowerData data, Vector3 shootDirection, float damage = -1f)
>>>>>>> Stashed changes
    {
        _data = data;
        _shootDirection = shootDirection;
        _projectileDuration = _data.projectileDuration;
<<<<<<< Updated upstream
        _actualDamage = actualDamage > 0 ? actualDamage : _data.damage;
=======
        _damage = (damage < 0) ? data.damage : damage;
>>>>>>> Stashed changes
    }
}
