using UnityEngine;

namespace Zaliczenie
{
    public class Coin : MonoBehaviour
    {
        public bool tossOnSpawn = true;

        Sprite[] frames;
        SpriteRenderer sprite;
        float time;

        void Awake()
        {
            sprite = gameObject.AddComponent<SpriteRenderer>();
            sprite.sortingOrder = 2;
            frames = SpriteBank.SliceHorizontal("Monetka_002", 32, 32);
            if (frames.Length > 0)
                sprite.sprite = frames[0];

            CircleCollider2D circle = gameObject.AddComponent<CircleCollider2D>();
            circle.radius = 8f;
            circle.isTrigger = !tossOnSpawn;

            Rigidbody2D body = gameObject.AddComponent<Rigidbody2D>();
            body.freezeRotation = true;
            if (tossOnSpawn)
            {
                body.gravityScale = 1f;
                body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                float throwX = Random.Range(-100f, 100f);
                float throwY = Random.Range(120f, 160f);
                body.AddForce(new Vector2(throwX, throwY), ForceMode2D.Impulse);
            }
            else
            {
                body.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        void Update()
        {
            if (frames.Length == 0)
                return;

            time += Time.deltaTime;
            sprite.sprite = frames[(int)(time * 10f) % frames.Length];
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            TryCollect(collision.collider);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            TryCollect(other);
        }

        void TryCollect(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (Money.Instance != null)
                Money.Instance.AddCoin();
            Destroy(gameObject);
        }
    }
}
