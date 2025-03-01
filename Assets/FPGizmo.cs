using UnityEngine;

public class FirePointGizmo : MonoBehaviour
{
    [SerializeField] private Color gizmoColor = Color.red;
    [SerializeField] private float gizmoSize = 0.1f;

    private void OnDrawGizmos()
    {
        // Set the Gizmo color
        Gizmos.color = gizmoColor;

        // Draw a sphere at the firePoint's position
        Gizmos.DrawSphere(transform.position, gizmoSize);

        // Draw a line to represent the firePoint's forward direction
        Gizmos.DrawRay(transform.position, transform.forward * 0.5f);
    }
}
