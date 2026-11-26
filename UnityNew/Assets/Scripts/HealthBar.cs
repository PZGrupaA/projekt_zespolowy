using UnityEngine;

namespace Zaliczenie
{
    public class HealthBar : MonoBehaviour
    {
        Transform fill;
        int max = 1;
        bool maxSet;

        public void Bind(Health health)
        {
            health.HealthChanged += OnHealthChanged;
            OnHealthChanged(health.Current > 0 ? health.Current : health.MaxHealth);
        }

        void Awake()
        {
            SpriteRenderer back = CreatePart("back", 22f, 4f, 0);
            back.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);
            SpriteRenderer fillRenderer = CreatePart("fill", 20f, 2f, 1);
            fillRenderer.color = new Color(0.75f, 0.15f, 0.15f);
            fill = fillRenderer.transform;
        }

        SpriteRenderer CreatePart(string partName, float width, float height, int order)
        {
            GameObject part = new GameObject(partName);
            part.transform.SetParent(transform, false);
            part.transform.localScale = new Vector3(width, height, 1f);
            SpriteRenderer renderer = part.AddComponent<SpriteRenderer>();
            renderer.sprite = SpriteBank.Solid(Color.white);
            renderer.sortingOrder = 5 + order;
            return renderer;
        }

        void OnHealthChanged(int amount)
        {
            if (!maxSet)
            {
                max = Mathf.Max(1, amount);
                maxSet = true;
            }

            float ratio = Mathf.Clamp01(amount / (float)max);
            fill.localScale = new Vector3(20f * ratio, 2f, 1f);
            fill.localPosition = new Vector3(-10f * (1f - ratio), 0f, 0f);
        }
    }
}
