using Godot;
using System;

public partial class Header : Control
{
	private player playerInstance;
	private Label scoreValueLabel;
	
	public override void _Ready()
	{
		scoreValueLabel = GetNode<Label>("CanvasLayer/HBoxContainer/puntuacion");
		
		if (scoreValueLabel == null)
		{
			GD.PrintErr("No se encontró ScoreLabel/Puntuacion. Revisa la jerarquía.");
		}
		
		string escenaActual = globalThings.current_scene;

		GD.Print($"Escena actual: {escenaActual}");
		
		playerInstance = GetTree().Root.GetNodeOrNull<player>($"{escenaActual}/Player");
		
		if (playerInstance == null)
		{
			GD.PrintErr("No se encontró el nodo Player en la escena.");
		}
	}

	
	public override void _Process(double delta)
	{
		
		scoreValueLabel.Text = globalThings.playerScore.ToString();
		
	}
}
