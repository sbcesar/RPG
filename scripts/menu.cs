using Godot;
using System;

public partial class menu : Control
{

	private void _on_play_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/world.tscn");
	}
	
	private void _on_quit_pressed()
	{
		GetTree().Quit();
	}
}
