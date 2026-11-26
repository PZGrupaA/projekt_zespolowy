using UnityEngine;

namespace Zaliczenie
{
    public class Projectile : MonoBehaviour
    {
        public float speed = 200f;
        public int damage = 50;
        public float cooldown = 1f;
        public string shooter = "";
        public int direction = 1;

        SpriteRenderer sprite;
        CircleCollider2D hitbox;

        void Awake()
        {
            sprite = gameObject.AddComponent<SpriteRenderer>();
            sprite.sprite = SpriteBank.LoadSingle("fireball2");
            sprite.sortingOrder = 3;

            hitbox = gameObject.AddComponent<CircleCollider2D>();
            hitbox.isTrigger = true;
            hitbox.radius = 11f;

            Rigidbody2D body = gameObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.useFullKinematicContacts = true;
        }

        void Start()
        {
            Destroy(gameObject, 5f);
            if (direction < 0 && sprite != null)
                sprite.flipX = true;
        }

        void Update()
        {
            transform.position += new Vector3(direction * speed * Time.deltaTime, 0f, 0f);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!string.IsNullOrEmpty(shooter) && other.CompareTag(shooter))
                return;

            Health health = other.GetComponent<Health>();
            if (health != null)
                health.DamageTaken(damage);
        }
    }
}
