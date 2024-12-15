using Godot;
using System;

public partial class player : CharacterBody2D
{
    private float speed = 100f;
    private float health = 100f;
    private float attackDamage = 20f;
    private String currentDirection = "none";
    private bool attack_in_progress;
    private bool enemy_in_attack_range;
    private bool enemy_attack_cooldown = true;
    private bool player_alive = true;
    private AnimatedSprite2D sprite;
    private ProgressBar healthBar;
    private Timer timer;
    private Timer attack_timer;
    private Timer regen_timer;

    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        healthBar = GetNode<ProgressBar>("ProgressBar");
        timer = GetNode<Timer>("attack_cooldown");
        attack_timer = GetNode<Timer>("deal_attack");
        regen_timer = GetNode<Timer>("regen_timer");
        sprite.Play("front_idle");
    }

    public override void _PhysicsProcess(double delta)
    {
        player_movement(delta);
        enemy_attack();
        attack();
        update_health();
        showMenu();
        
        
        if (health <= 0)
        {
            player_alive = false;
            health = 0f;
            
            GetTree().ChangeSceneToFile("res://scenes/end_menu.tscn");
        }
    }
    
    public float GetAttackDamage()
    {
        return attackDamage;
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
            if (health < 0) health = 0;
            enemy_attack_cooldown = false;
            timer.Start();
            GD.Print("Player health: " + health);
            update_health();
        }
        
    }

    private void _on_attack_cooldown_timeout()
    {
        enemy_attack_cooldown = true;
    }

    private void _on_regen_timer_timeout()
    {
        if (health < 100)
        {
            health += 20f;
            if (health > 100)
            {
                health = 100;
            }
        }

        update_health();
    }

    private void update_health()
    {
        healthBar.Value = health;
    }

    private void showMenu()
    {
        // Verifica si la acción "options" ha sido presionada
        if (Input.IsActionJustPressed("options"))
        {
            GD.Print("Options button pressed"); // Mensaje de depuración

            // Verifica en qué escena estamos actualmente
            if (GetTree().CurrentScene.Name == "menu")
            {
                GD.Print("Currently in the menu scene, changing to world"); // Mensaje de depuración
                // Cambiar a la escena "world"
                GetTree().ChangeSceneToFile("res://scenes/world.tscn");
            }
            else if (GetTree().CurrentScene.Name == "world")
            {
                GD.Print("Currently in the world scene, changing to menu"); // Mensaje de depuración
                // Cambiar a la escena "menu"
                GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
            }
            else if (GetTree().CurrentScene.Name == "route2")
            {
                GD.Print("Currently in the route 2 scene, changing to menu"); // Mensaje de depuración
                // Cambiar a la escena "menu"
                GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
            }
            else
            {
                GD.Print("Not in menu or world or route 2 scene"); // Mensaje de depuración
            }
        }
    }





    private void player_movement(double delta)
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
            globalThings.player_current_attack = true;
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
        globalThings.player_current_attack = false;
        attack_in_progress = false;
    }
    
    public void IncreaseAttackDamage(float amount)
    {
        attackDamage += amount;
        GD.Print("Nuevo daño de ataque: " + attackDamage);
    }
}
