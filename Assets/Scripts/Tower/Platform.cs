using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Platform : MonoBehaviour
{
    public static event Action<Platform> OnPlatformClicked; // Event được gọi khi platform được click
    [SerializeField] private LayerMask platformLayerMask;
    
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Camera.main == null) return;
            
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Camera cam = Camera.main;
            Rect cameraRect = cam.pixelRect;
            
            // Kiểm tra trực tiếp bằng Rect.Contains để tránh cảnh báo
            if (!cameraRect.Contains(mouseScreenPos))
            {
                return; // Bỏ qua nếu chuột nằm ngoài camera rect
            }
            
            Vector2 worldPosition = cam.ScreenToWorldPoint(mouseScreenPos);
            RaycastHit2D raycastHit = Physics2D.Raycast(worldPosition, Vector2.zero, Mathf.Infinity, 
                platformLayerMask);

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
}
