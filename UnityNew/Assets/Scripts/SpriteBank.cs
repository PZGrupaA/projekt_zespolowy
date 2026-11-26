using System.Collections.Generic;
using UnityEngine;

namespace Zaliczenie
{
    public static class SpriteBank
    {
        static readonly Dictionary<string, Sprite> singles = new Dictionary<string, Sprite>();
        static readonly Dictionary<string, Sprite[]> strips = new Dictionary<string, Sprite[]>();
        static Sprite solid;

        public static Sprite LoadSingle(string resourceName)
        {
            if (singles.TryGetValue(resourceName, out Sprite cached))
                return cached;

            Sprite sprite = Resources.Load<Sprite>("Sprites/" + resourceName);
            if (sprite == null)
            {
                Texture2D texture = Resources.Load<Texture2D>("Sprites/" + resourceName);
                if (texture != null)
                    sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1f);
            }

            singles[resourceName] = sprite;
            return sprite;
        }

        public static Sprite[] SliceHorizontal(string resourceName, int frameWidth, int frameHeight)
        {
            string key = resourceName + ":" + frameWidth + "x" + frameHeight;
            if (strips.TryGetValue(key, out Sprite[] cached))
                return cached;

            Texture2D texture = Resources.Load<Texture2D>("Sprites/" + resourceName);
            if (texture == null || frameWidth <= 0 || frameHeight <= 0)
            {
                strips[key] = new Sprite[0];
                return strips[key];
            }

            int count = Mathf.Max(1, texture.width / frameWidth);
            Sprite[] frames = new Sprite[count];
            float y = texture.height - frameHeight;
            for (int i = 0; i < count; i++)
            {
                frames[i] = Sprite.Create(
                    texture,
                    new Rect(i * frameWidth, y, frameWidth, frameHeight),
                    new Vector2(0.5f, 0.5f),
                    1f);
            }

            strips[key] = frames;
            return frames;
        }

        public static Sprite Solid(Color color)
        {
            if (solid != null)
                return solid;

            Texture2D texture = new Texture2D(1, 1);
            texture.filterMode = FilterMode.Point;
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            solid = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            return solid;
        }
    }
}
