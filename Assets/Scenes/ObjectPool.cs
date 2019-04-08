using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes
{
	/// <summary>
	/// time:2019/4/8
	/// author:Sun
	/// des:ObjectPool
	/// </summary>
	public class ObjectPool<T>
	{
		/// <summary>
		/// 未使用对象列表
		/// </summary>
		private List<ObjectPoolContainer<T>> _unUseList;
		/// <summary>
		/// 正在使用的对象字典
		/// </summary>
		private Dictionary<T, ObjectPoolContainer<T>> _userDict;
		/// <summary>
		/// 返回指定对象
		/// </summary>
		private Func<T> _returnT;
		/// <summary>
		/// 刷新清理时间 负数不清理
		/// </summary>
		public float RefreshTime = -1;
		/// <summary>
		/// 默认保持池子内数量 负数不保持
		/// </summary>
		public int HoldObject = -1;
		
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="returnT"></param> 目标对象
		/// <param name="initSize"></param> 池子内常驻数
		/// <param name="refreshTime"></param> 刷新时间
		public ObjectPool(Func<T> returnT)
		{
//			_returnT = returnT;
//			initSize = initSize < 0 ? 0 : initSize;
			_unUseList = new List<ObjectPoolContainer<T>>();
			_userDict = new Dictionary<T, ObjectPoolContainer<T>>();
//			CreateSpecifiedContainer(initSize);
			//设置刷新时间
			
		}

		/// <summary>
		/// 创建指定数量的对象容器
		/// </summary>
		/// <param name="initSize"></param>
		private void CreateSpecifiedContainer(int initSize)
		{
			for (int i = 0; i < initSize; i++)
			{
				CreateContainer();
			}
		}

		/// <summary>
		/// 创建对象容器
		/// </summary>
		/// <param name="initSize"></param>
		private ObjectPoolContainer<T> CreateContainer()
		{
			var container = new ObjectPoolContainer<T>();
			container.Item = _returnT();
			_unUseList.Add(container);
			return container;
		}

		/// <summary>
		/// 从池子中取出对象
		/// </summary>
		/// <returns></returns>
		public T Get()
		{
			ObjectPoolContainer<T> container = null;
			if (_unUseList.Count>0)
			{
				container = _unUseList[0];
			}
			else
			{
				container = CreateContainer();
			}
			
			container.Consume();
			_unUseList.Remove(container);
			_userDict.Add(container.Item,container);
			return container.Item;
		}

		/// <summary>
		/// 释放对象
		/// </summary>
		/// <param name="item"></param>
		public void Release(T item)
		{
			if (_userDict.ContainsKey(item))
			{
				var container = _userDict[item];
				container.Release();
				_userDict.Remove(item);
				_unUseList.Add(container);
			}
			else
			{
				Debug.LogWarning("这池子中不存在包含 "+item +"的容器！");
			}
		}

//		IEnumerator RefreshObjectPool(float refreshTime)
//		{
//			while (true)
//			{
//				
//				yield return new WaitForSeconds(refreshTime);
//			}
//		}
	}
}
