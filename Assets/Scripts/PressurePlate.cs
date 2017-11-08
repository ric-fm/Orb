using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : Sensor {

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			SetSignal(true);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			SetSignal(false);
		}
	}
}
