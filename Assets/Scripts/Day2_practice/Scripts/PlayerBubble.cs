using UnityEngine;

public class PlayerBubble : MonoBehaviour
{
    public float speed = 10f;
    [HideInInspector] public float maxDistance = 5f;
    [HideInInspector] public float damage = 1f;

    private Vector3 startPosition;
    private LayerMask enemyLayer;

    void Start()
    {
        startPosition = transform.position;
        enemyLayer = LayerMask.GetMask("Enemy");
        
        if (enemyLayer == 0)
        {
            Debug.LogWarning("[PlayerBubble] Không tìm thấy layer 'Enemy'. Hãy chắc chắn bạn đã gõ đúng tên layer trong Unity (Viết hoa chữ E)!");
        }
    }

    void Update()
    {
        float stepDistance = speed * Time.deltaTime;
        float bubbleRadius = 0.7f; 

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, bubbleRadius, transform.forward, out hit, stepDistance, enemyLayer))
        {
            Debug.Log($"[PlayerBubble] Bắn trúng: {hit.collider.gameObject.name}");

            Day8.Monster monster = hit.collider.GetComponentInParent<Day8.Monster>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
                Destroy(gameObject); 
                return;
            }

            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            Destroy(gameObject);
            return;
        }

        transform.Translate(Vector3.forward * stepDistance);

        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }
}
