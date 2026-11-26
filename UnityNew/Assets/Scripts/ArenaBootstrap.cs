using UnityEngine;

namespace Zaliczenie
{
    public class ArenaBootstrap : MonoBehaviour
    {
        const string BackgroundSpriteName = "IMG_1062";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void StartArena()
        {
            if (Object.FindObjectOfType<ArenaBootstrap>() != null)
                return;

            GameObject root = new GameObject("Arena");
            root.AddComponent<ArenaBootstrap>();
        }

        void Awake()
        {
            Physics2D.gravity = new Vector2(0f, -980f);
            int playerLayer = LayerMask.NameToLayer("Player");
            int enemyLayer = LayerMask.NameToLayer("Enemy");
            if (playerLayer >= 0 && enemyLayer >= 0)
            {
                Physics2D.IgnoreLayerCollision(playerLayer, playerLayer, true);
                Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
                Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, true);
            }

            Camera camera = PrepareCamera();
            LevelLayout.ScreenWidth = camera.orthographicSize * 2f * camera.aspect;
            LevelLayout.Completed = false;

            Transform level = new GameObject("Level").transform;
            BuildBackground(camera, level);

            for (int i = 0; i < LevelLayout.ScreenCount; i++)
                BuildScreen(level, i);

            BuildPlayer();
            BuildGoal(level);

            if (Money.Instance == null)
                new GameObject("Money").AddComponent<Money>();
            if (LetterBag.Instance == null)
                new GameObject("LetterBag").AddComponent<LetterBag>();
            if (FindObjectOfType<Hud>() == null)
                new GameObject("HUD").AddComponent<Hud>();

            CameraFollow follow = camera.GetComponent<CameraFollow>();
            follow.levelMinX = 0f;
            follow.levelMaxX = LevelLayout.Length;
            follow.ResetToStart();
        }

        static void BuildBackground(Camera camera, Transform level)
        {
            Sprite bgSprite = SpriteBank.LoadSingle(BackgroundSpriteName);
            if (bgSprite == null)
                return;

            float worldHeight = camera != null ? camera.orthographicSize * 2f : 240f;
            float spriteHeight = bgSprite.bounds.size.y > 0f ? bgSprite.bounds.size.y : 1f;
            float spriteWidth = bgSprite.bounds.size.x > 0f ? bgSprite.bounds.size.x : 1f;

            float scaleY = (worldHeight * 1.35f) / spriteHeight;
            float scaleX = scaleY;
            float tileWidth = spriteWidth * scaleX;
            float centerY = worldHeight * 0.12f;

            float startX = -tileWidth;
            float endX = Mathf.Max(LevelLayout.Length + tileWidth, tileWidth);

            GameObject bgRoot = new GameObject("Background");
            bgRoot.transform.SetParent(level, false);

            int index = 0;
            for (float x = startX; x < endX; x += tileWidth)
            {
                GameObject tile = new GameObject("bg_tile_" + index++);
                tile.transform.SetParent(bgRoot.transform, false);
                tile.transform.position = new Vector3(x + tileWidth * 0.5f, centerY, 5f);
                tile.transform.localScale = new Vector3(scaleX, scaleY, 1f);

                SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
                renderer.sprite = bgSprite;
                renderer.sortingOrder = -10;
            }
        }

