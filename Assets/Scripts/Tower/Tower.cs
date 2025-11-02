using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private TowerData data;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, data.range);
    }

}
