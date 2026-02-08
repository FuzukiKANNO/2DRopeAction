using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Follow Speed")]
    public float normalSmooth = 6f;
    public float ropeSmooth = 3f;

    public Vector3 offset;

    public PlayerController player;

    void LateUpdate()
    {
        if (target == null) return;

        float currentSmooth =
            (player != null && player.IsHanging())
            ? ropeSmooth
            : normalSmooth;

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = -10f;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            currentSmooth * Time.deltaTime
        );
    }
}