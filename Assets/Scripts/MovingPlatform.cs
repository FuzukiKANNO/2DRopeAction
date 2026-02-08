using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 startPoint;
    [SerializeField] private Vector3 endPoint;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTime = 1f;

    private Rigidbody2D rb;
    private Vector3 target;
    private bool isWaiting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = endPoint;
    }

    void Update()
    {
        if (isWaiting) return;

        Vector2 dir = (target - transform.position).normalized;
        rb.linearVelocity = dir * speed;

        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            StartCoroutine(WaitAndSwitch());
        }
    }

    IEnumerator WaitAndSwitch()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        target = (target == endPoint)
            ? startPoint
            : endPoint;

        isWaiting = false;
    }
}
