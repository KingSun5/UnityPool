using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		public List<ObjectPoolContainer<T>> UnUseList;
		/// <summary>
		/// 正在使用的对象字典
		/// </summary>
		public Dictionary<T, ObjectPoolContainer<T>> UserDict;
		/// <summary>
		/// 返回指定对象
		/// </summary>
		private Func<T> _returnT;
		/// <summary>
		/// 默认保持池子内数量 负数不保持
		/// </summary>
		public int HoldObject = -1;
		/// <summary>
		/// 池子Name
		/// </summary>
		public string PoolName = "";
		
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="returnT"></param> 目标对象
		/// <param name="initSize"></param> 池子内常驻数
		/// <param name="refreshTime"></param> 刷新时间
		public ObjectPool(Func<T> returnT)
		{
			_returnT = returnT;
//			initSize = initSize < 0 ? 0 : initSize;
			UnUseList = new List<ObjectPoolContainer<T>>();
			UserDict = new Dictionary<T, ObjectPoolContainer<T>>();
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
			UnUseList.Add(container);
			return container;
		}

		/// <summary>
		/// 从池子中取出对象
		/// </summary>
		/// <returns></returns>
		public T Get()
		{
			ObjectPoolContainer<T> container = null;
			if (UnUseList.Count>0)
			{
				container = UnUseList[0];
			}
			else
			{
				container = CreateContainer();
			}
			
			container.Consume();
			UnUseList.Remove(container);
			UserDict.Add(container.Item,container);
			return container.Item;
		}

		/// <summary>
		/// 释放对象
		/// </summary>
		/// <param name="item"></param>
		public void Release(T item)
		{
			if (UserDict.ContainsKey(item))
			{
				var container = UserDict[item];
				container.Release();
				UserDict.Remove(item);
				UnUseList.Add(container);
			}
			else
			{
				Debug.LogWarning("这池子中不存在包含 "+item +"的容器！");
			}
		}
	}
}
