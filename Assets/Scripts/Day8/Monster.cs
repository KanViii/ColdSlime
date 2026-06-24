using UnityEngine;
using UnityEngine.AI;
using System;

namespace Day8
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Monster : MonoBehaviour
    {
        private NavMeshAgent agent;
        private Animator animator;
        private MonsterStats currentStats;
        
        public float currentHealth;
        private float lastAttackTime;

        [Header("Audio")]
        [SerializeField] private AudioClip spawnSound;
        [SerializeField] private AudioClip attackSound;

        // Reference to the pool to release itself when dead
        public Action<Monster> OnDeath;
        public static event Action<Monster> OnAnyMonsterDied;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
        }

        public void Initialize(MonsterStats stats)
        {
            currentStats = stats;
            currentHealth = stats.maxHealth;
            
            agent.enabled = true;
            agent.speed = stats.moveSpeed;
            agent.stoppingDistance = stats.attackRange;

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = true;
            
            // Reset state
            lastAttackTime = 0f;
            gameObject.SetActive(true);

            if (animator != null)
            {
                animator.SetFloat("currentHealth", currentHealth);
                animator.ResetTrigger("isGetHit");
            }

            // Play Spawn Sound
            if (spawnSound != null)
            {
                AudioSource.PlayClipAtPoint(spawnSound, transform.position);
            }
        }

        public void CustomUpdate()
        {
            if (Player.Instance == null || currentHealth <= 0) return;

            float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);

            // Debug.Log($"[NAVMESH] monster {transform.position}");
            // Debug.Log($"[NAVEMESH] player {Player.Instance.transform.position}");

            // Chase Player
            if (distanceToPlayer > currentStats.attackRange)
            {
                var navmeshHit = new NavMeshHit();
                NavMesh.SamplePosition(Player.Instance.transform.position, out navmeshHit, 5f, NavMesh.AllAreas); 
                agent.SetDestination(navmeshHit.position);
            }
            else
            {
                // Attack Player
                agent.ResetPath();
                
                if (Time.time >= lastAttackTime + currentStats.attackCooldown)
                {
                    Attack();
                }
            }

            // Update Custom Animator Parameters
            if (animator != null)
            {
                animator.SetBool("isNearPlayer", distanceToPlayer <= currentStats.attackRange);
                animator.SetFloat("currentHealth", currentHealth);
                animator.SetFloat("playerCurrentHealth", Player.Instance.CurrentHealth); // assuming CurrentHealth is a property
            }
        }

        private void Attack()
        {
            lastAttackTime = Time.time;
        
            if (attackSound != null)
            {
                AudioSource.PlayClipAtPoint(attackSound, transform.position);
            }

            Player.Instance.TakeDamage(currentStats.attackDamage);
        }

        public void TakeDamage(float amount)
        {
            currentHealth -= amount;
            Debug.Log($"[Monster] Monster is attacked, HP remain is: {Mathf.Max(0, currentHealth)}");
            
            if (animator != null) 
            {
                animator.SetFloat("currentHealth", currentHealth);
                animator.SetTrigger("isGetHit"); 
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (agent.isActiveAndEnabled) agent.ResetPath();
            agent.enabled = false; // Stop moving
            
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false; // Stop physics/hit detection
            
            if (animator != null) 
            {
                animator.SetFloat("currentHealth", 0f);
            }

            OnAnyMonsterDied?.Invoke(this); // Notify manager to increase score
            
            StartCoroutine(ReturnToPoolAfterDelay(0.2f)); // Wait 2 seconds for Die animation
        }

        private System.Collections.IEnumerator ReturnToPoolAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            OnDeath?.Invoke(this); // Now actually return to pool
        }
    }
}
