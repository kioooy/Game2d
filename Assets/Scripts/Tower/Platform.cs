using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Platform : MonoBehaviour
{
    public static event Action<Platform> OnPlatformClicked;
    [SerializeField] private LayerMask platformLayerMask;
    public static bool towerPanelOpen { get; set; } = false;

    private void Update()
    {
        if (towerPanelOpen || Time.timeScale == 0f)
            return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D raycastHit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, platformLayerMask);

            if (raycastHit.collider != null)
            {
                Platform platform = raycastHit.collider.GetComponent<Platform>();
                if (platform != null)
                {
                    OnPlatformClicked?.Invoke(platform);
                }
            }
        }
    }
    public void PlaceTower(TowerData data)
    {
        // Store platform position
        Vector3 platformPosition = transform.position;
        // Instantiate tower at platform position
        GameObject towerInstance = Instantiate(data.prefab, platformPosition, Quaternion.identity);

        // Find the child object named "Tower" (the tower base)
        Transform towerBase = towerInstance.transform.Find("Tower");
        if (towerBase != null)
        {
            // Calculate the offset from root to tower base
            Vector3 towerBaseOffset = towerBase.localPosition;
            // Adjust root position so that tower base aligns with platform position
            towerInstance.transform.position = platformPosition - towerBaseOffset;
        }

        // Destroy the platform after placing the tower
        Destroy(gameObject);
    }
}
