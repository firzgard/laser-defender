using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public int Dmg = 10;
	public float Spd = 8;

	public void Hit()
	{
		Destroy(gameObject);
	}

	public void Fire()
	{
		GetComponent<Rigidbody2D>().velocity = transform.up * Spd;
	}
}
