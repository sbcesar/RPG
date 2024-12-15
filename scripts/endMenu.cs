using Godot;
using System;

public partial class endMenu : Control
{
	private void _on_quit_pressed()
	{
		GetTree().Quit();
	}
}
