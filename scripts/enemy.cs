using Godot;
using System;

public partial class enemy : CharacterBody2D
{
	private float speed = 50.0f;
	private float health = 100.0f;
	private bool player_in_attack_range;
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
		deal_with_damage();
		
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
	
	private void _on_player_hitbox_body_entered(Node2D body)
	{
		if (body.HasMethod("playerItself"))
		{
			player_in_attack_range = true;
		}
	}

	private void _on_player_hitbox_body_exited(Node2D body)
	{
		if (body.HasMethod("playerItself"))
		{
			player_in_attack_range = false;
		}
	}

	private void deal_with_damage()
	{
		if (player_in_attack_range && world.player_current_attack)
		{
			health -= 20;
			if (health <= 0)
			{
				QueueFree();
			}
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

	private void enemyItself()
	{
		
	}
}
