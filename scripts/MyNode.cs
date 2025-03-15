using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using HttpClient = Godot.HttpClient;

public partial class MyNode : Node
{
	private static string baseUrl = "https://rpgapi.onrender.com";
	private static HttpRequest _httpRequest;
	private int playerScore = globalThings.playerScore;
	public override void _Ready()
	{
		_httpRequest = GetNode<HttpRequest>("HTTPRequest");
		_httpRequest.RequestCompleted += OnRequestCompleted;
	}
	
	/*
	public void InsertScoreOnServer(int score)
	{
		var personaje = new Dictionary<String, Object>
		{
			{ "id", 1 },
			{ "score", score }
		};
		
		string body = JsonConvert.SerializeObject(personaje);
		
		string[] headers = new string[]
		{
			"Content-Type: application/json"
		};

		_httpRequest.Request(baseUrl + "/api/Player/1", headers, HttpClient.Method.Post, body);
	}
	*/

	public static void UpdateScoreOnServer(int score)
	{
		var personaje = new Dictionary<string, object>
		{
			{ "id", 1 },
			{ "Score", score }
		};
		
		string body = JsonConvert.SerializeObject(personaje);

		GD.Print(body);
		
		string[] headers = new string[]
		{
			"Content-Type: application/json"
		};

		_httpRequest.Request(baseUrl + "/api/player/1", headers, HttpClient.Method.Put, body);
	}

	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		if (responseCode == 200)
		{
			string response = Encoding.UTF8.GetString(body);
			GD.Print("Respuesta del servidor: " + response);
		}
		else
		{
			GD.PrintErr("Error al actualizar el score: " + responseCode);
		}
	}
}
