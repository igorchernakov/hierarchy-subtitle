using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[InitializeOnLoad]
class HierarchySubtitle
{
	static GUIStyle style;

	const float margin = 3;

	static Dictionary<System.Type, string> types = new Dictionary<System.Type, string> {
		{ typeof(LayoutGroup), "Panel" },
		{ typeof(Button), "Button" },
		{ typeof(Text), "Text" },
		{ typeof(TMP_Text), "TextMesh Pro" },
		{ typeof(Image), "Image" },
		{ typeof(MeshRenderer), "Mesh" },
		{ typeof(SpriteRenderer), "Sprite" },
		{ typeof(TextMesh), "3D Text" },
		{ typeof(ScrollRect), "Scroll View" },
	};

	static HierarchySubtitle ()
	{
		EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyItem;
	}

	private const string propertyName = "EnableHierarchySubtitle";

	[MenuItem ("Edit/Toggle Hierarchy Subtitle")]
	static void DoSomething () 
	{
		Enabled = !Enabled;
	}

	static bool Enabled {
		get {
			return !EditorPrefs.HasKey(propertyName) || EditorPrefs.GetBool(propertyName);
		}
		set {
			EditorPrefs.SetBool(propertyName, value);
		}
	}

	static void DrawHierarchyItem (int instanceID, Rect selectionRect)
	{
		if (!Enabled)
			return;
			
		if (style == null)
			GenerateStyles();

		GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

		if (!gameObject)
			return;

		selectionRect.x += GUI.skin.label.CalcSize(new GUIContent(gameObject.name)).x;
		selectionRect.y += margin;

		foreach (var label in CollectLabels(gameObject)) {
			selectionRect.width = style.CalcSize(new GUIContent(label)).x;
			GUI.Label(selectionRect, label, style);
			selectionRect.x += selectionRect.width;
		}
	}

	static IEnumerable<string> CollectLabels(GameObject gameObject)
	{
		foreach (var cell in gameObject.GetComponents<IEditorDescription>()) {
			if (string.IsNullOrEmpty(cell.GetEditorDescription()))
				continue;
			yield return cell.GetEditorDescription();
		}

		foreach (var type in types) {
			if (gameObject.GetComponent(type.Key)) {
				yield return type.Value;
				break;
			}
		}
	}

	static void GenerateStyles ()
	{
		//style = new GUIStyle(GUI.skin.GetStyle("AssetLabel"));
		style = new GUIStyle(GUI.skin.GetStyle("ShurikenValue"));
	}
}
