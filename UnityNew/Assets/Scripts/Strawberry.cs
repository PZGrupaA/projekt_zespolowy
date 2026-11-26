using UnityEngine;

namespace Zaliczenie
{
    public class Strawberry : MonoBehaviour
    {
        public int heal = 1000;

        Sprite[] frames;
        SpriteRenderer sprite;
        float time;

        void Awake()
        {
            sprite = gameObject.AddComponent<SpriteRenderer>();
            sprite.sortingOrder = 1;
            frames = SpriteBank.SliceHorizontal("Strawberry", 32, 32);
            if (frames.Length > 0)
                sprite.sprite = frames[0];

            CapsuleCollider2D capsule = gameObject.AddComponent<CapsuleCollider2D>();
            capsule.direction = CapsuleDirection2D.Vertical;
            capsule.size = new Vector2(12f, 16f);
            capsule.isTrigger = true;
        }

        void Update()
        {
            if (frames.Length == 0)
                return;

            time += Time.deltaTime;
            sprite.sprite = frames[(int)(time * 13f) % frames.Length];
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
                health.Heal(heal);
        }
    }
}
