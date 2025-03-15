using Godot;
using System;
using System.Threading.Tasks;

public partial class boss : CharacterBody2D
{
    
    private float speed = 50.0f;
	private float health = 200.0f;
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

	private async Task deal_with_damage()
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
	                

	                if (player != null)
	                {
		                player.SetScore(500);

		                await ToSignal(GetTree().CreateTimer(0.5f), "timeout"); 
						GetTree().ChangeSceneToFile("res://scenes/end_menu.tscn");
		                
	                }
	                
	                QueueFree();
	                
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
