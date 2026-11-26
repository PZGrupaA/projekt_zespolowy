using System;
using UnityEngine;

namespace Zaliczenie
{
    public class Health : MonoBehaviour
    {
        public int MaxHealth { get; private set; }
        public int Current { get; private set; }

        public event Action<int> HealthChanged;
        public event Action Died;

        bool dead;

        public void Init(int maxHealth)
        {
            MaxHealth = maxHealth;
            Current = maxHealth;
            dead = false;
            HealthChanged?.Invoke(Current);
        }

        public void DamageTaken(int amount)
        {
            if (dead)
                return;

            Current -= amount;
            HealthChanged?.Invoke(Current);
            if (Current <= 0)
            {
                dead = true;
                Died?.Invoke();
            }
        }

        public void Heal(int amount)
        {
            Current = Mathf.Min(MaxHealth, Current + amount);
            HealthChanged?.Invoke(Current);
        }

        public void RestoreFull()
        {
            dead = false;
            Current = MaxHealth;
            HealthChanged?.Invoke(Current);
        }
    }
}
