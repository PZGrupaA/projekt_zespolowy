using UnityEngine;

namespace Zaliczenie
{
    public class Player : MonoBehaviour
    {
        public const float Speed = 300f;
        public const float JumpVelocity = 400f;
        const float Gravity = 980f;

        public Stats stats = new Stats { maxHealth = 1000, defence = 10, attack = 3 };
        public Rigidbody2D Body { get; private set; }
        public float Horizontal { get; private set; }
        public bool JumpPressed { get; private set; }
        public int Facing { get; private set; } = 1;
        public bool IsGrounded { get; private set; }

        Health health;
        SpriteRenderer sprite;
        StateMachine movement;
        Vector3 spawnPoint;
        bool jumpRequested;
        bool canShoot = true;
        float shootTimer;

        void Awake()
        {
            tag = "Player";
            int playerLayer = LayerMask.NameToLayer("Player");
            if (playerLayer >= 0)
                gameObject.layer = playerLayer;

            sprite = gameObject.AddComponent<SpriteRenderer>();
            sprite.sprite = SpriteBank.LoadSingle("Character");
            sprite.sortingOrder = 2;

            BoxCollider2D box = gameObject.AddComponent<BoxCollider2D>();
            box.size = new Vector2(19f, 25f);

            Body = gameObject.AddComponent<Rigidbody2D>();
            Body.gravityScale = 0f;
            Body.freezeRotation = true;
            Body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            Body.interpolation = RigidbodyInterpolation2D.Interpolate;

            health = gameObject.AddComponent<Health>();
            HealthBar bar = new GameObject("HP").AddComponent<HealthBar>();
            bar.transform.SetParent(transform, false);
            bar.transform.localPosition = new Vector3(0f, 20f, 0f);
            health.Init(stats.maxHealth);
            bar.Bind(health);
            health.Died += OnDied;

            GameObject weapon = new GameObject("fireball");
            weapon.transform.SetParent(transform, false);
            weapon.AddComponent<FireballVisual>();

            movement = new StateMachine();
            movement.Register("idle_state", new IdleState(), this);
            movement.Register("walk_state", new WalkState(), this);
            movement.Register("jump_state", new JumpState(), this);
            movement.Change("idle_state");
        }

        void Start()
        {
            spawnPoint = transform.position;
        }

        void Update()
        {
            Horizontal = 0f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                Horizontal -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                Horizontal += 1f;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                jumpRequested = true;

            if (!canShoot)
            {
                shootTimer -= Time.deltaTime;
                if (shootTimer <= 0f)
                    canShoot = true;
            }

            if (Input.GetKeyDown(KeyCode.E) && canShoot)
                Shoot();

            if (transform.position.y < -400f && health != null && health.Current > 0)
                health.DamageTaken(health.Current);
        }

        void FixedUpdate()
        {
            JumpPressed = jumpRequested;
            IsGrounded = CheckGround();
            movement.PhysicsUpdate(Time.fixedDeltaTime);

            Vector2 velocity = Body.velocity;
            if (!IsGrounded)
                velocity.y -= Gravity * Time.fixedDeltaTime;
            else if (velocity.y < 0f)
                velocity.y = 0f;
            Body.velocity = velocity;

            jumpRequested = false;
            JumpPressed = false;
        }

        public void ConsumeJump()
        {
            jumpRequested = false;
            JumpPressed = false;
        }

        public void SetHorizontalSpeed(float speedX)
        {
            Vector2 velocity = Body.velocity;
            velocity.x = speedX;
            Body.velocity = velocity;
        }

        public void SetVerticalSpeed(float speedY)
        {
            Vector2 velocity = Body.velocity;
            velocity.y = speedY;
            Body.velocity = velocity;
        }

        public void Face(float direction)
        {
            if (direction < 0f)
            {
                Facing = -1;
                sprite.flipX = true;
            }
            else if (direction > 0f)
            {
                Facing = 1;
                sprite.flipX = false;
            }
        }

        void Shoot()
        {
            GameObject bulletObject = new GameObject("fireball");
            bulletObject.transform.position = transform.position;
            Projectile bullet = bulletObject.AddComponent<Projectile>();
            bullet.direction = Facing;
            bullet.shooter = "Player";
            bullet.damage = 50 * stats.attack;
            bullet.cooldown = 1f;
            canShoot = false;
            shootTimer = bullet.cooldown;
        }

        bool CheckGround()
        {
            Vector2 origin = (Vector2)transform.position + Vector2.down * 12.5f;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 4f, MapMask());
            return hit.collider != null;
        }

        public static int MapMask()
        {
            int map = LayerMask.NameToLayer("Map");
            if (map < 0)
                return Physics2D.DefaultRaycastLayers;
            return 1 << map;
        }

        public void OnDied()
        {
            transform.position = spawnPoint;
            Body.velocity = Vector2.zero;
            health.RestoreFull();
            movement.Change("idle_state");
            CameraFollow camera = Camera.main != null ? Camera.main.GetComponent<CameraFollow>() : null;
            if (camera != null)
                camera.ResetToStart();
        }
    }
}