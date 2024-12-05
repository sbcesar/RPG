using Godot;
using System;

public partial class player : CharacterBody2D
{
    const float speed = 100f;
    
    String currentDirection = "none";

    private AnimatedSprite2D sprite;

    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        sprite.Play("front_idle");
    }

    public override void _PhysicsProcess(double delta)
    {
        player_movement(delta);
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
                sprite.Play("side_idle");
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
                sprite.Play("side_idle");
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
                sprite.Play("front_idle");
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
                sprite.Play("back_idle");
            }
        }
    }
}
