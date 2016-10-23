using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class LineColor
{
    public enum ColorStyle { Solid, Gradient }

    public ColorStyle ColorType = ColorStyle.Solid;
    public Color[] Colors = { Color.white };

    public Color[] GetLineColors(List<Vector3> points)
    {
        switch (ColorType)
        {
            case ColorStyle.Gradient:
                return GradientLineColors(points);
            default:
                return SolidLineColors(points);
        }
    }

    private Color[] SolidLineColors(List<Vector3> points)
    {
        Color[] arr = new Color[(points.Count - 1) * 4];
        for (int i = 0; i < arr.Length; i++) arr[i] = Colors[0];
        return arr;
    }

    private Color[] GradientLineColors(List<Vector3> points)
    {
        float totalDistance = 0;
        float[] pointDistance = new float[points.Count];
        Vector3 currentPoint = points[0];
        for (int i = 1; i < points.Count; i++)
        {
            totalDistance += Vector3.Distance(currentPoint, points[i]);
            pointDistance[i] = totalDistance;
            currentPoint = points[i];
        }

        Color[] arr = new Color[(points.Count - 1) * 4];
        arr[0] = arr[1] = Colors[0];
        for (int i = 2; i < arr.Length - 2; i += 4)
        {
            for (int x = 0; x < 4; x++)
                arr[i + x] = Color.Lerp(Colors[0], Colors[1], pointDistance[(i + 2) / 4] / totalDistance);
        }

        arr[arr.Length - 2] = arr[arr.Length - 1] = Colors[1];
        return arr;
    }
}

