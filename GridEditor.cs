using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

[CustomEditor(typeof(Grid)), CanEditMultipleObjects]
public class GridEditor : Editor {
    Grid grid;
    bool editGrid;

    public void OnEnable()
    {
        grid = target as Grid;
        editGrid = false;
    }

    public void OnDisable() { }

    public virtual void OnMouseDown(Vector3 worldPosition) { grid.SetChildFromWorldPosition(worldPosition); }
    public virtual void OnMouseDrag(Vector3 worldPosition) { OnMouseDown(worldPosition); }

    public virtual void OnScrollUp() { if (grid.CurrentLevel < grid.Depth - 1) grid.CurrentLevel++; }

    public virtual void OnScrollDown() { if (grid.CurrentLevel > 0) grid.CurrentLevel--; }

    public virtual void OnKeyDown() { }

    public virtual void OnKeyUp()
    {
        int keyCodeValue = (int)Event.current.keyCode;
        if (keyCodeValue >= (int)KeyCode.Alpha0 && keyCodeValue <= (int)KeyCode.Alpha9)
        {
            keyCodeValue -= (int)KeyCode.Alpha0;
            if (--keyCodeValue < grid.basePrefabs.Length && keyCodeValue >= 0)
                grid.prefabToUse = grid.basePrefabs[keyCodeValue]; 
            else
                grid.prefabToUse = null;
        }
    }

    public void OnSceneGUI()
    {
        if (Application.isPlaying || !editGrid) return;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        EditorUtility.SetSelectedWireframeHidden(grid.GetComponent<Renderer>(), false);

        switch(Event.current.type)
        {
            case EventType.KeyDown:
                OnKeyDown();
                Event.current.Use();
                break;
            case EventType.KeyUp:
                OnKeyUp();
                Event.current.Use();
                break;
            case EventType.ScrollWheel:
                if (Event.current.control)
                {
                    if (Event.current.delta.y < 0)
                        OnScrollUp();
                    else
                        OnScrollDown();
                    Event.current.Use();
                    Repaint();
                }
                break;
            case EventType.MouseDown:
            case EventType.MouseDrag:
                if (Event.current.button != 0) break;
                Camera editorCamera = SceneView.currentDrawingSceneView.camera;
                Ray mouseRay = editorCamera.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, Event.current.mousePosition.y, editorCamera.nearClipPlane));
                mouseRay.direction = Vector3.Reflect(mouseRay.direction, editorCamera.transform.up.normalized);
                float distance;
                if (grid.Plane.Raycast(mouseRay, out distance))
                {
                    Vector3 worldPosition = mouseRay.GetPoint(distance);
                    if (Event.current.type == EventType.MouseDown)
                        OnMouseDown(worldPosition);

                    if (Event.current.type == EventType.MouseDrag)
                        OnMouseDrag(worldPosition);

                    Event.current.Use();
                }
                break;
        }

        if (!Selection.activeGameObject)
            Selection.activeGameObject = grid.gameObject;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        bool oldEdit = editGrid;
        editGrid = EditorGUILayout.Toggle("Edit Mode", editGrid);
        if (editGrid != oldEdit)
        {
            Tools.hidden = editGrid;
            grid.SetAllLevelActive(!editGrid);
            if (editGrid)
                grid.SetLevelActive(grid.CurrentLevel, true);
        }
        if (GUILayout.Button("Re-Initialize Grid"))
            grid.GenerateGrid();
    }
}
