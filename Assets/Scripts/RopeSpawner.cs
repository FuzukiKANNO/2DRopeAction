using System.Collections;
using UnityEngine;

public class RopeSpawner : MonoBehaviour
{
    public GameObject ropeSegmentPrefab;
    public int segmentCount = 10;
    public float segmentLength = 0.5f;
    public float spawnInterval = 0.05f;

    private Rigidbody2D previousRb;
    private bool spawned = false;

    public AudioClip ropeSE;

    public void Spawn()
    {
        if (spawned) return;

        spawned = true;
        StartCoroutine(SpawnRope());
    }

    IEnumerator SpawnRope()
    {
        previousRb = null;

        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 spawnPos =
                transform.position + Vector3.down * segmentLength * i;

            GameObject seg =
                Instantiate(ropeSegmentPrefab, spawnPos, Quaternion.identity);

            Rigidbody2D rb = seg.GetComponent<Rigidbody2D>();
            HingeJoint2D joint = seg.GetComponent<HingeJoint2D>();

            if (i == 0)
            {
                joint.connectedBody = null;
                joint.anchor = Vector2.zero;
            }
            else
            {
                joint.connectedBody = previousRb;
                joint.anchor = Vector2.zero;
                joint.connectedAnchor = Vector2.down * segmentLength;
            }

            previousRb = rb;
            if (ropeSE != null)
            {
                AudioSource.PlayClipAtPoint(
                    ropeSE,
                    transform.position,
                    1f
                );
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
