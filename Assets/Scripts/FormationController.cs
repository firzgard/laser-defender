using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;

public class FormationController : MonoBehaviour
{
	public int Lvl = 0;
	public float Width = 20;
	public float Height = 6;
	public float XAmplitute = 7;
	public float YAmplitute = 4;
	public float XFreq = .2f;
	public float YFreq = .2f;
	public float XFreqLvlUpRate = .1f;
	public float SpawnInterval = .5f;
	public float RelocationTime = 1;
	public float TimeToNextWave = 1;
	public float RareEnemyRate = .3f;

	public GameObject EnemyPrefab;
	public GameObject Enemy2Prefab;
	public GameObject SpawnPos;


	Vector3 _originalPosition;

	public void CheckToStartNextWave()
	{
		var childCount = transform.Cast<Transform>().Count(child => child.childCount > 0);

		if (childCount <= 1)
		{
			Invoke("StartNextWave", TimeToNextWave);
		}
	}

	void StartNextWave()
	{
		Lvl++;
		Respawn();
	}

	void LvlUp()
	{
		XFreq += XFreqLvlUpRate;
	}

	void Respawn()
	{
		var i = 0;
		// Spawn enemy inside EnemyFormation
		foreach (Transform child in transform)
		{
			try
			{
				StartCoroutine(RespawnEachPosition(child, SpawnInterval * i++));
			}
			catch (NullReferenceException e)
			{
				Debug.Log(e.Message);
			}
		}
	}

	IEnumerator RespawnEachPosition(Transform pos, float time)
	{
		yield return new WaitForSeconds(time);

		var prefab = Random.value > RareEnemyRate ? EnemyPrefab : Enemy2Prefab;

		// Spawn enemy to spawn position
		var enemy = Instantiate(prefab
			, SpawnPos.transform.position
			, Quaternion.identity);

		// Start moving enemy to position inside formation
		try
		{
			StartCoroutine(enemy.GetComponent<EnemyBehavior>().MoveToPosition(RelocationTime, pos));
		}
		catch (Exception e)
		{
		}
	}

	void DoHarmonicMove()
	{
		// Calc formation's position using harmonic motion
		var xPosition = _originalPosition.x + XAmplitute * Mathf.Cos(2 * Mathf.PI * XFreq * Time.timeSinceLevelLoad);
		var yPosition = _originalPosition.y + YAmplitute * Mathf.Cos(2 * Mathf.PI * YFreq * Time.timeSinceLevelLoad);


		// Move enemy's formation
		transform.position = new Vector3(xPosition, yPosition);
	}


	// Use this for initialization
	void Start ()
	{
		// Init private values
		_originalPosition = transform.position;

		Respawn();
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height));
	}
	
	// Update is called once per frame
	void Update ()
	{
		DoHarmonicMove();
	}
}
