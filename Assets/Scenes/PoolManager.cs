
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
		

		/// <summary>
		/// 刷新清理时间 负数不清理
		/// </summary>
		public float RefreshTime = 15;
		/// <summary>
		/// 对象池挂载
		/// </summary>
		private Transform _poolTransform;
		/// <summary>
		/// 生成物Id和对应池子
		/// </summary>
		private Dictionary<int, ObjectPool<GameObject>> _objectIdDict;
		/// <summary>
		/// 对象池子
		/// </summary>
		private Dictionary<GameObject, ObjectPool<GameObject>> _objectPools;
		/// <summary>
		/// 不活跃物体存放地点
		/// </summary>
		private Dictionary<string, GameObject> _unActiveTransform;
		
		private void Awake()
		{
			StartCoroutine(ClearObjectPool());
			_objectIdDict = new Dictionary<int, ObjectPool<GameObject>>();
			_objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
			_unActiveTransform = new Dictionary<string, GameObject>();
			_poolTransform = GameObject.Find("PoolManager").transform;
		}

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
				pool.PoolName = prefab.name;
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
			//对象池
			var value = _objectIdDict[prefab.GetInstanceID()];
			//不活跃物体父节点
			GameObject root = null;
			if (!_unActiveTransform.ContainsKey(value.PoolName))
			{
				root = new GameObject(value.PoolName);
				root.transform.SetParent(_poolTransform);
				_unActiveTransform.Add(value.PoolName,root);
			}
			root = _unActiveTransform[value.PoolName];
			prefab.gameObject.SetActive(false);
			prefab.transform.SetParent(root.transform);
			value.Release(prefab);
			_objectIdDict.Remove(prefab.GetInstanceID());
		}
		
		
		/// <summary>
		/// 获取目标对象实例
		/// </summary>
		/// <param name="prefab"></param>
		/// <returns></returns>
		private GameObject InstantiatePrefab(GameObject prefab)
		{
			var go = Instantiate(prefab);
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
//				if (_objectPools==null)
//				{
//					Debug.Log("不存在任何池子");
//					continue;
//				}
//				foreach (KeyValuePair<GameObject,ObjectPool<GameObject>> pair in _objectPools)
//				{
//					if (pair.Value.UserDict.Count==0)
//					{
//						Debug.Log("该池子不活跃删除该池子");
//						_objectPools.Remove(pair.Key);
//						var root = _unActiveTransform[pair.Value.PoolName];
//						if (root)
//						{
//							_unActiveTransform.Remove(pair.Value.PoolName);
//							Destroy(root);
//						}
//					}
//				}
				Debug.Log("刷新了一下池子");
				yield return new WaitForSeconds(RefreshTime);
			}
		}

	}
}
