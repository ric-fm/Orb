using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowIntensityTest : MonoBehaviour {

	public Renderer rend;
	void Start()
	{
		rend = GetComponents<Renderer>()[0];
		rend.materials[1].shader = Shader.Find("Custom/Glow");
		
	}
	void Update()
	{
		float intensity = Mathf.PingPong(Time.time, 3.0F);
		rend.materials[1].SetFloat("_Glow", intensity);
	}
}
