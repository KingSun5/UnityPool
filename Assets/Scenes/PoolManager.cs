
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scenes
{
	/// <summary>
	/// time:2019/4/9
	/// author:Sun
	/// des:对象池管理
	/// </summary>
	public class PoolManager:MonoBehaviour
	{

		private static PoolManager _instance = null;

		public static PoolManager Instance
		{
			get
			{
				if (GameObject.Find("PoolManager")==null)
				{
					var pool = new GameObject("PoolManager").AddComponent<PoolManager>();
					DontDestroyOnLoad(pool);
				}
				_instance = GameObject.Find("PoolManager").GetComponent<PoolManager>();
				return _instance;
			}
		}

		private void Awake()
		{
			StartCoroutine(ClearObjectPool());
			_objectIdDict = new Dictionary<int, ObjectPool<GameObject>>();
			_objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
		}

		/// <summary>
		/// 刷新清理时间 负数不清理
		/// </summary>
		public float RefreshTime = 5;
		/// <summary>
		/// 生成物Id和对应池子
		/// </summary>
		private Dictionary<int, ObjectPool<GameObject>> _objectIdDict;
		/// <summary>
		/// 对象池子
		/// </summary>
		private Dictionary<GameObject, ObjectPool<GameObject>> _objectPools;

		/// <summary>
		/// 拿到目标对象
		/// </summary>
		/// <param name="prefab"></param>
		/// <returns></returns>
		public GameObject Get(GameObject prefab)
		{
			GameObject obj = GetPool(prefab).Get();
			if (!_objectIdDict.ContainsKey(obj.GetInstanceID()))
			{
				_objectIdDict.Add(obj.GetInstanceID(),GetPool(prefab));				
			}
			obj.gameObject.SetActive(true);
			return obj;
		}

		/// <summary>
		/// 拿到对象并设置位置旋转
		/// </summary>
		/// <param name="prefab"></param>
		/// <param name="position"></param>
		/// <param name="rotation"></param>
		/// <param name="root"></param>
		/// <returns></returns>
		public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation,Transform root)
		{
			var obj = Get(prefab);
			obj.transform.position = position;
			obj.transform.rotation = rotation;
			obj.transform.parent = root;
			return obj;
		}

		/// <summary>
		/// 返回物体对应的池子
		/// </summary>
		/// <param name="prefab"></param>
		/// <returns></returns>
		private ObjectPool<GameObject> GetPool(GameObject prefab)
		{
			if (!_objectPools.ContainsKey(prefab))
			{
				var pool = new ObjectPool<GameObject>(() => { return InstantiatePrefab(prefab); });
				_objectPools.Add(prefab,pool);
			}
			return _objectPools[prefab];
		}

		/// <summary>
		/// 释放目标对象
		/// </summary>
		/// <param name="prefab"></param>
		/// <exception cref="Exception"></exception>
		public void Release(GameObject prefab)
		{
			if (!_objectIdDict.ContainsKey(prefab.GetInstanceID()))
			{
				throw new Exception("不存在"+ prefab +"相关对象池");
			}
			prefab.gameObject.SetActive(false);
			_objectIdDict[prefab.GetInstanceID()].Release(prefab);
		}
		
		
		/// <summary>
		/// 获取目标对象实例
		/// </summary>
		/// <param name="prefab"></param>
		/// <returns></returns>
		private GameObject InstantiatePrefab(GameObject prefab)
		{
			var go = Object.Instantiate(prefab);
			return go;
		}

		/// <summary>
		/// 定时清理池子
		/// </summary>
		/// <returns></returns>
		IEnumerator ClearObjectPool()
		{
			while (true)
			{
//				print(111);
				yield return new WaitForSeconds(RefreshTime);
			}
		}

	}
}
