using Godot;
using System;

public partial class globalThings : Node
{
	public static bool player_current_attack = false;
	public static String current_scene = "world";
	public static bool transition_scene = false;
	public static int player_position_x = 46;
	public static int player_position_y = 113;
	public static int player_start_route_1_posx = 663;
	public static int player_start_route_1_posy = 120;
	public static string last_scene = "";
	public static bool game_first_load = true;

	public static void finish_changescenes()
	{
		if (transition_scene)
		{
			transition_scene = false;

			// Guarda la última escena antes de cambiar
			last_scene = current_scene;

			if (current_scene == "world")
			{
				// Al salir de `world`, guarda la posición actual
				player_position_x = player_start_route_1_posx;
				player_position_y = player_start_route_1_posy;
				current_scene = "route_2";
			}
			else
			{
				// Al salir de `route_2`, guarda la posición deseada
				player_position_x = 669;
				player_position_y = 122;
				current_scene = "world";
			}
		}
	}

	
	public static void ResetPlayerPosition(CharacterBody2D player)
	{
		if (current_scene == "world")
		{
			player.Position = new Vector2(player_position_x, player_position_y);
		}
		else if (current_scene == "route_2")
		{
			player.Position = new Vector2(player_start_route_1_posx, player_start_route_1_posy);
		}
	}
}
