using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Editor
{
	/// <summary>
	/// 자동 테스트 데이터 생성을 위한 어트리뷰트들
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class AutoTestDataAttribute : Attribute
	{
		public object[] TestValues { get; }

		public AutoTestDataAttribute(params object[] testValues)
		{
			TestValues = testValues;
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class RandomRangeAttribute : Attribute
	{
		public float Min { get; }
		public float Max { get; }

		public RandomRangeAttribute(float min, float max)
		{
			Min = min;
			Max = max;
		}
	}

	/// <summary>
	/// 자동 테스트 데이터 생성기
	/// </summary>
	public static class AutoTestDataGenerator
	{
		private static readonly Dictionary<Type, Func<object>> DefaultValueGenerators = new()
		{
			{ typeof(string), () => "Test" },
			{ typeof(int), () => UnityEngine.Random.Range(0, 100) },
			{ typeof(float), () => UnityEngine.Random.Range(0f, 100f) },
			{ typeof(bool), () => UnityEngine.Random.value > 0.5f },
			{ typeof(Vector2), () => Vector2.one },
			{ typeof(Vector3), () => Vector3.one },
			{ typeof(Color), () => Color.white },
			{ typeof(Sprite), () => GetDefaultSprite() }
		};

		public static T GenerateTestData<T>() where T : class, new()
		{
			return (T)GenerateTestData(typeof(T));
		}

		public static object GenerateTestData(Type type)
		{
			if (type == null) return null;

			var instance = Activator.CreateInstance(type);
			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

			// 프로퍼티 처리
			foreach (var prop in properties)
			{
				if (!prop.CanWrite) continue;

				var value = GenerateValueForMember(prop.PropertyType, prop);
				if (value != null)
				{
					prop.SetValue(instance, value);
				}
			}

			// 필드 처리
			foreach (var field in fields)
			{
				var value = GenerateValueForMember(field.FieldType, field);
				if (value != null)
				{
					field.SetValue(instance, value);
				}
			}

			return instance;
		}

		private static object GenerateValueForMember(Type memberType, MemberInfo member)
		{
			// AutoTestData 어트리뷰트 체크
			var autoTestAttr = member.GetCustomAttribute<AutoTestDataAttribute>();
			if (autoTestAttr != null && autoTestAttr.TestValues.Length > 0)
			{
				return Convert.ChangeType(
					autoTestAttr.TestValues[UnityEngine.Random.Range(0, autoTestAttr.TestValues.Length)],
					memberType);
			}

			// RandomRange 어트리뷰트 체크
			var randomRangeAttr = member.GetCustomAttribute<RandomRangeAttribute>();
			if (randomRangeAttr != null)
			{
				if (memberType == typeof(int))
					return (int)UnityEngine.Random.Range(randomRangeAttr.Min, randomRangeAttr.Max);
				return UnityEngine.Random.Range(randomRangeAttr.Min, randomRangeAttr.Max);
			}

			// 컬렉션 타입 처리
			if (memberType.IsGenericType)
			{
				if (memberType.GetGenericTypeDefinition() == typeof(List<>))
				{
					var listType = memberType.GetGenericArguments()[0];
					var list = (IList)Activator.CreateInstance(memberType);

					int count = UnityEngine.Random.Range(1, 5);
					for (int i = 0; i < count; i++)
					{
						list.Add(GenerateTestData(listType));
					}

					return list;
				}
			}

			// 기본 타입 처리
			if (DefaultValueGenerators.TryGetValue(memberType, out var generator))
			{
				return generator();
			}

			// 커스텀 클래스 처리
			if (!memberType.IsPrimitive && !memberType.IsEnum)
			{
				return GenerateTestData(memberType);
			}

			return null;
		}

		private static Sprite GetDefaultSprite()
		{
			// 에디터의 기본 스프라이트 반환
			return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
		}
	}
}
