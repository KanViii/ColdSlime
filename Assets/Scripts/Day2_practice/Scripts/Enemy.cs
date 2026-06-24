using UnityEngine;
using System;
using System.Collections;

public class Enemy : UnitBase
{
    public static event Action<Enemy> OnAnyEnemyDied;

    public float attackRange = 1f; 
    private bool isKnockedBack = false;
    private Animator animator; 
    public AudioClip jumpAudio;

    public AudioClip hitAudio;

    public AudioClip dieAudio;
    public AudioSource audioSource;

    public GameObject diePrefab;
    public GameObject hitPrefab;
    public GameObject surprisePrefab;

    private float attackCooldown = 0.5f;
    private float attackTimer = 0f;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        isKnockedBack = false;
        
        if (audioSource != null && jumpAudio != null)
        {
            audioSource.PlayOneShot(jumpAudio);
        }
    }

    public void CustomUpdate()
    {
        if (transform.position.y < -2f)
        {
            Die();
            return;
        }

        if (isKnockedBack) return;

        if (Player.Instance == null) {
            // Debug.Log("player Instance null");
            return;
        }

        if (Vector3.Distance(transform.position, Player.Instance.transform.position) > 10f)
        {
            animator.SetBool("isWalking", false);
            // Debug.Log("Enemy stop walking");
            return;
        }
        else
        {
            if (!animator.GetBool("isWalking"))
            {
                if (surprisePrefab != null)
                {
                    GameObject surprise = Instantiate(surprisePrefab, transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity, transform);
                    Destroy(surprise, 0.35f);
                }
            }
            animator.SetBool("isWalking", true);
        
            // Debug.Log("Enemy see Player");
            Vector3 direction = (Player.Instance.transform.position - transform.position).normalized;
            direction.y = 0;

            transform.Translate(direction * myStats.moveSpeed * Time.deltaTime, Space.World);

            if (direction != Vector3.zero)
            {
                transform.forward = direction;
            }
        }
        HandleAttack();
    }

    void HandleAttack()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        bool didAttack = false;
        
        foreach (Collider hit in hitColliders)
        {
            Player player = hit.GetComponent<Player>();
            
            if (player != null) 
            {
                // Debug.Log("Enemy Attack!");
                player.TakeDamage(myStats.attackDamage);
                didAttack = true; 
            }
        }
    
        if (didAttack)
        {
            attackTimer = attackCooldown;
        }
    }

    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;

        Debug.Log($"{myStats.unitName} is attacked! Remaining HP: {Mathf.Max(0,currentHealth)}");

        // if (hitPrefab != null)
        // {
        //     GameObject hitEffect = Instantiate(hitPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
        //     Destroy(hitEffect, 1f);
        // }

        if (audioSource != null && hitAudio != null)
        {
            audioSource.PlayOneShot(hitAudio);
        }

        if (currentHealth <= 0) 
        {
            Die();
        }
        else if (gameObject.activeInHierarchy)
        {
            StartCoroutine(KnockbackRoutine());
        }
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnockedBack = true;

        Vector3 startPos = transform.position;
        Vector3 knockbackDirection = (transform.position - Player.Instance.transform.position).normalized;
        if (knockbackDirection == Vector3.zero) knockbackDirection = -transform.forward;
        knockbackDirection.y = 0;
        
        float knockbackDistance = 2f;
        float jumpHeight = 2.5f;
        float duration = 0.3f;
        Vector3 targetPos = startPos + knockbackDirection * knockbackDistance;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Hàm sin tạo đường vòng cung để jump
            float yOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            transform.position = Vector3.Lerp(startPos, targetPos, t) + new Vector3(0, yOffset, 0);
            
            yield return null;
        }

        transform.position = targetPos;
        isKnockedBack = false;
    }

    protected override void Die()
    {
        GameObject dieEffect = Instantiate(diePrefab, this.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        Destroy(dieEffect, 3f);

        if (dieAudio != null)
        {
            AudioSource.PlayClipAtPoint(hitAudio, transform.position);
        }

        OnAnyEnemyDied?.Invoke(this);
        gameObject.SetActive(false);
    }

}