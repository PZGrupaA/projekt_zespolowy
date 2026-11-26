using UnityEngine;

namespace Zaliczenie
{
    public class WalkState : State
    {
        float idleTimer;
        const float IdleDelay = 1f;

        public override void Enter()
        {
            idleTimer = IdleDelay;
        }

        public override void PhysicsUpdate(float delta)
        {
            float direction = player.Horizontal;
            float target = direction * Player.Speed;
            float next = Mathf.MoveTowards(player.Body.velocity.x, target, Player.Speed);
            player.SetHorizontalSpeed(next);
            player.Face(direction);

            if (player.JumpPressed)
            {
                machine.Change("jump_state");
                return;
            }

            if (direction == 0f && Mathf.Approximately(player.Body.velocity.x, 0f))
            {
                idleTimer -= delta;
                if (idleTimer <= 0f)
                    machine.Change("idle_state");
            }
            else
            {
                idleTimer = IdleDelay;
            }
        }
    }
}
