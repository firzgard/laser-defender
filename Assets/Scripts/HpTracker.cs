using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpTracker : MonoBehaviour {
	
	private static HpTracker _instance = null;

	public static void PostHp(int Hp)
	{
		_instance.GetComponent<Text>().text = "HP: " + Hp;
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
		}

	}
}
