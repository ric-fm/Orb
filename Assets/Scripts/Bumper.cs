using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour {

	public Vector3 direction;
	public float aceleration;

	Vector3 dir;

	// Use this for initialization
	void Start () {
		dir = transform.InverseTransformDirection(direction);
	}

	// Update is called once per frame
	void Update () {
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			other.GetComponent<PlayerController>().Push(dir * aceleration);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(transform.position, transform.position + direction);
	}
}
