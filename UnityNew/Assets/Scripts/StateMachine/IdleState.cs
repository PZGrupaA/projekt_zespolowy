namespace Zaliczenie
{
    public class IdleState : State
    {
        public override void Enter()
        {
            player.SetHorizontalSpeed(0f);
        }

        public override void PhysicsUpdate(float delta)
        {
            player.SetHorizontalSpeed(0f);
            if (player.Horizontal != 0f)
                machine.Change("walk_state");
            else if (player.JumpPressed)
                machine.Change("jump_state");
        }
    }
}
