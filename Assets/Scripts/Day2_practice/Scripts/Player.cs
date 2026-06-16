using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Player : UnitBase
{
    public static event Action OnPlayerDied;
    public static Player Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    public float attackRange = 2f; 

    void Update()
    {
        if (transform.position.y < -2f)
        {
            Die();
            return;
        }
        
        HandleMovement();
        HandleAttack();
    }

    void HandleMovement()
    {
        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current != null)
        {
            // Z axis - up/down
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveZ += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveZ -= 1f;
            
            // X axis - left/right
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX -= 1f;
        }
        
        
        
        Vector3 movement = new Vector3(moveX, 0, moveZ).normalized;

        transform.Translate(movement * myStats.moveSpeed * Time.deltaTime, Space.World);
    }

    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;

        Debug.Log($"{myStats.unitName} is attacked! Remaining HP: {Mathf.Max(0,currentHealth)}");

        if (currentHealth <= 0) Die();
    }

    void HandleAttack()
    {

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Player Attack!");
            AttackEnemiesNearby();
        }
    }

    void AttackEnemiesNearby()
    {
        // Tạo một vòng tròn ảo quanh Player để quét xem có ai đứng gần không
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        
        foreach (Collider hit in hitColliders)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            
            if (enemy != null) 
            {
                enemy.TakeDamage(myStats.attackDamage);
            }
        }
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("--- GAME OVER! LOSER ^^ ---");
        OnPlayerDied?.Invoke();
    }
}