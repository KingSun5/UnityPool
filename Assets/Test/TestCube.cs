using System.Collections;
using System.Collections.Generic;
using Scenes;
using UnityEngine;

public class TestCube : MonoBehaviour {

	// Use this for initialization
	private void OnEnable()
	{
		Invoke("Lose",5);
	}

	
	
	// Update is called once per frame
	void Update () {
		
	}

	private void Lose()
	{
		PoolManager.Instance.Release(gameObject);
	}
}
