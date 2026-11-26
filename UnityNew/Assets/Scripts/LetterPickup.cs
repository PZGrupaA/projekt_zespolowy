using UnityEngine;

namespace Zaliczenie
{
    public class LetterPickup : MonoBehaviour
    {
        public char letter = 'A';

        Vector3 origin;
        TextMesh label;

        void Start()
        {
            origin = transform.position;

            GameObject tile = new GameObject("tile");
            tile.transform.SetParent(transform, false);
            tile.transform.localScale = new Vector3(24f, 28f, 1f);
            SpriteRenderer plate = tile.AddComponent<SpriteRenderer>();
            plate.sprite = SpriteBank.Solid(Color.white);
            plate.color = new Color(0.25f, 0.12f, 0.35f, 1f);
            plate.sortingOrder = 15;

            GameObject textObject = new GameObject("glyph");
            textObject.transform.SetParent(transform, false);
            textObject.transform.localPosition = new Vector3(0f, 0f, -0.2f);
            label = textObject.AddComponent<TextMesh>();
            label.text = letter.ToString();
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.fontSize = 64;
            label.characterSize = 0.3f;
            label.fontStyle = FontStyle.Bold;
            label.color = new Color(1f, 0.92f, 0.45f, 1f);

            Font font = GameFont.Get();
            if (font != null)
            {
                string needed = label.text + letter.ToString().ToUpper() + letter.ToString().ToLower();
                font.RequestCharactersInTexture(needed, label.fontSize, label.fontStyle);
                label.font = font;
                MeshRenderer mesh = textObject.GetComponent<MeshRenderer>();
                if (mesh != null)
                {
                    mesh.material = font.material;
                    mesh.sortingOrder = 16;
                }
            }

            CircleCollider2D circle = gameObject.AddComponent<CircleCollider2D>();
            circle.isTrigger = true;
            circle.radius = 18f;
        }

        void Update()
        {
            float bob = Mathf.Sin(Time.time * 3f + origin.x * 0.01f) * 4f;
            transform.position = origin + new Vector3(0f, bob, 0f);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (LetterBag.Instance == null)
                return;

            if (!LetterBag.Instance.TryAdd(letter))
                return;

            Destroy(gameObject);
        }
    }
}