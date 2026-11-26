using System.Collections.Generic;

namespace Zaliczenie
{
    public class StateMachine
    {
        readonly Dictionary<string, State> states = new Dictionary<string, State>();
        public State Current { get; private set; }

        public void Register(string name, State state, Player player)
        {
            state.Bind(this, player);
            states[name.ToLower()] = state;
        }

        public void Change(string name)
        {
            if (Current != null)
                Current.Exit();

            states.TryGetValue(name.ToLower(), out State next);
            Current = next;
            if (Current != null)
                Current.Enter();
        }

        public void PhysicsUpdate(float delta)
        {
            if (Current != null)
                Current.PhysicsUpdate(delta);
        }
    }
}
