using System.Collections.Generic;
using System.Linq;
using UI.Data;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Editor
{
	/// <summary>
	/// 에디터 확장 - UI 프리팹 관리 창
	/// </summary>
	public class UIPrefabManagerWindow : EditorWindow
	{
		private UIPrefabData prefabData;
		private Vector2 scrollPosition;
		private readonly string[] tabNames = { "Prefab List", "Settings" };
		private int selectedTab;
		private SerializedObject serializedObject;
		private SerializedProperty prefabInfosProperty;
		private bool isAddressablesInit;
		private AddressableAssetSettings settings;
		private GUIStyle headerStyle;
		private Color defaultColor;

		[MenuItem("UILabs/UI Prefab Manager")]
		public static void ShowWindow()
		{
			var window = GetWindow<UIPrefabManagerWindow>("UI Prefab Manager");
			window.minSize = new Vector2(450, 600);
		}

		private void OnEnable()
		{
			LoadPrefabData();
			InitializeAddressables();
			InitializeStyles();
		}

		private void InitializeStyles()
		{
			headerStyle = new GUIStyle(UnityEditor.EditorStyles.boldLabel)
			{
				fontSize = 13,
				padding = new RectOffset(5, 5, 5, 5)
			};
			defaultColor = GUI.backgroundColor;
		}

		private void InitializeAddressables()
		{
			settings = AddressableAssetSettingsDefaultObject.Settings;
			isAddressablesInit = settings != null;
		}

		private void LoadPrefabData()
		{
			// Resources 폴더에서 UIPrefabData 찾기
			string[] guids = AssetDatabase.FindAssets("t:UIPrefabData");
			if (guids.Length > 0)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[0]);
				prefabData = AssetDatabase.LoadAssetAtPath<UIPrefabData>(path);
			}

			if (prefabData == null)
			{
				CreateNewPrefabData();
			}

			serializedObject = new SerializedObject(prefabData);
			prefabInfosProperty = serializedObject.FindProperty("prefabInfos");
		}

		private void CreateNewPrefabData()
		{
			// Resources 폴더 확인 및 생성
			if (!AssetDatabase.IsValidFolder("Assets/Resources"))
			{
				AssetDatabase.CreateFolder("Assets", "Resources");
			}

			prefabData = CreateInstance<UIPrefabData>();
			AssetDatabase.CreateAsset(prefabData, "Assets/Resources/UIPrefabData.asset");
			AssetDatabase.SaveAssets();
		}

		private void OnGUI()
		{
			if (!ValidateSetup()) return;

			serializedObject.Update();

			DrawToolbar();

			switch (selectedTab)
			{
				case 0:
					DrawPrefabList();
					break;
				case 1:
					DrawSettings();
					break;
			}

			serializedObject.ApplyModifiedProperties();
		}

		private bool ValidateSetup()
		{
			if (prefabData == null)
			{
				EditorGUILayout.HelpBox("UIPrefabData not found!", MessageType.Error);
				if (GUILayout.Button("Create UIPrefabData"))
				{
					LoadPrefabData();
				}

				return false;
			}

			if (!isAddressablesInit)
			{
				EditorGUILayout.HelpBox("Addressables system is not initialized!", MessageType.Error);
				if (GUILayout.Button("Initialize Addressables"))
				{
					InitializeAddressables();
				}

				return false;
			}

			return true;
		}

		private void DrawToolbar()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			selectedTab = GUILayout.Toolbar(selectedTab, tabNames, EditorStyles.toolbarButton);
			EditorGUILayout.EndHorizontal();
		}

		private void DrawPrefabList()
		{
			EditorGUILayout.BeginVertical();

			// 헤더
			DrawHeader("UI Prefab List");

			// 스크롤 뷰
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

			// 프리팹 리스트
			for (int i = 0; i < prefabInfosProperty.arraySize; i++)
			{
				DrawPrefabInfoElement(i);
			}

			EditorGUILayout.EndScrollView();

			// Add 버튼
			DrawAddButton();

			EditorGUILayout.EndVertical();
		}

		private void DrawHeader(string title)
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(title, headerStyle);
			EditorGUILayout.Space();
		}

		private void DrawPrefabInfoElement(int index)
		{
			var elementProperty = prefabInfosProperty.GetArrayElementAtIndex(index);
			var keyProperty = elementProperty.FindPropertyRelative("Key");
			var prefabReferenceProperty = elementProperty.FindPropertyRelative("PrefabReference");
			var typeProperty = elementProperty.FindPropertyRelative("Type");
			var isCachedProperty = elementProperty.FindPropertyRelative("IsCached");

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			// 요소 헤더
			GUI.backgroundColor = Color.gray;
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			EditorGUILayout.LabelField($"Prefab #{index + 1}: {keyProperty.stringValue}");
			if (GUILayout.Button("Remove", EditorStyles.toolbarButton, GUILayout.Width(60)))
			{
				if (EditorUtility.DisplayDialog("Remove Prefab",
					    "Are you sure you want to remove this prefab?", "Yes", "No"))
				{
					prefabInfosProperty.DeleteArrayElementAtIndex(index);
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
					return;
				}
			}

			EditorGUILayout.EndHorizontal();
			GUI.backgroundColor = defaultColor;

			// 프리팹 정보 필드들
			EditorGUI.indentLevel++;

			EditorGUILayout.PropertyField(keyProperty);
			EditorGUILayout.PropertyField(prefabReferenceProperty);
			EditorGUILayout.PropertyField(typeProperty);
			EditorGUILayout.PropertyField(isCachedProperty);

			EditorGUI.indentLevel--;

			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
		}

		private void DrawAddButton()
		{
			EditorGUILayout.Space();
			if (GUILayout.Button("Add New Prefab", GUILayout.Height(30)))
			{
				AddNewPrefabInfo();
			}

			EditorGUILayout.Space();
		}

		private void AddNewPrefabInfo()
		{
			prefabInfosProperty.InsertArrayElementAtIndex(prefabInfosProperty.arraySize);
			var element = prefabInfosProperty.GetArrayElementAtIndex(prefabInfosProperty.arraySize - 1);

			var keyProperty = element.FindPropertyRelative("Key");
			keyProperty.stringValue = $"NewPrefab_{prefabInfosProperty.arraySize}";

			var typeProperty = element.FindPropertyRelative("Type");
			typeProperty.enumValueIndex = 0;

			var isCachedProperty = element.FindPropertyRelative("IsCached");
			isCachedProperty.boolValue = false;
		}

		private void DrawSettings()
		{
			EditorGUILayout.BeginVertical();

			DrawHeader("Settings");

			EditorGUILayout.HelpBox(
				"Prefab Settings location: " + AssetDatabase.GetAssetPath(prefabData),
				MessageType.Info);

			if (GUILayout.Button("Validate All References"))
			{
				ValidateAllReferences();
			}

			if (GUILayout.Button("Make All Prefabs Addressable"))
			{
				MakeAllPrefabsAddressable();
			}

			EditorGUILayout.Space();
			EditorGUILayout.HelpBox(
				"Total Prefabs: " + prefabInfosProperty.arraySize + "\n" +
				"Cached Prefabs: " + CountCachedPrefabs(),
				MessageType.Info);

			EditorGUILayout.EndVertical();
		}

		private void ValidateAllReferences()
		{
			int invalidCount = 0;
			for (int i = 0; i < prefabInfosProperty.arraySize; i++)
			{
				var element = prefabInfosProperty.GetArrayElementAtIndex(i);
				var prefabReference = element.FindPropertyRelative("PrefabReference");
				var keyProperty = element.FindPropertyRelative("Key");

				if (prefabReference.objectReferenceValue == null)
				{
					Debug.LogWarning($"Invalid prefab reference found for key: {keyProperty.stringValue}");
					invalidCount++;
				}
			}

			EditorUtility.DisplayDialog("Validation Result",
				$"Found {invalidCount} invalid references.", "OK");
		}

		private void MakeAllPrefabsAddressable()
		{
			if (settings == null) return;

			int count = 0;
			for (int i = 0; i < prefabInfosProperty.arraySize; i++)
			{
				var element = prefabInfosProperty.GetArrayElementAtIndex(i);
				var prefabReference = element.FindPropertyRelative("PrefabReference");
				var keyProperty = element.FindPropertyRelative("Key");

				if (prefabReference.objectReferenceValue is GameObject prefab)
				{
					var guid = AssetDatabase.AssetPathToGUID(
						AssetDatabase.GetAssetPath(prefab));
					var entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
					entry.address = keyProperty.stringValue;
					count++;
				}
			}

			settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, null, true);
			EditorUtility.DisplayDialog("Operation Complete",
				$"Made {count} prefabs addressable.", "OK");
		}

		private int CountCachedPrefabs()
		{
			int count = 0;
			for (int i = 0; i < prefabInfosProperty.arraySize; i++)
			{
				var element = prefabInfosProperty.GetArrayElementAtIndex(i);
				var isCached = element.FindPropertyRelative("IsCached");
				if (isCached.boolValue) count++;
			}

			return count;
		}
	}
}
