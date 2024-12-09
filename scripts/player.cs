using Godot;
using System;

public partial class player : CharacterBody2D
{
    private float speed = 100f;
    private float health = 200f;
    private String currentDirection = "none";
    private bool attack_in_progress;
    private bool enemy_in_attack_range;
    private bool enemy_attack_cooldown = true;
    private bool player_alive = true;
    private AnimatedSprite2D sprite;
    private Timer timer;
    private Timer attack_timer;

    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        timer = GetNode<Timer>("attack_cooldown");
        attack_timer = GetNode<Timer>("deal_attack");
        sprite.Play("front_idle");
    }

    public override void _PhysicsProcess(double delta)
    {
        player_movement(delta);
        enemy_attack();
        attack();
        
        
        if (health <= 0)
        {
            player_alive = false;   // End game or respawn
            health = 0f;
            GD.Print("Player has been killed");
            QueueFree();
        }
    }

    private void _on_player_hitbox_body_entered(Node2D body)
    {
        if (body.HasMethod("enemyItself"))
        {
            enemy_in_attack_range = true;
        }
    }

    private void _on_player_hitbox_body_exited(Node2D body)
    {
        if (body.HasMethod("enemyItself"))
        {
            enemy_in_attack_range = false;
        }
    }

    private void playerItself()
    {
        
    }

    private void enemy_attack()
    {
        if (enemy_in_attack_range && enemy_attack_cooldown)
        {
            health -= 20f;
            enemy_attack_cooldown = false;
            timer.Start();
            GD.Print(health);
        }
        
    }

    private void _on_attack_cooldown_timeout()
    {
        enemy_attack_cooldown = true;
    }

    public void player_movement(double delta)
    {
        Vector2 velocity = Velocity;
        
        if (Input.IsActionPressed("right"))
        {
            currentDirection = "right";
            play_animation(1);
            velocity.X = speed;
            velocity.Y = 0;
        } 
        else if (Input.IsActionPressed("left"))
        {
            currentDirection = "left";
            play_animation(1);
            velocity.X = -speed;
            velocity.Y = 0;
        }
        else if (Input.IsActionPressed("down"))
        {
            currentDirection = "down";
            play_animation(1);
            velocity.Y = speed;
            velocity.X = 0;
        }
        else if (Input.IsActionPressed("up"))
        {
            currentDirection = "up";
            play_animation(1);
            velocity.Y = -speed;
            velocity.X = 0;
        }
        else
        {
            play_animation(0);
            velocity.X = 0;
            velocity.Y = 0;
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public void play_animation(int movement)
    {
        String dir = currentDirection;

        if (dir == "right")
        {
            sprite.FlipH = false;
            if (movement == 1)
            {
                sprite.Play("side_walk");
            }
            else if (movement == 0)
            {
                if (attack_in_progress == false)
                {
                    sprite.Play("side_idle");
                }
                
            }
        }
        if (dir == "left")
        {
            sprite.FlipH = true;
            if (movement == 1)
            {
                sprite.Play("side_walk");
            }
            else if (movement == 0)
            {
                if (attack_in_progress == false)
                {
                    sprite.Play("side_idle");
                }
            }
        }
        if (dir == "down")
        {
            sprite.FlipH = true;
            if (movement == 1)
            {
                sprite.Play("front_walk");
            }
            else if (movement == 0)
            {
                if (attack_in_progress == false)
                {
                    sprite.Play("front_idle");
                }
            }
        }
        if (dir == "up")
        {
            sprite.FlipH = true;
            if (movement == 1)
            {
                sprite.Play("back_walk");
            }
            else if (movement == 0)
            {
                if (attack_in_progress == false)
                {
                    sprite.Play("back_idle");
                }
            }
        }
    }

    private void attack()
    {
        String dir = currentDirection;
        if (Input.IsActionJustPressed("attack"))
        {
            world.player_current_attack = true;
            attack_in_progress = true;

            if (dir == "right")
            {
                sprite.FlipH = false;
                sprite.Play("side_attack");
                attack_timer.Start();
            }
            if (dir == "left")
            {
                sprite.FlipH = true;
                sprite.Play("side_attack");
                attack_timer.Start();
            }
            if (dir == "down")
            {
                sprite.Play("front_attack");
                attack_timer.Start();
            }
            if (dir == "up")
            {
                sprite.Play("back_attack");
                attack_timer.Start();
            }
        }
    }

    private void _on_deal_attack_timeout()
    {
        attack_timer.Stop();
        world.player_current_attack = false;
        attack_in_progress = false;
    }
}
