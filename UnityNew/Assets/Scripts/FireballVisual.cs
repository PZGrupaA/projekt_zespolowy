using UnityEngine;

namespace Zaliczenie
{
    public class FireballVisual : MonoBehaviour
    {
        Player player;
        SpriteRenderer sprite;

        void Awake()
        {
            player = GetComponentInParent<Player>();
            sprite = gameObject.AddComponent<SpriteRenderer>();
            sprite.sprite = SpriteBank.LoadSingle("fireball2");
            sprite.sortingOrder = 3;
            transform.localScale = new Vector3(0.3f, 0.3f, 1f);
        }

        void Update()
        {
            if (player == null)
                return;

            bool left = player.Facing < 0;
            transform.localPosition = new Vector3(left ? -15f : 15f, 0f, 0f);
            transform.localRotation = Quaternion.Euler(0f, 0f, left ? 90f : -90f);
            sprite.flipX = left;
        }
    }
}
