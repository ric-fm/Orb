using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorButton : Sensor
{
	public void Push()
	{
		ToggleSignal();
	}
}
