using UnityEngine;
using System;
using System.Collections;

public class Enemy : UnitBase
{
    // Đây là "tiếng chuông" báo hiệu khi quái chết
    public static event Action<Enemy> OnAnyEnemyDied;

    public float attackRange = 1f; 
    private bool isKnockedBack = false;

    private void OnEnable()
    {
        isKnockedBack = false;
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
            return; // Đứng yên nếu cách xa hơn 10 đơn vị
        }

        // Debug.Log("Enemy see Player");
        Vector3 direction = (Player.Instance.transform.position - transform.position).normalized;
        direction.y = 0;

        transform.Translate(-direction * myStats.moveSpeed * Time.deltaTime, Space.World);

        if (direction != Vector3.zero)
        {
            transform.forward = -direction;
        }

        // HandleAttack();
    }

    void HandleAttack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        
        foreach (Collider hit in hitColliders)
        {
            Player player = hit.GetComponent<Player>();
            
            if (player != null) 
            {
                Debug.Log("Enemy Attack!");
                player.TakeDamage(myStats.attackDamage);
            }
        }
    
    }

    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;

        Debug.Log($"{myStats.unitName} is attacked! Remaining HP: {Mathf.Max(0,currentHealth)}");

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
        float jumpHeight = 1.5f;
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
        // Invoke WaveManager: "Tôi chết rồi!"
        OnAnyEnemyDied?.Invoke(this);
        gameObject.SetActive(false);
    }

}