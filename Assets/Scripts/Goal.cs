using UnityEngine;

public class Goal : MonoBehaviour
{
    public AudioClip goalSE;

    private SpriteRenderer spriteRenderer;
    public Animator animator;
    private bool isGoal = false;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit: " + collision.name);
        if (isGoal) return;

        if (collision.CompareTag("Player"))
        {
            if (goalSE != null)
            {
                AudioSource.PlayClipAtPoint(
                    goalSE,
                    transform.position,
                    1f
                );
            }
            isGoal = true;
            animator.SetTrigger("Goal");
        }
    }
}
