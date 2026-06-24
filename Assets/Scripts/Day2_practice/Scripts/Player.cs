using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System;

public class Player : UnitBase
{

    // public GameObject attackPrefab;
    public static event Action OnPlayerDied;
    public static event Action<float, float> OnPlayerHealthChanged; // Sự kiện cập nhật máu (current, max)

    public AudioClip audioDie;
    public AudioClip audioWin;
    public AudioClip audioAttack;
    public GameObject hitPrefab; 
    public GameObject bubblePrefab;

    public static Player Instance { get; private set; }



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
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

        if (InputManager.Instance != null)
        {
            moveX = InputManager.Instance.MoveInput.x;
            moveZ = InputManager.Instance.MoveInput.y;
        }
        else if (Keyboard.current != null)
        {
            // Fallback if InputManager is not in scene
            // Z axis - up/down
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveZ += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveZ -= 1f;
            
            // X axis - left/right
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX -= 1f;
        }
        
        Vector3 movement = new Vector3(moveX, 0, moveZ).normalized;

        // Chuyển hướng di chuyển từ Local sang World Space
        Vector3 worldDisplacement = transform.TransformDirection(movement) * myStats.moveSpeed * Time.deltaTime;

        // Dùng Rigidbody để di chuyển nếu có, giúp mượt mà và tự trượt trên vách núi
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.MovePosition(transform.position + worldDisplacement);
        }
        else
        {
            // Nếu không có Rigidbody thì dùng NavMesh Raycast để chặn không cho đi ra ngoài NavMesh
            Vector3 targetPosition = transform.position + worldDisplacement;
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.Raycast(transform.position, targetPosition, out hit, UnityEngine.AI.NavMesh.AllAreas))
            {
                // Bị chặn bởi mép NavMesh
                transform.position = hit.position;
            }
            else
            {
                // Đi bình thường
                transform.position = targetPosition;
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;

        OnPlayerHealthChanged?.Invoke(currentHealth, myStats.maxHealth);

        Debug.Log($"{myStats.unitName} is attacked! Remaining HP: {Mathf.Max(0,currentHealth)}");

        
        if (hitPrefab != null)
        {
            GameObject hitEffect = Instantiate(hitPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
            Destroy(hitEffect, 1f); 
        }

        if (currentHealth <= 0) Die();
    }

    void HandleAttack()
    {
        bool attack = false;
        if (InputManager.Instance != null) attack = InputManager.Instance.AttackTriggered;
        else if (Mouse.current != null) attack = Mouse.current.leftButton.wasPressedThisFrame;

        if (attack)
        {
            if (audioAttack != null)
            {
                AudioSource.PlayClipAtPoint(audioAttack, transform.position);
            }
            Debug.Log("Player shoots bubble!");
            ShootBubble();
        }
    }

    void ShootBubble()
    {
        if (bubblePrefab != null)
        {
            // Spawn bubble ở vị trí ngang hông của Player (cao lên 0.5f để ngang tầm bọn slime)
            Vector3 spawnPos = transform.position + transform.forward * 0.5f + Vector3.up * 0.1f;
            GameObject bubble = Instantiate(bubblePrefab, spawnPos, transform.rotation);
            
            // Lấy script PlayerBubble (nếu prefab chưa gắn thì tự động AddComponent)
            PlayerBubble bubbleScript = bubble.GetComponent<PlayerBubble>();
            if (bubbleScript == null) 
            {
                bubbleScript = bubble.AddComponent<PlayerBubble>();
            }
            
            // Truyền thông số từ Player sang viên đạn
            bubbleScript.damage = myStats.attackDamage;
            bubbleScript.maxDistance = attackRange;
        }
    }

    protected override void Die()
    {
        if (audioDie != null)
        {
            AudioSource.PlayClipAtPoint(audioDie, transform.position);
        }
        
        Debug.Log("--- GAME OVER! LOSER ^^ ---");

        OnPlayerDied?.Invoke();

        // Stop the player from moving/acting
        this.enabled = false;
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        StartCoroutine(LoadMainSceneAfterDelay(2f));
    }

    private System.Collections.IEnumerator LoadMainSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Main");
    }

    public void PlayWinSound()
    {
        if (audioWin != null)
        {
            AudioSource.PlayClipAtPoint(audioWin, transform.position);
            Debug.Log("Player Wins Level!");
        }
    }
}