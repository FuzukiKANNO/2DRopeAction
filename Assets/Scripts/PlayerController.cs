using UnityEngine;
using UnityEngine.Audio;
using System.Collections;


public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float regrabCooldown = 0.2f;
    private float regrabTimer = 0f;


    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isHanging;
    public float swimSpeed = 3f;
    public float swimUpForce = 12f;
    public float maxSwimUpSpeed = 4f;

    private bool isInWater = false;
    private float defaultGravity;
    private float defaultDrag;

    private HingeJoint2D ropeJoint;

    //Swing
    public float swingForce = 1f;
    public float maxSwingSpeed = 6f;

    //Death
    public GameObject deathEffectPrefab;
    public float slowTimeScale = 0.2f;
    public float slowDuration = 0.3f;
    private bool isDead = false;

    
    public GameObject ropeGrabEffectPrefab;
    public AudioClip ropeGrabSE;

    private TrailRenderer trail;

    private Rigidbody2D currentPlatform;

    private SpriteRenderer spriteRenderer;

    public AudioMixer audioMixer;

    private Animator animator;
    float moveInput;
    [SerializeField] private Transform visual;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
        defaultDrag = rb.linearDamping;
        trail = GetComponent<TrailRenderer>();
        trail.emitting = false;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }


    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        if (transform.position.y <= -10)
        {
            Die();
        }
        if (regrabTimer > 0)
        {
            regrabTimer -= Time.deltaTime;
        }

        if(isHanging)
        {
            Swing();
            JumpFromRope();
        }
    else if (isInWater)
        {
            Swim();
        }
        else
        {
            Move();
            Jump();
        }
        UpdateAnimation();
        HandleFlip();
    }

    void UpdateAnimation()
    {
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsHanging", isHanging);
        animator.SetBool("IsInWater", isInWater);
    }

    void HandleFlip()
    {
        if (isHanging) return;

        if (moveInput > 0.1f)
        {
            visual.localScale = new Vector3(3f, 3f, 1f);
        }
        else if (moveInput < -0.1f)
        {
            visual.localScale = new Vector3(-3f, 3f, 1f);
        }
    }

    void Move()
    {
        float platformX = currentPlatform ? currentPlatform.linearVelocity.x : 0f;

        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed + platformX,
            rb.linearVelocity.y
        );
    }


    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void Swim()
    {
        float x = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(
            x * swimSpeed,
            rb.linearVelocity.y
        );

        if (Input.GetButton("Jump"))
        {
            if (rb.linearVelocity.y < maxSwimUpSpeed)
            {
                rb.AddForce(Vector2.up * swimUpForce);
            }
        }
    }


    void JumpFromRope()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Vector2 swingVelocity = rb.linearVelocity;

            Destroy(ropeJoint);
            isHanging = false;
            regrabTimer = regrabCooldown;

            float swingBoost = 0.8f;

            Vector2 jumpVelocity =
                Vector2.up * jumpForce +
                swingVelocity * swingBoost;

            rb.linearVelocity = jumpVelocity;

            StartCoroutine(TrailBurst());
        }
    }

    IEnumerator TrailBurst()
    {
        trail.Clear();
        trail.emitting = true;

        yield return new WaitForSeconds(0.5f);

        trail.emitting = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isHanging) return;
        if (regrabTimer > 0) return;

        if (collision.gameObject.name.Contains("Segment"))
        {
            if (ropeGrabSE != null)
            {
                AudioSource.PlayClipAtPoint(
                    ropeGrabSE,
                    transform.position,
                    1f
                );
            }

            if (ropeGrabEffectPrefab != null)
            {
                Instantiate(
                    ropeGrabEffectPrefab,
                    collision.transform.position,
                    Quaternion.identity
                );
            }

            ropeJoint = gameObject.AddComponent<HingeJoint2D>();
            ropeJoint.connectedBody = collision.attachedRigidbody;
            ropeJoint.autoConfigureConnectedAnchor = false;
            ropeJoint.anchor = Vector2.zero;
            ropeJoint.connectedAnchor = Vector2.zero;

            isHanging = true;
        }
        if (collision.CompareTag("Hazard"))
        {
            Die();
        }
        if (collision.CompareTag("Water"))
        {
            EnterWater();
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            ExitWater();
        }
    }

    void EnterWater()
    {
        isInWater = true;
        audioMixer.SetFloat("Cutoff freq", 800f);
        rb.gravityScale = defaultGravity * 0.7f;
        rb.linearDamping = 1.5f; 
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            Mathf.Min(rb.linearVelocity.y, 0f)
        );
    }

    void ExitWater()
    {
        isInWater = false;
        audioMixer.SetFloat("Cutoff freq", 22000f);
        rb.gravityScale = defaultGravity;
        rb.linearDamping = defaultDrag;
    }


    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (audioMixer != null)
        {
            audioMixer.SetFloat("Cutoff freq", 800f);
            audioMixer.SetFloat("Pitch", 0.7f);
        }
        if (deathEffectPrefab != null)
        {
            Instantiate(
                deathEffectPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        spriteRenderer.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        Time.timeScale = slowTimeScale;
        yield return new WaitForSecondsRealtime(slowDuration);
        Time.timeScale = 1f;
        yield return new WaitForSeconds(1f);
        Respawn();
    }

    void Respawn()
    {
        Time.timeScale = 1f;

        Vector3 respawnPos = GameManager.Instance
       .GetRespawnPosition(Vector3.zero);

        transform.position = respawnPos;
        rb.simulated = true;
        spriteRenderer.enabled = true;
        if (audioMixer != null)
        {
            audioMixer.SetFloat("Cutoff freq", 22000f);
            audioMixer.SetFloat("Pitch", 1.0f);
        }


        isDead = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            currentPlatform = collision.rigidbody;
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            currentPlatform = null;
            isGrounded = false;
        }
    }

    void Swing()
    {
        float input = Input.GetAxisRaw("Horizontal");
        if (input == 0) return;

        float speed = rb.linearVelocity.x;

        float speedRatio = Mathf.Abs(speed) / maxSwingSpeed;
        float accelMultiplier = Mathf.Clamp01(1f - speedRatio);

        bool sameDirection =
            Mathf.Sign(speed) == Mathf.Sign(input) || Mathf.Abs(speed) < 0.1f;

        float finalForce = swingForce * accelMultiplier;

        if (sameDirection)
        {
            rb.AddForce(Vector2.right * input * finalForce, ForceMode2D.Force);
        }
        else
        {
            rb.AddForce(Vector2.right * input * finalForce * 0.5f, ForceMode2D.Force);
        }

        rb.linearVelocity = new Vector2(
            Mathf.Clamp(rb.linearVelocity.x, -maxSwingSpeed, maxSwingSpeed),
            rb.linearVelocity.y
        );
    }

    public bool IsHanging()
    {
        return isHanging;
    }

}
