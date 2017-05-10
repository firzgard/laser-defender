using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour
{
	public float Hp = 20;
	public int Points = 100;
	public float MaxReload = 2;
	public float MinReload = 1;
	public float PerHpRaisedPerLvl = 1.5f;
	public float PerPointsRaisedPerLvl = 2;
	public float PerMaxReloadReducedPerLvl = .9f;
	public float PerMinReloadReducedPerLvl = .9f;
	public float MaxSfxLifeTime = 3;

	public GameObject ProjectileObj;
	public GameObject ExplosionVfx;
	public GameObject GetHitVfx;
	public AudioClip FireSfx;
	public AudioClip DieSfx;
		
	private float _timeOfNextShot;
	private static FormationController _formation;


	public IEnumerator MoveToPosition(float time, Transform target)
	{
		Reload(time);

		var elapsedTime = 0f;
		var initPos = transform.position;

		while (elapsedTime < time)
		{
			transform.position = Vector3.Lerp(initPos, target.position, elapsedTime / time);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}

		// Parenting to formation's position
		transform.parent = target;
	}

	void GetHit(float dmg, Vector3 pos)
	{
		Hp -= dmg;
		
		// Fk'd if all hp depleted
		if (Hp <= 0)
		{
			// Start the explosion xFXs
			SoundManager.PlayOneShot2d(DieSfx);
			var explosionInstance = Instantiate(ExplosionVfx, transform.position, transform.rotation);
			Destroy(explosionInstance, MaxSfxLifeTime);

			// Check if formation is empty to respawn
			_formation.CheckToStartNextWave();

			ScoreKeeper.Score(Points);

			StopAllCoroutines();
			Destroy(gameObject);
		}
		else
		{
			var getHitSfxInstance = Instantiate(GetHitVfx, pos, Quaternion.identity);
			Destroy(getHitSfxInstance, MaxSfxLifeTime);
		}
	}

	void Fire()
	{
		var beamObj = Instantiate(ProjectileObj
					, gameObject.transform.position + new Vector3(0, -.3f, 1)
					, gameObject.transform.rotation * Quaternion.Euler(0,0,180))
				as GameObject;
		var beam = beamObj.GetComponent<Projectile>();
		beam.Fire();

		SoundManager.PlayOneShot2d(FireSfx);

		Reload();
	}

	void Reload()
	{
		// Frame nex shot using min/max firing rate
		_timeOfNextShot = Time.unscaledTime + Random.Range(MinReload, MaxReload);
	}

	void Reload(float time)
	{
		_timeOfNextShot = Time.unscaledTime + Random.Range(MinReload + time, MaxReload + time);
	}


	void Start()
	{
		if(!_formation)
			_formation = FindObjectOfType<FormationController>();

		Hp *= Mathf.Pow(PerHpRaisedPerLvl, _formation.Lvl);
		MaxReload *=  Mathf.Pow(PerMaxReloadReducedPerLvl, _formation.Lvl);
		MinReload *= Mathf.Pow(PerMinReloadReducedPerLvl, _formation.Lvl);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		// Handle being hit by player
		var projectile = col.gameObject.GetComponent<Projectile>();
		if (projectile)
		{
			GetHit(projectile.Dmg, col.transform.position);
			projectile.Hit();
		}
	}

	void Update()
	{
		// Fire next shot
		if (_timeOfNextShot <= Time.unscaledTime)
		{
			Fire();
		}
	}
}
