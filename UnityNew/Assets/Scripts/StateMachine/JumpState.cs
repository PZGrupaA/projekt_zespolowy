namespace Zaliczenie
{
    public class JumpState : State
    {
        const int MaxJumps = 5;
        int jumps;

        public override void Enter()
        {
            if (player.IsGrounded)
            {
                player.SetVerticalSpeed(Player.JumpVelocity);
                jumps = 1;
            }
            else
            {
                jumps = 1;
            }
        }

        public override void PhysicsUpdate(float delta)
        {
            if (player.JumpPressed && jumps < MaxJumps && !player.IsGrounded)
            {
                player.SetVerticalSpeed(Player.JumpVelocity * 0.8f);
                jumps += 1;
                player.ConsumeJump();
            }

            float direction = player.Horizontal;
            player.SetHorizontalSpeed(direction * Player.Speed);
            player.Face(direction);

            if (player.IsGrounded && player.Body.velocity.y <= 0f)
                machine.Change(direction != 0f ? "walk_state" : "idle_state");
        }
    }
}
