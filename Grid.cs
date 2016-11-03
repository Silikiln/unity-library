using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Grid : MonoBehaviour
{
    public float X_DIFF = 1;
    public float Y_DIFF = 1;
    public float Z_DIFF = 1;

    public int Width = 200;
    public int Height = 200;
    public int Depth = 1;

    public int Left { get { return -Width / 2; } }
    public int Right { get { return Width / 2 + Width % 2; } }
    public int Bottom { get { return -Height / 2; } }
    public int Top { get { return Height / 2 + Height % 2; } }
    public int Floor { get { return -Depth / 2; } }
    public int Ceiling { get { return Depth / 2 + Depth % 2; } }

    [SerializeField]
    private int _currentLevel = 0;
    public int CurrentLevel
    {
        get { return _currentLevel; }
        set
        {
            if (value >= 0 && value < Depth && value != _currentLevel)
            {
                SetLevelActive(_currentLevel, false);
                _currentLevel = value;
                SetLevelActive(_currentLevel, true);
            }
        }
    }
    public virtual Plane Plane { get { return new Plane() { normal = Vector3.up, distance = (-CurrentLevel * Z_DIFF + transform.position.y) }; } }

    Dictionary<int, GameObject>[] _gridObjects;
    protected Dictionary<int, GameObject>[] GridObjects
    {
        get
        {
            if (_gridObjects == null)
                GenerateGrid();
            return _gridObjects;
        }
    }

    public GameObject[] basePrefabs;
    public GameObject prefabToUse;

    void Start() { Debug.Log("Starting"); }

    public bool PositionInGrid(Vector3 worldPos) { return SlotInGrid(WorldToLocalSlot(worldPos)); }
    public bool SlotInGrid(Vector3 slotPos) { return SlotInGrid((int)slotPos.x, (int)slotPos.y, (int)slotPos.z); }
    public virtual bool SlotInGrid(int x, int y, int z)
    {
        return x >= Left    && x < Right
            && z >= Bottom  && z < Top
            && y >= Floor   && y < Ceiling;
    }

    public int ChildHashCode(Vector3 xyz) { return ChildHashCode((int)xyz.x, (int)xyz.y, (int)xyz.z); }
    public virtual int ChildHashCode(int x, int y, int z) { return (x - Left) + (z - Bottom) * Width; }

    public GameObject this[Vector3 xyz] { get { return this[(int)xyz.x, (int)xyz.y, (int)xyz.z]; } }
    public GameObject this[int x, int y, int z] {
        get
        {
            if (!SlotInGrid(x, y, z)) return null;
            return GridObjects[y][ChildHashCode(x, y, z)];
        }
        protected set
        {
            if (!SlotInGrid(x, y, z)) return;
            int hashCode = ChildHashCode(x, y, z);
            if (GridObjects[y].ContainsKey(hashCode))
                DestroyImmediate(GridObjects[y][hashCode]);
            GridObjects[y][ChildHashCode(x, y, z)] = value;
        }
    }
    
    public bool TryGetGridObject(Vector3 xyz, out GameObject found) { return TryGetGridObject((int)xyz.x, (int)xyz.y, (int)xyz.z, out found); }
    public bool TryGetGridObject(int x, int y, int z, out GameObject found)
    {
        found = this[x, y, z];
        if (found != null) return true;
        return false;
    }

    public GameObject SetChildFromWorldPosition(Vector3 worldPos) { return SetChild(WorldToLocalSlot(worldPos)); }
    public GameObject SetChild(Vector3 xyz) { return SetChild((int)xyz.x, (int)xyz.y, (int)xyz.z); }
    public virtual GameObject SetChild(int x, int y, int z)
    {
        if (SlotInGrid(x, y, z))
            return this[x, y, z] = Instantiate(x, y, z);
        return null;
    }

    protected virtual GameObject Instantiate(int x, int y, int z)
    {
        if (prefabToUse == null) return null;
        return (GameObject) Instantiate(prefabToUse, LocalPosition(x, y, z) + prefabToUse.transform.position, prefabToUse.transform.rotation, transform);
    }

    public virtual Vector3 WorldToLocalSlot(Vector3 worldPosition)
    {
        worldPosition -= transform.position;
        Vector3 result = new Vector3(
            Mathf.Round(worldPosition.x / (X_DIFF * transform.lossyScale.x)),
            Mathf.Round(worldPosition.y / (Y_DIFF * transform.lossyScale.y)),
            Mathf.Round(worldPosition.z / (Z_DIFF * transform.lossyScale.z))
        );
        return result;
    }

    public virtual Vector3 ScaledPosition(int x, int y, int z)
    {
        Vector3 scaledPosition = LocalPosition(x, y, z);
        scaledPosition.Scale(transform.localScale);
        return transform.position + scaledPosition;
    }

    public virtual Vector3 LocalPosition(int x, int y, int z)
    {
        return new Vector3(x * X_DIFF, y * Y_DIFF, z * Z_DIFF);
    }

    public void SetAllLevelActive(bool active) { for (int i = 0; i < Depth; i++) SetLevelActive(i, active); }
    public virtual void SetLevelActive(int level, bool active)
    {
        foreach (KeyValuePair<int, GameObject> kv in GridObjects[level])
        {
            if (kv.Value == null) continue;
            kv.Value.gameObject.SetActive(active);
        }
    }

    public virtual void GenerateGrid()
    {
        ClearGrid();

        _gridObjects = new Dictionary<int, GameObject>[Depth];
        for (int i = 0; i < Depth; i++) _gridObjects[i] = new Dictionary<int, GameObject>();
        foreach (Transform t in transform)
            if (PositionInGrid(t.position))
            {
                Vector3 localSlot = WorldToLocalSlot(t.position);
                _gridObjects[(int)localSlot.y].Add(ChildHashCode(localSlot), t.gameObject);
            }
    }

    protected virtual void ClearGrid()
    {
        if (_gridObjects != null)
            foreach (Dictionary<int, GameObject> dict in _gridObjects)
                if (dict != null) dict.Clear();
    }
}
