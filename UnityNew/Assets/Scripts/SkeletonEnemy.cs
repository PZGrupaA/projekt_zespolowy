using System.Collections;
using UnityEngine;

namespace Zaliczenie
{
    public class SkeletonEnemy : MonoBehaviour
    {
        public const float Speed = 75f;
        public const float JumpVelocity = 250f;
        const float Gravity = 980f;

        public SpawnPoint Home;
        public Stats stats = new Stats { maxHealth = 150, defence = 10, attack = 10 };

        Rigidbody2D body;
        SpriteRenderer sprite;
        Sprite[] walkFrames;
        Sprite[] deadFrames;
        float frameTime;
        int direction = 1;
        float wanderTimer;
        float shootCooldown;
        bool isDead;

        void Awake()
        {
            tag = "Enemy";
            int enemyLayer = LayerMask.NameToLayer("Enemy");
            if (enemyLayer >= 0)
                gameObject.layer = enemyLayer;

            sprite = gameObject.AddComponent<SpriteRenderer>();
            sprite.sortingOrder = 2;
            Sprite[] frames = SpriteBank.SliceHorizontal("Przeciwnik_003", 16, 16);
            int split = frames.Length > 4 ? frames.Length - 4 : frames.Length;
            walkFrames = new Sprite[split];
            deadFrames = new Sprite[frames.Length - split];
            for (int i = 0; i < frames.Length; i++)
            {
                if (i < split)
                    walkFrames[i] = frames[i];
                else
                    deadFrames[i - split] = frames[i];
            }
            if (walkFrames.Length > 0)
                sprite.sprite = walkFrames[0];

            CapsuleCollider2D capsule = gameObject.AddComponent<CapsuleCollider2D>();
            capsule.direction = CapsuleDirection2D.Vertical;
            capsule.size = new Vector2(12f, 18f);

            body = gameObject.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            Health health = gameObject.AddComponent<Health>();
            HealthBar bar = new GameObject("HP").AddComponent<HealthBar>();
            bar.transform.SetParent(transform, false);
            bar.transform.localPosition = new Vector3(0f, 14f, 0f);
            health.Init(stats.maxHealth);
            bar.Bind(health);
            health.Died += OnDied;

            wanderTimer = Random.Range(3f, 8f);
        }

        void Update()
        {
            if (isDead || walkFrames.Length == 0)
                return;

            frameTime += Time.deltaTime;
            sprite.sprite = walkFrames[(int)(frameTime * 7f) % walkFrames.Length];
            sprite.flipX = direction < 0;
        }

        void FixedUpdate()
        {
            bool grounded = IsGrounded();
            Vector2 velocity = body.velocity;
            if (!grounded)
                velocity.y -= Gravity * Time.fixedDeltaTime;
            else if (velocity.y < 0f)
                velocity.y = 0f;

            if (!isDead)
            {
                Wander(grounded, ref velocity);
                TryJump(grounded, ref velocity);
            }
            else
            {
                velocity.x = Mathf.MoveTowards(velocity.x, 0f, Speed);
            }

            body.velocity = velocity;
        }

        void Wander(bool grounded, ref Vector2 velocity)
        {
            wanderTimer -= Time.fixedDeltaTime;
            if (wanderTimer <= 0f)
            {
                direction = Random.Range(0, 2) == 0 ? -1 : 1;
                wanderTimer = 3f;
            }

            velocity.x = direction * Speed;
            if (grounded && !HasFloorAhead())
            {
                direction *= -1;
                velocity.x = direction * Speed;
                wanderTimer = Random.Range(3f, 8f);
            }

            if (shootCooldown <= 0f)
            {
                shootCooldown = 3f;
                StartCoroutine(Burst());
            }
            else
            {
                shootCooldown -= Time.fixedDeltaTime;
            }
        }

        void TryJump(bool grounded, ref Vector2 velocity)
        {
            if (!grounded)
                return;

            RaycastHit2D wall = Physics2D.Raycast(transform.position, Vector2.right * direction, 25f, Player.MapMask());
            if (wall.collider != null)
                velocity.y = JumpVelocity;
        }

        bool HasFloorAhead()
        {
            Vector2 origin = (Vector2)transform.position + Vector2.right * direction * 20f;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 104f, Player.MapMask());
            return hit.collider != null;
        }

        bool IsGrounded()
        {
            Vector2 origin = (Vector2)transform.position + Vector2.down * 9f;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 6f, Player.MapMask());
            return hit.collider != null;
        }

        IEnumerator Burst()
        {
            for (int i = 0; i < 3; i++)
            {
                if (isDead)
                    yield break;

                GameObject bulletObject = new GameObject("small_fireball");
                float spread = Random.Range(2f, 5f) * (Random.Range(0, 2) == 0 ? -1f : 1f);
                bulletObject.transform.position = transform.position + new Vector3(0f, spread, 0f);
                Projectile bullet = bulletObject.AddComponent<Projectile>();
                bullet.direction = direction;
                bullet.shooter = "Enemy";
                bullet.damage = ShotDamage();
                bullet.speed = 200f;
                bullet.cooldown = 0.5f;

                if (i < 2)
                    yield return new WaitForSeconds(0.2f);
            }
        }

        int ShotDamage()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Player component = player.GetComponent<Player>();
                if (component != null && component.stats != null && component.stats.maxHealth > 0)
                    return Mathf.Max(1, component.stats.maxHealth / 5);
            }

            return 200;
        }

        void OnDied()
        {
            if (isDead)
                return;

            isDead = true;
            if (deadFrames.Length > 0)
                sprite.sprite = deadFrames[deadFrames.Length - 1];

            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
                collider.enabled = false;

            if (Home != null)
                Home.currentEnemies -= 1;

            StartCoroutine(DropCoin());
        }

        IEnumerator DropCoin()
        {
            yield return new WaitForSeconds(2f);
            GameObject coin = new GameObject("moneta");
            coin.transform.position = transform.position;
            coin.AddComponent<Coin>();
            Destroy(gameObject);
        }
    }
}
