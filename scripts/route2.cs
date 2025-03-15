using Godot;
using System;

public partial class route2 : Node2D
{
	
	public override void _Process(double delta)
	{
		change_scenes();
	}

	private void _on_route_2_exit_body_entered(Node2D body)
	{
		if (body.HasMethod("playerItself"))
		{
			globalThings.transition_scene = true;
		}
	}

	private void _on_route_2_exit_body_exited(Node2D body)
	{
		if (body.HasMethod("playerItself"))
		{
			globalThings.transition_scene = false;
		}
	}

	private void change_scenes()
	{
		if (globalThings.transition_scene)
		{
			if (globalThings.current_scene == "route2")
			{
				// Actualiza la posición deseada ANTES de cambiar de escena
				globalThings.player_position_x = 669;
				globalThings.player_position_y = 122;

				globalThings.current_scene = "world";
				globalThings.transition_scene = false;

				// Cambia de escena
				GetTree().ChangeSceneToFile("res://scenes/world.tscn");
			}
		}
	}
}
