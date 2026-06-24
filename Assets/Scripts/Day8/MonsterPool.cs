using UnityEngine;
using UnityEngine.Pool;

namespace Day8
{
    public class MonsterPool : MonoBehaviour
    {
        [SerializeField] private Monster prefab;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxSize = 50;

        private ObjectPool<Monster> pool;

        public void Initialize(Monster monsterPrefab)
        {
            this.prefab = monsterPrefab;
            
            pool = new ObjectPool<Monster>(
                createFunc: CreateMonster,
                actionOnGet: OnTakeFromPool,
                actionOnRelease: OnReturnedToPool,
                actionOnDestroy: OnDestroyPoolObject,
                collectionCheck: true,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }

        private Monster CreateMonster()
        {
            Monster instance = Instantiate(prefab);
            instance.OnDeath = ReturnToPool;
            return instance;
        }

        private void OnTakeFromPool(Monster monster)
        {
            monster.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(Monster monster)
        {
            monster.gameObject.SetActive(false);
        }

        private void OnDestroyPoolObject(Monster monster)
        {
            Destroy(monster.gameObject);
        }

        public Monster Get()
        {
            return pool.Get();
        }

        public void ReturnToPool(Monster monster)
        {
            pool.Release(monster);
        }
    }
}
