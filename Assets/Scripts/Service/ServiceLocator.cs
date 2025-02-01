using System;
using System.Collections.Generic;

namespace Service
{
	public class ServiceLocator
	{
		private static Dictionary<Type, object> Services { get; } = new();

		public static void RegisterService<T>(T service) where T : class
		{
			Services[typeof(T)] = service;
		}

		public static T GetService<T>() where T : class
		{
			if (Services.TryGetValue(typeof(T), out var service))
			{
				return (T)service;
			}
			throw new Exception($"Service of type {typeof(T)} not registered");
		}

		public static void Clear()
		{
			Services.Clear();
		}
	}
}
