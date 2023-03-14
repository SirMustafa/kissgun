#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SolidArc
{
    public Vector3 center;
    public Vector3 normal;
    public Vector3 from;
    public float angle;
    public float radius;
    public Color color;

    public SolidArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, Color color)
    {
        this.center = center;
        this.normal = normal;
        this.from = from;
        this.angle = angle;
        this.radius = radius;
        this.color = color;
    }
}

public static class DrawSolidArc
{

    private static List<SolidArc> arcs = new List<SolidArc>();
    private static void OnSceneGUI() {
        foreach (SolidArc arc in arcs)
        {
            Handles.color = arc.color;
            Handles.DrawSolidArc(arc.center,
                    arc.normal,
                    arc.from,
                    arc.angle,
                    arc.radius);
        }
    }

    public static void DrawArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, Color color)
    {
        SolidArc arc = null;
        arc = arcs.Find(x => x.center == center 
                        && x.normal == normal
                        && x.from == from
                        && x.angle == angle
                        && x.radius == radius
                        && x.color == color);

        if (arc == null)
        {
            arc = new SolidArc(center, normal, from, angle, radius, color);
            arcs.Add(arc);
        }
    }
}

#endif