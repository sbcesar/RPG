using Godot;
using System;

public partial class player : CharacterBody2D
{
    public static int score;
    private float speed = 100f;
    private float health = 100f;
    private float baseAttackDamage = 20f;
    private float attackDamage = 20f;
    private String currentDirection = "none";
    private bool attack_in_progress;
    private bool isChargingAttack = false;
    private bool enemy_in_attack_range;
    private bool enemy_attack_cooldown = true;
    private bool player_alive = true;
    private AnimatedSprite2D sprite;
    private Sprite2D buffIcon;
    private ProgressBar healthBar;
    private Timer timer;
    private Timer attack_timer;
    private Timer regen_timer;
    private Timer charge_attack_timer;

    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        buffIcon = GetNode<Sprite2D>("buff_icon");
        healthBar = GetNode<ProgressBar>("ProgressBar");
        timer = GetNode<Timer>("attack_cooldown");
        attack_timer = GetNode<Timer>("deal_attack");
        regen_timer = GetNode<Timer>("regen_timer");
        charge_attack_timer = GetNode<Timer>("charge_attack");
        sprite.Play("front_idle");
    }

    public override void _PhysicsProcess(double delta)
    {
        player_movement(delta);
        enemy_attack();
        Attack();
        update_health();
        
        if (Input.IsActionJustPressed("options"))
        {
            PauseGame();
        }
        
        
        if (health <= 0)
        {
            player_alive = false;
            health = 0f;
            
            GetTree().ChangeSceneToFile("res://scenes/end_menu.tscn");
        }
        
    }

    private void PauseGame()
    {
        GetTree().Paused = true;
        
        var pauseMenu = GD.Load<PackedScene>("res://scenes/pause_menu.tscn").Instantiate<PauseMenu>();
        GetTree().Root.AddChild(pauseMenu);
    }

    public int GetScore()
    {
        return score;
    }
    
    public static void SetScore(int newScore)
    {
        score += newScore;
        globalThings.UpdatePlayerScore(score);
        globalThings.playerScore = score;
        GD.Print("Puntos actuales: " + score);
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
    
    private void player_movement(double delta)
    {
        Vector2 velocity = Vector2.Zero; // Reiniciar la velocidad en cada frame

        if (Input.IsActionPressed("right"))
        {
            currentDirection = "right";
            play_animation(1);
            velocity.X = speed;
        }
        else if (Input.IsActionPressed("left"))
        {
            currentDirection = "left";
            play_animation(1);
            velocity.X = -speed;
        }
        else if (Input.IsActionPressed("down"))
        {
            currentDirection = "down";
            play_animation(1);
            velocity.Y = speed;
        }
        else if (Input.IsActionPressed("up"))
        {
            currentDirection = "up";
            play_animation(1);
            velocity.Y = -speed;
        }
        else
        {
            play_animation(0); // Animación de idle
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

    private void Attack()
    {
        String dir = currentDirection;

        if (Input.IsActionJustPressed("attack"))
        {
            isChargingAttack = true;
            charge_attack_timer.Start();
        }

        if (Input.IsActionJustReleased("attack"))
        {
            attack_in_progress = true;
            globalThings.player_current_attack = true;
            
            if (charge_attack_timer.TimeLeft <= 0)
            {
                sprite.SpeedScale = 0.5f;
                attackDamage = baseAttackDamage + 5f;
                GD.Print("¡Ataque cargado ejecutado! Daño aumentado a: " + attackDamage);
            }
            else
            {
                sprite.SpeedScale = 1.0f;
                attackDamage = baseAttackDamage;
            }
            
            if (dir == "right" || dir == "left")
            {
                sprite.Play("side_attack");
                sprite.FlipH = (dir == "left");
            }
            else if (dir == "down")
            {
                sprite.Play("front_attack");
            }
            else if (dir == "up")
            {
                sprite.Play("back_attack");
            }

            attack_timer.Start();
            charge_attack_timer.Stop();
            isChargingAttack = false;
        }
    }
    
    private void _on_deal_attack_timeout()
    {
        attack_timer.Stop();
        globalThings.player_current_attack = false;
        attack_in_progress = false;
        sprite.SpeedScale = 1.0f;

        if (attackDamage == 40f) attackDamage = 40f;
        if (attackDamage == 20f) attackDamage = 20f;
        
    }
    
    public void IncreaseAttackDamage(float amount)
    {
        buffIcon.Visible = true;
        baseAttackDamage += amount;
        GD.Print("Nuevo daño de ataque base: " + baseAttackDamage);
    }
}
