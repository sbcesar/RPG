using Godot;
using System;

public partial class enemy : CharacterBody2D
{
	private float speed = 50.0f;
	private bool playerChased;
	private player player;
	private AnimatedSprite2D sprite;
	

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		sprite.Play("front_idle");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (playerChased && player != null)
        {
	        Position += (player.Position - Position).Normalized() * speed * (float)delta;
	        
	        Vector2 direction = (player.Position - Position).Normalized();
	        
	        if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
	        {
		        // Movimiento lateral
		        sprite.Play("side_walk");
		        sprite.FlipH = direction.X < 0;
	        }
	        else
	        {
		        if (direction.Y < 0)
		        {
			        // Movimiento hacia arriba
			        sprite.Play("back_walk");
		        }
		        else
		        {
			        // Movimiento hacia abajo
			        sprite.Play("front_walk");
		        }
	        }
        }
		else
		{
			sprite.Play("front_idle");
		}
		
	}

	private void _on_detection_area_body_entered(Node2D body)
	{
		player = body as player;
		if (player != null)
		{
			playerChased = true;
		}
		
	}

	private void _on_detection_area_body_exited(Node2D body)
	{
		if (body is player)
		{
			player = null;
			playerChased = false;
		}
		
	}
}
