using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	public int Hp = 30;
	public float HorizontalSpeed = 5;
	public float Padding = 1;
	public float MaxSfxLifeTime = 3;
	public float ChargeTime = 1.5f;

	public GameObject Beam;
	public GameObject ChargedBeam;
	public AudioClip FireSfx;
	public AudioClip DieSfx;
	public GameObject GetHitVfx;
	public GameObject ExplosionVfx;
	public GameObject ChargeVfx;
	public GameObject DisChargeVfx;

	float _maxX;
	float _minX;
	float _chargeTime;

	int chargeStage;
	GameObject _chargeVfx;
	GameObject _dischargeVfx;
	ParticleSystem _chargeParticleSys;
	ParticleSystem _dischargeParticleSys;

	void Charge()
	{
		if (_chargeTime > ChargeTime)
		{
			// Switch to discharge animation if not already
			if (chargeStage == 1)
			{
				_chargeParticleSys.Stop();
				_dischargeParticleSys.Play();

				chargeStage++;
			}
		}
		else
		{
			_chargeTime += Time.deltaTime;

			// Turn on charge animation if not already on
			if (chargeStage == 0)
			{
				_chargeParticleSys.Play();

				chargeStage++;
			}
		}
		
	}

	void Fire()
	{
		GameObject beamPrefarb;
		switch (chargeStage)
		{
			case 2:
				beamPrefarb = ChargedBeam;
				_dischargeParticleSys.Stop();
				break;
			case 1:
				beamPrefarb = Beam;
				_chargeParticleSys.Stop();
				break;
			default:
				beamPrefarb = Beam;
				break;
		}

		var beamObj = Instantiate(beamPrefarb, transform.position + new Vector3(0, .2f, 1), transform.rotation) as GameObject;
		var beam = beamObj.GetComponent<Projectile>();
		beam.Fire();

		SoundManager.PlayOneShot2d(FireSfx);

		chargeStage = 0;
		_chargeTime = 0;
	}

	void GetHit(int dmg, Vector3 pos)
	{
		Hp -= dmg;
		Hp = Hp < 0 ? 0 : Hp;
		HpTracker.PostHp(Hp);

		// Fk'd if all hp depleted
		if (Hp == 0) Die();
		else
		{
			var getHitSfxInstance = Instantiate(GetHitVfx, pos, Quaternion.identity);
			Destroy(getHitSfxInstance, MaxSfxLifeTime);
		}
	}

	void Die()
	{
		gameObject.GetComponent<SpriteRenderer>().enabled = false;

		// Start the explosion xFXs
		SoundManager.PlayOneShot2d(DieSfx);
		var explosionInstance = Instantiate(ExplosionVfx, transform.position, transform.rotation);
		Destroy(explosionInstance, MaxSfxLifeTime);

		Destroy(gameObject);

		GameObject.Find("LevelManager").GetComponent<LevelManager>().LoadLevel("The End", MaxSfxLifeTime);
	}

	// Use this for initialization
	void Awake ()
	{
		_chargeVfx = Instantiate(ChargeVfx, transform.position, transform.rotation);
		_chargeVfx.transform.parent = transform;
		_chargeParticleSys = _chargeVfx.GetComponent<ParticleSystem>();

		_dischargeVfx = Instantiate(DisChargeVfx, transform.position, transform.rotation);
		_dischargeVfx.transform.parent = transform;
		_dischargeParticleSys = _dischargeVfx.GetComponent<ParticleSystem>();
	}

	void Start()
	{
		// Get the right most and left most of camera and use them as restrict movement inside camera view
		var distance = transform.position.z - Camera.main.transform.position.z;
		var leftMost = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
		var rightMost = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance));

		_maxX = rightMost.x - Padding;
		_minX = leftMost.x + Padding;

		HpTracker.PostHp(Hp);
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

	// Update is called once per frame
	void Update () {

		// Input handling
		// Movement
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			transform.position += Vector3.left * Time.deltaTime * HorizontalSpeed;
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			transform.position += Vector3.right * Time.deltaTime * HorizontalSpeed;
		}

		// Firing
		if (Input.GetKey(KeyCode.Space))
		{
			Charge();
		} else if (Input.GetKeyUp(KeyCode.Space))
		{
			Fire();
		}

		// Restrict movement to inside the scene
		transform.position = new Vector3(Mathf.Clamp(transform.position.x, _minX, _maxX), transform.position.y);
	}
}
