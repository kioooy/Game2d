using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    public static event Action<EnemyData> OnEnemyReachedEnd;

    private Path _currentPath;

    private Vector3 _targetPosition;
    private int _currentWaypoint;

    //code de tham chieu den Path trong scene
    private void Awake()
    {
        _currentPath = GameObject.Find("Path").GetComponent<Path>();
    }

    // Reset bien ve vi tri dau tien tren path moi khi ke dich dc kich hoat
    private void OnEnable()
    {
        _currentWaypoint = 0;
        _targetPosition = _currentPath.GetPosition(_currentWaypoint);
    }


    // khi di chuyen den target position, tiep tuc di chuyen den waypoint tiep theo
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition,data.speed * Time.deltaTime);
    
        float relativeDistance = (transform.position - _targetPosition).magnitude;
        if (relativeDistance < 0.1f)
        {
            if (_currentWaypoint < _currentPath.Waypoints.Length - 1)
            {
                _currentWaypoint++;
                _targetPosition = _currentPath.GetPosition(_currentWaypoint);
            }
            else // neu di chuyen den duoc waypoint cuoi cung
        
            {
                OnEnemyReachedEnd?.Invoke(data);
                gameObject.SetActive(false);
            }
        }
    }
}
