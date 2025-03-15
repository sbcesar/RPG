using Godot;
using System;

public partial class enemy : CharacterBody2D
{
	private float speed = 50.0f;
	private float health = 100.0f;
	private bool player_in_attack_range;
	private bool playerChased;
	private bool can_take_damage = true;
	private player player;
	private AnimatedSprite2D sprite;
	private ProgressBar healthBar;
	private Timer take_damage_timer;
	

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		healthBar = GetNode<ProgressBar>("ProgressBar");
		take_damage_timer = GetNode<Timer>("take_damage_cooldown");
		sprite.Play("front_idle");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		deal_with_damage();

		if (playerChased && player != null)
		{
			Vector2 direction = (player.Position - Position).Normalized();
			Velocity = direction * speed;

			if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
			{
				sprite.Play("side_walk");
				sprite.FlipH = direction.X < 0;
			}
			else
			{
				sprite.Play(direction.Y < 0 ? "back_walk" : "front_walk");
			}
		}
		else
		{
			Velocity = Vector2.Zero;
			sprite.Play("front_idle");
		}

		MoveAndSlide();
	}

	
	private void _on_enemy_hitbox_body_entered(Node2D body)
	{
		if (body.HasMethod("playerItself"))
		{
			player_in_attack_range = true;
		}
	}

	private void _on_enemy_hitbox_body_exited(Node2D body)
	{
		if (body.HasMethod("playerItself"))
		{
			player_in_attack_range = false;
		}
	}

	private void deal_with_damage()
	{
		if (player_in_attack_range && globalThings.player_current_attack)
		{
			if (can_take_damage)
			{
				health -= player.GetAttackDamage();
				take_damage_timer.Start();
				can_take_damage = false;
				GD.Print("Slime health: " + health);
				update_health();

				

				if (health <= 0f)
				{
					QueueFree();
					if (player != null)
					{
						player.SetScore(10);
					}
				}
			}
		}
	}


	private void update_health()
	{
		healthBar.Value = health;
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

	private void _on_take_damage_cooldown_timeout()
	{
		can_take_damage = true;
	}
}
