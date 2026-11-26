namespace Zaliczenie
{
    public abstract class State
    {
        protected StateMachine machine;
        protected Player player;

        public void Bind(StateMachine stateMachine, Player owner)
        {
            machine = stateMachine;
            player = owner;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void PhysicsUpdate(float delta) { }
    }
}
