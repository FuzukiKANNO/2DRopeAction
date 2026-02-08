using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private bool activated = false;

    public GameObject saveEffectPrefab;
    public AudioClip saveSE;
    public Material material;

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
            GameManager.Instance.SavePosition(transform.position);
            activated = true;

            Debug.Log("Saved");
            spriteRenderer.material = material; 
            
            if (saveSE != null)
            {
                AudioSource.PlayClipAtPoint(
                    saveSE,
                    transform.position,
                    1f
                );
            }

            if (saveEffectPrefab != null)
            {
                Instantiate(
                    saveEffectPrefab,
                    collision.transform.position,
                    Quaternion.identity
                );
            }
        }
    }
}
