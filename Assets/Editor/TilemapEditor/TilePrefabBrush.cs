using System;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Tilemaps;
using Object = UnityEngine.Object;
#endif

[CreateAssetMenu(fileName = "Tile Prefab brush", menuName = "Brushes/Tile Prefab brush")]
[CustomGridBrush(false, true, false, "Tile Prefab Brush")]
public class TilePrefabBrush : GridBrushBase
{
	public GameObject m_Prefabs;
	public GameObject ColliderPrefab;
	public Sprite selectedSprite;
	public Vector3 m_Anchor = new Vector3(0.5f, 0.5f, 0.0f);
	public bool IsTrigger = false;
	public int width, height;

	private GameObject prev_brushTarget;
	private Vector3Int prev_position = Vector3Int.one * Int32.MaxValue;
	private GameObject selectedTilemap;

	public override void Select(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
	{
		string name = "Tile(" + position.x.ToString() + " | " + position.y.ToString() + ")";

		if (selectedTilemap)
			brushTarget = selectedTilemap;

		var InhierarchyObject = brushTarget.transform.Find(name);
		if (InhierarchyObject == null)
		{
			base.Select(gridLayout, brushTarget, position);
		}
		else
		{
			selectedTilemap = brushTarget;
			Selection.activeGameObject = InhierarchyObject.gameObject;
		}
	}

	public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pivot)
	{
		base.Pick(gridLayout, brushTarget, position, pivot);

		Tilemap palette = brushTarget.GetComponent<Tilemap>();
		if (palette == null)
			return;
		Vector3Int palettePosition = position.position;
		TileBase tile = palette.GetTile(palettePosition);
		if (tile != null)
		{
			selectedSprite = ((Tile)tile).sprite;
		}
	}

	public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
	{
		if (position == prev_position)
		{
			return;
		}
		prev_position = position;
		if (brushTarget)
		{
			prev_brushTarget = brushTarget;
		}
		brushTarget = prev_brushTarget;

		// Do not allow editing palettes
		if (brushTarget.layer == 31)
			return;

		GameObject prefab;
		if (IsTrigger)
			prefab = ColliderPrefab;
		else
			prefab = m_Prefabs;

		GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
		string name = "Tile(" + position.x.ToString() + " | " + position.y.ToString() + ")";
		if (instance != null)
		{
			Undo.MoveGameObjectToScene(instance, brushTarget.scene, "Paint Prefabs");
			Undo.RegisterCreatedObjectUndo((Object)instance, "Paint Prefabs");
			var existObject = brushTarget.transform.Find(name);
			if (existObject != null)
			{
				DestroyImmediate(existObject.gameObject);
			}

			instance.name = name;
			instance.GetComponent<SpriteRenderer>().sprite = selectedSprite;
			instance.transform.SetParent(brushTarget.transform);
			instance.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(position + m_Anchor));
		}
	}

	public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
	{
		if (brushTarget)
		{
			prev_brushTarget = brushTarget;
		}
		brushTarget = prev_brushTarget;
		// Do not allow editing palettes
		if (brushTarget.layer == 31)
			return;

		Transform erased = GetObjectInCell(grid, brushTarget.transform, position);
		if (erased != null)
			Undo.DestroyObjectImmediate(erased.gameObject);
	}

	private static Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
	{
		int childCount = parent.childCount;
		Vector3 min = grid.LocalToWorld(grid.CellToLocalInterpolated(position));
		Vector3 max = grid.LocalToWorld(grid.CellToLocalInterpolated(position + Vector3Int.one));
		Bounds bounds = new Bounds((max + min) * .5f, max - min);

		for (int i = 0; i < childCount; i++)
		{
			Transform child = parent.GetChild(i);
			if (bounds.Contains(child.position))
				return child;
		}
		return null;
	}
}

/// <summary>
/// The Brush Editor for a Prefab Brush.
/// </summary>
[CustomEditor(typeof(PrefabBrush))]
public class TilePrefabBrushEditor : GridBrushEditor
{
	private PrefabBrush prefabBrush { get { return target as PrefabBrush; } }

	private SerializedProperty m_Prefabs;
	private SerializedProperty selectedSprite;
	private SerializedProperty m_Anchor;
	private SerializedObject m_SerializedObject;

	protected override void OnEnable()
	{
		base.OnEnable();
		m_SerializedObject = new SerializedObject(target);
		m_Prefabs = m_SerializedObject.FindProperty("m_Prefabs");
		selectedSprite = m_SerializedObject.FindProperty("selectedSprite");
		m_Anchor = m_SerializedObject.FindProperty("m_Anchor");
	}

	/// <summary>
	/// Callback for painting the inspector GUI for the PrefabBrush in the Tile Palette.
	/// The PrefabBrush Editor overrides this to have a custom inspector for this Brush.
	/// </summary>
	public override void OnPaintInspectorGUI()
	{
		m_SerializedObject.UpdateIfRequiredOrScript();
		EditorGUILayout.PropertyField(m_Prefabs);
		EditorGUILayout.PropertyField(selectedSprite);
		EditorGUILayout.PropertyField(m_Anchor);
		m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();
	}
}