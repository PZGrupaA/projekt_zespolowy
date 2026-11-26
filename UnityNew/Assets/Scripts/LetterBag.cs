using UnityEngine;

namespace Zaliczenie
{
    public class LetterBag : MonoBehaviour
    {
        public const int Size = 5;
        public const string Answer = "HEART";
        public const string Riddle = "I beat without a drum, I race without legs, and I stop only when your story ends";

        public static LetterBag Instance { get; private set; }

        readonly char[] slots = new char[Size];

        public int Count { get; private set; }
        public bool Solved { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public char Get(int index)
        {
            if (index < 0 || index >= Size)
                return '\0';
            return slots[index];
        }

        public bool TryAdd(char letter)
        {
            if (Solved || Count >= Size)
                return false;

            char upper = char.ToUpper(letter);
            for (int i = 0; i < Size; i++)
            {
                if (slots[i] != '\0')
                    continue;

                slots[i] = upper;
                Count += 1;
                CheckAnswer();
                return true;
            }

            return false;
        }

        public void Swap(int a, int b)
        {
            if (Solved || a == b || a < 0 || b < 0 || a >= Size || b >= Size)
                return;

            char saved = slots[a];
            slots[a] = slots[b];
            slots[b] = saved;
            CheckAnswer();
        }

        void CheckAnswer()
        {
            if (Count < Size)
                return;

            if (new string(slots) != Answer)
                return;

            Solved = true;
            LevelLayout.Completed = true;
        }
    }
}
