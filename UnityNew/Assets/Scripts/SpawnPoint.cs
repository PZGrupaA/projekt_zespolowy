using UnityEngine;

namespace Zaliczenie
{
    public class SpawnPoint : MonoBehaviour
    {
        public int currentEnemies;
        float wait = 1f;

        void Update()
        {
            wait -= Time.deltaTime;
            if (wait > 0f)
                return;

            Spawn();
            wait = 5f;
        }

        void Spawn()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return;

            float distance = player.transform.position.x - transform.position.x;
            if (Mathf.Abs(distance) > 50f && currentEnemies <= 2)
            {
                GameObject enemy = new GameObject("small_skeleton");
                enemy.transform.SetParent(transform, false);
                enemy.transform.position = transform.position;
                SkeletonEnemy skeleton = enemy.AddComponent<SkeletonEnemy>();
                skeleton.Home = this;
                currentEnemies += 1;
            }
        }
    }
}
