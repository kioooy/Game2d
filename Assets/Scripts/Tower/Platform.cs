using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Platform : MonoBehaviour
{
    public static event Action<Platform> OnPlatformClicked;
    public static bool towerPanelOpen { get; set; } = false;

    public static void InvokePlatformClicked(Platform p) => OnPlatformClicked?.Invoke(p);
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

        // Store placed position and platform reference for selling
        Tower tower = towerInstance.GetComponent<Tower>();
        if (tower != null)
        {
            tower.PlacedPosition = platformPosition;
            tower.SetInitialCost(data.cost);
            tower.OriginalPlatform = gameObject;
        }

        // Hide the platform (not destroy, so we can restore it when selling)
        gameObject.SetActive(false);

        // Tower Visual Fix: Increase sorting order of all parts to ensure base is visible over Ground
        SpriteRenderer[] renderers = towerInstance.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sr in renderers)
        {
            sr.sortingOrder += 1;
        }
    }
}
