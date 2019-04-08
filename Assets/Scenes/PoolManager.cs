
using System;
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
	public class PoolManager
	{
		/// <summary>
		/// 对象池
		/// </summary>
		private Dictionary<GameObject, ObjectPool<GameObject>> _objectDict;

		/// <summary>
		/// 拿到目标对象
		/// </summary>
		/// <param name="prefab"></param>
		/// <returns></returns>
		public GameObject Get(GameObject prefab)
		{
			var obj = new GameObject();
			if (!_objectDict.ContainsKey(prefab))
			{
				var pool = new ObjectPool<GameObject>(() => { return InstantiatePrefab(prefab); });
				_objectDict.Add(prefab,pool);
			}
			obj = _objectDict[prefab].Get();
			return obj;
		}

		/// <summary>
		/// 拿到对象并设置位置旋转
		/// </summary>
		/// <param name="prefab"></param>
		/// <param name="position"></param>
		/// <param name="rotation"></param>
		/// <returns></returns>
		public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
		{
			var obj = Get(prefab);
			obj.transform.position = position;
			obj.transform.rotation = rotation;
			return obj;
		}

		/// <summary>
		/// 释放目标对象
		/// </summary>
		/// <param name="prefab"></param>
		/// <exception cref="Exception"></exception>
		public void Release(GameObject prefab)
		{
			if (!_objectDict.ContainsKey(prefab))
			{
				throw new Exception("不存在"+ prefab +"相关对象池");
			}
			_objectDict[prefab].Release(prefab);
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

	}
}
