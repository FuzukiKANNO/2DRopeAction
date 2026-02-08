using UnityEngine;

public class Switch : MonoBehaviour
{
    public RopeSpawner ropeSpawner;
    private bool activated = false;

    public AudioClip switchSE;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) return;

        if (collision.CompareTag("Player"))
        {
            activated = true;
            ropeSpawner.Spawn();

            spriteRenderer.color = Color.gray;

            Vector3 scale = transform.localScale;
            scale.y = 0.2f;
            transform.localScale = scale;

            Vector3 pos = transform.position;
            pos.y = 0.1f;
            transform.position = pos;

            if (switchSE != null)
            {
                AudioSource.PlayClipAtPoint(
                    switchSE,
                    transform.position,
                    1f
                );
            }
        }
    }
}
