using Godot;
using System;

public partial class sword : Area2D
{
    [Export] public float DamageIncrease = 20f;

    private void _on_body_entered(Node body)
    {
        if (body is player playerNode)
        {
            playerNode.IncreaseAttackDamage(DamageIncrease);
            QueueFree();
            GD.Print("Daño incrementado en: " + DamageIncrease);
        }
    }
}
