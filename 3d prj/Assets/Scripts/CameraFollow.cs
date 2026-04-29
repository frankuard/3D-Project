using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;          // drag Player Armature here

    [Header("Position")]
    public float distance  = 4f;       // how far behind
    public float height    = 2f;       // how high above player
    public float smoothing = 8f;       // follow smoothness

    void LateUpdate()
    {
        if (target == null) return;

        // Desired position: behind and above the player
        Vector3 desiredPos = target.position
                           - target.forward * distance
                           + Vector3.up * height;

        // Smoothly move camera to desired position
        transform.position = Vector3.Lerp(
            transform.position, desiredPos,
            smoothing * Time.deltaTime);

        // Always look at slightly above player root
        transform.LookAt(target.position + Vector3.up * 1.2f);
    }
}