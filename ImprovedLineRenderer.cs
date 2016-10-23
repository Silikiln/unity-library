using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ImprovedLineRenderer : MonoBehaviour
{
    [SerializeField]
    float _lineWeight = .3f;
    public float LineWeight
    {
        get { return _lineWeight; }
        set { _lineWeight = value; GenerateVertices(); }
    }

    private bool _delayMeshUpdate = false;
    public bool DelayMeshUpdate
    {
        get { return _delayMeshUpdate; }
        set
        {
            _delayMeshUpdate = value;
            if (!_delayMeshUpdate)
                UpdateMesh();
        }
    }
    public LineColor LineColor;

    private const float PIOver2 = Mathf.PI / 2;

    private List<Vector3> points = new List<Vector3>();
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    private bool firstUpdate = true;

    void Start()
    {
        GetComponent<MeshFilter>().mesh = new Mesh();
    }

    void Update()
    {
        if (!firstUpdate) return;

        firstUpdate = false;
        UpdateMesh();
    }

    void UpdateMesh()
    {
        if (DelayMeshUpdate) return;

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        if (points.Count < 2) return;

        while (vertices.Count > (points.Count - 1) * 4)
            vertices.RemoveAt(vertices.Count - 1);

        while (triangles.Count > (points.Count - 1) * 6)
            triangles.RemoveAt(triangles.Count - 1);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        UpdateColors();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    void UpdateColors()
    {
        if (LineColor == null || points.Count < 2) return;
        GetComponent<MeshFilter>().mesh.colors = LineColor.GetLineColors(points);
    }

    public void AddPoint(Vector3 point)
    {
        point.z = transform.position.z;
        points.Add(transform.InverseTransformPoint(point));
        if (points.Count == 1) return;
        int i;
        for (i = 0; i < 4; i++) vertices.Add(Vector3.zero);

        i = (points.Count - 2) * 4;
        triangles.AddRange(new int[] {
            i, i + 1, i + 2,
            i + 2, i + 3, i + 1
        });

        GenerateVertices(points.Count - 1);
    }

    public void SetPoint(int index, Vector3 point)
    {
        points[index] = point;
        GenerateVertices(index);
    }

    public void RemovePoint(int index)
    {
        points.RemoveAt(index);
        GenerateVertices(index);
    }

    public void Clear()
    {
        points.Clear();
        UpdateMesh();
    }

    public void SetColor(LineColor colorHandler)
    {
        this.LineColor = colorHandler;
        UpdateColors();
    }

    void GenerateVertices()
    {
        for (int i = 1; i < Count; i += 2)
            GenerateVertices(i);
    }

    void GenerateVertices(int pointIndex)
    {
        if (pointIndex > 0 && pointIndex < points.Count)
            GenerateVertices(pointIndex - 1, pointIndex);
        if (pointIndex < points.Count - 1)
            GenerateVertices(pointIndex, pointIndex + 1);
        UpdateMesh();
    }

    void GenerateVertices(int aIndex, int bIndex)
    {
        Vector3 a = points[aIndex], b = points[bIndex];
        float angleBetween = AngleBetween(a, b);
        SetVertex(a, aIndex * 4, angleBetween);
        SetVertex(b, aIndex * 4 + 2, angleBetween);
    }

    static float AngleBetween(Vector3 from, Vector3 to)
    {
        Vector3 result = from - to;
        float angle = Mathf.Atan2(result.y, result.x);
        return angle;
    }

    void SetVertex(Vector3 point, int vertexIndex, float angleOffset)
    {
        angleOffset += PIOver2;
        vertices[vertexIndex] = new Vector3(point.x + Mathf.Cos(angleOffset) * LineWeight, point.y + Mathf.Sin(angleOffset) * LineWeight, point.z);
        angleOffset -= Mathf.PI;
        vertices[vertexIndex + 1] = new Vector3(point.x + Mathf.Cos(angleOffset) * LineWeight, point.y + Mathf.Sin(angleOffset) * LineWeight, point.z);
    }

    public int Count { get { return points.Count; } }
}
