using UnityEngine;

public class Projectile : MonoBehaviour
{

    private TowerData _data;
    private Vector3 _shootDirection;
    private float _projectileDuration;

    void Update()
    {
        if (_projectileDuration <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            _projectileDuration -= Time.deltaTime;
            transform.position += new Vector3();
        }
    }

    public void Shoot(TowerData data, Vector3 shootDirection)
    {
        _data = data;
        _shootDirection = shootDirection;
        _projectileDuration = _data.projectileDuration;
    }
}

