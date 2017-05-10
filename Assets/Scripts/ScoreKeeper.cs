using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
{
	public string ScoreFormat = "D7";

	private static ScoreKeeper _instance = null;
	private static int _score = 0;

	public static int Score(int points)
	{
		_score += points;
		_instance.GetComponent<Text>().text = _score.ToString(_instance.ScoreFormat);
		return _score;
	}

	public void ResetScore()
	{
		_score = 0;
		_instance.GetComponent<Text>().text = _score.ToString(_instance.ScoreFormat);
	}


	void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(gameObject);
			print("Duplicate self-destructing!");
		}
		else
		{
			_instance = this;
			
			_instance.GetComponent<Text>().text = _score.ToString(ScoreFormat);
		}

	}
}
