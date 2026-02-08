using UnityEngine;

public class PlayerGoal : MonoBehaviour
{
    private Animator animator;
    private bool isGoal = false;
    public BGMManager BGMManager;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isGoal) return;

        if (other.CompareTag("Goal"))
        {
            isGoal = true;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;

            animator.SetTrigger("Goal");
        }
    }

    public void OnGoalAnimationEnd()
    {
        Debug.Log("GOAL!");
        BGMManager.Instance.FadeOutBGM(1.5f);
        gameObject.SetActive(false);
    }
}
