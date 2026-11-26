using System;
using UnityEngine;

namespace Zaliczenie
{
    public class Money : MonoBehaviour
    {
        public static Money Instance { get; private set; }

        public int Coins { get; private set; }
        public event Action<int> CoinCollected;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void AddCoin()
        {
            Coins += 1;
            CoinCollected?.Invoke(Coins);
        }
    }
}
