using Godot;
using System;

public partial class player : CharacterBody2D
{
    public static int score;
    private float speed = 100f;
    private float health = 100f;
    private float attackDamage = 20f;
    private String currentDirection = "none";
    private bool attack_in_progress;
    private bool isChargingAttack = false;
    private bool enemy_in_attack_range;
    private bool enemy_attack_cooldown = true;
    private bool player_alive = true;
    private AnimatedSprite2D sprite;
    private ProgressBar healthBar;
    private Timer timer;
    private Timer attack_timer;
    private Timer regen_timer;
    private Timer charge_attack_timer;

    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
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
        showMenu();
        
        
        if (health <= 0)
        {
            player_alive = false;
            health = 0f;
            
            GetTree().ChangeSceneToFile("res://scenes/end_menu.tscn");
        }
        
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

            // Aplicar daño cargado si se completó la carga
            if (charge_attack_timer.TimeLeft <= 0)
            {
                sprite.SpeedScale = 0.5f;
                attackDamage += 5f;
                GD.Print("¡Ataque cargado ejecutado! Daño aumentado a: " + attackDamage);
            }
            else
            {
                sprite.SpeedScale = 1.0f;
                attackDamage = 20f;
            }

            // Seleccionar animación dependiendo de la dirección del jugador
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

    // Restaurar velocidad y daño después del ataque
    private void _on_deal_attack_timeout()
    {
        attack_timer.Stop();
        globalThings.player_current_attack = false;
        attack_in_progress = false;
        sprite.SpeedScale = 1.0f; // Restaurar la velocidad normal

        // Restaurar el daño base después del ataque cargado
        attackDamage = 20f;
    }
    
    public void IncreaseAttackDamage(float amount)
    {
        attackDamage += amount;
        GD.Print("Nuevo daño de ataque: " + attackDamage);
    }
}
