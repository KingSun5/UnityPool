using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scenes;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

	public GameObject cube;
	public GameObject sphere;

	public Button Btn_One;

	public Button Btn_Two;
	
	private Dictionary<int,int> testDict = new Dictionary<int, int>();

	// Use this for initialization
	void Start () {
		
		Btn_One.onClick.AddListener(() => { PoolManager.Instance.Get(cube); });
		Btn_Two.onClick.AddListener(() => { PoolManager.Instance.Get(sphere); });
		
		
		testDict.Add(1,2);
		testDict.Add(2,3);
		testDict.Add(3,4);
		
		foreach (var item in testDict.ToList()) {
			if (item.Value == 3)
			{
				testDict.Remove(item.Key);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
