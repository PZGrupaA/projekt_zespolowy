using UnityEngine;

namespace Zaliczenie
{
    public static class GameFont
    {
        static Font font;

        public static Font Get()
        {
            if (font != null)
                return font;

            font = Font.CreateDynamicFontFromOSFont(new[] { "Arial", "Segoe UI", "Tahoma", "Calibri" }, 64);
            return font;
        }
    }
}