        static Camera PrepareCamera()
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>();
                cameraObject.AddComponent<AudioListener>();
            }

            camera.orthographic = true;
            camera.orthographicSize = 120f;
            camera.backgroundColor = new Color(0.10f, 0.07f, 0.16f);
            if (camera.GetComponent<CameraFollow>() == null)
                camera.gameObject.AddComponent<CameraFollow>();
            return camera;
        }

        static void BuildScreen(Transform level, int index)
        {
            float width = LevelLayout.ScreenWidth;
            float x0 = index * width;
            int kind = PickKind(index);
            float stripe = index % 2 == 0 ? 1f : 0.82f;
            Color ground = new Color(0.22f * stripe, 0.55f * stripe, 0.2f * stripe);
            Color dirt = new Color(0.45f * stripe, 0.28f * stripe, 0.16f * stripe);

            if (kind == 1)
            {
                float gap = Random.Range(90f, 140f);
                float left = Random.Range(90f, width * 0.35f);
                float rightStart = left + gap;
                Floor(level, x0, left, ground, dirt);
                Floor(level, x0 + rightStart, width - rightStart, ground, dirt);
                PlaceLetterIfNeeded(level, index, x0);
                return;
            }

            Floor(level, x0, width, ground, dirt);

            if (kind == 2)
            {
                float blockX = x0 + width * 0.55f;
                Block(level, "wall", blockX, 16f, 32f, 32f, new Color(0.72f, 0.28f, 0.18f));
            }
            else if (kind == 3)
            {
                int count = Random.Range(3, 6);
                float start = x0 + width * 0.3f;
                for (int i = 0; i < count; i++)
                    PlaceCoin(level, new Vector3(start + i * 28f, 48f + (i % 2) * 24f, 0f));
            }
            else if (kind == 4)
            {
                PlaceEnemy(level, new Vector3(x0 + width * 0.6f, 20f, 0f));
            }
            else if (kind == 5)
            {
                PlaceBerry(level, new Vector3(x0 + width * 0.5f, 24f, 0f));
                if (Random.value > 0.5f)
                    PlaceEnemy(level, new Vector3(x0 + width * 0.75f, 20f, 0f));
            }
            else if (kind == 6)
            {
                PlaceTorch(level, new Vector3(x0 + 36f, 24f, 0f));
                PlaceCoin(level, new Vector3(x0 + width * 0.4f, 56f, 0f));
                PlaceCoin(level, new Vector3(x0 + width * 0.4f + 28f, 56f, 0f));
            }

            PlaceLetterIfNeeded(level, index, x0);
        }

        static void PlaceLetterIfNeeded(Transform level, int index, float x0)
        {
            char[] letters = { 'A', 'T', 'H', 'R', 'E' };
            int[] screens = { 3, 7, 11, 14, 17 };
            float width = LevelLayout.ScreenWidth > 1f ? LevelLayout.ScreenWidth : 360f;

            for (int i = 0; i < letters.Length; i++)
            {
                if (screens[i] != index)
                    continue;

                GameObject pickup = new GameObject("letter_" + letters[i]);
                pickup.transform.SetParent(level, false);
                pickup.transform.position = new Vector3(x0 + width * 0.45f, 36f, 0f);
                LetterPickup letter = pickup.AddComponent<LetterPickup>();
                letter.letter = letters[i];
            }
        }

        static int PickKind(int index)
        {
            if (index == 0)
                return 0;
            if (index == LevelLayout.ScreenCount - 1)
                return 0;
            return Random.Range(0, 7);
        }

        static void BuildGoal(Transform level)
        {
            float x = LevelLayout.Length - 48f;
            Block(level, "flag", x, 48f, 16f, 96f, new Color(0.85f, 0.75f, 0.2f));
            PlaceTorch(level, new Vector3(x, 110f, 0f));
        }

        static void BuildPlayer()
        {
            GameObject player = new GameObject("main_character");
            player.transform.position = new Vector3(80f, 40f, 0f);
            player.AddComponent<Player>();
        }

        static void Floor(Transform level, float x0, float width, Color top, Color dirt)
        {
            if (width <= 4f)
                return;

            Block(level, "ground", x0 + width * 0.5f, -8f, width, 16f, top);
            Block(level, "dirt", x0 + width * 0.5f, -36f, width, 40f, dirt);
        }

        static void PlaceCoin(Transform parent, Vector3 position)
        {
            GameObject coin = new GameObject("moneta");
            coin.SetActive(false);
            coin.transform.SetParent(parent, false);
            coin.transform.position = position;
            Coin pickup = coin.AddComponent<Coin>();
            pickup.tossOnSpawn = false;
            coin.SetActive(true);
        }

        static void PlaceBerry(Transform parent, Vector3 position)
        {
            GameObject berry = new GameObject("truskawka");
            berry.transform.SetParent(parent, false);
            berry.transform.position = position;
            berry.AddComponent<Strawberry>();
        }

        static void PlaceEnemy(Transform parent, Vector3 position)
        {
            GameObject enemy = new GameObject("small_skeleton");
            enemy.transform.SetParent(parent, false);
            enemy.transform.position = position;
            enemy.AddComponent<SkeletonEnemy>();
        }

        static void PlaceTorch(Transform parent, Vector3 position)
        {
            Sprite[] frames = SpriteBank.SliceHorizontal("Pochodnia_001", 16, 16);
            if (frames.Length == 0)
                return;

            GameObject torch = new GameObject("pochodnia");
            torch.transform.SetParent(parent, false);
            torch.transform.position = position;
            SpriteRenderer renderer = torch.AddComponent<SpriteRenderer>();
            renderer.sprite = frames[0];
            renderer.sortingOrder = 1;
        }

        static void Block(Transform parent, string name, float x, float y, float width, float height, Color color)
        {
            GameObject block = new GameObject(name);
            block.transform.SetParent(parent, false);
            block.tag = "Map";
            int map = LayerMask.NameToLayer("Map");
            if (map >= 0)
                block.layer = map;
            block.transform.position = new Vector3(x, y, 0f);
            block.transform.localScale = new Vector3(width, height, 1f);
            SpriteRenderer renderer = block.AddComponent<SpriteRenderer>();
            renderer.sprite = SpriteBank.Solid(Color.white);
            renderer.color = color;
            renderer.sortingOrder = name == "flag" ? 1 : 0;
            block.AddComponent<BoxCollider2D>();
        }
    }
}