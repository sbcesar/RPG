using Godot;
using System;

public partial class world : Node2D
{
	private CharacterBody2D player;
	
	
	public override void _Ready()
	{
		player = GetNode<CharacterBody2D>("Player");

		
		Vector2 position = new Vector2(globalThings.player_position_x, globalThings.player_position_y);
		player.Position = position;
	}


	public override void _Process(double delta)
	{
		change_escene();
	}

	public void _on_route_transition_point_body_entered(Node2D body)
	{
		if (body.HasMethod("playerItself"))
		{
			globalThings.transition_scene = true;
		}
	}

	public void _on_route_transition_point_body_exited(Node2D body)
	{
		if (body.HasMethod("playerItself"))
		{
			globalThings.transition_scene = false;
		}
	}

	public void change_escene()
	{
		if (globalThings.transition_scene)
		{
			// Determina qué escena cargar
			if (globalThings.current_scene == "world")
			{
				globalThings.current_scene = "route_2";
				GetTree().ChangeSceneToFile("res://scenes/route_2.tscn");
			}
			else if (globalThings.current_scene == "route_2")
			{
				globalThings.current_scene = "world";
				GetTree().ChangeSceneToFile("res://scenes/world.tscn");
			}

			// Configura los estados necesarios
			globalThings.game_first_load = false;
			globalThings.finish_changescenes();
		}
	}
}
