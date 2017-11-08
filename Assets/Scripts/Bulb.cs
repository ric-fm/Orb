using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bulb : Activator
{
	public Light light;

	public override void OnStateChange(bool isOn)
	{
		light.enabled = isOn;
	}
}
