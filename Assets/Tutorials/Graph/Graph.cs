using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public Transform GraphPointPrefab;
    [Range(0, 200)] public int NumberOfPoints = 10;
    [Range(0f, 20f)] public float Resolution = 2f; //Blocks per unit
    Transform[] points;

    void Awake()
    {
        points = new Transform[NumberOfPoints * (int)Resolution];

        for (int i = 0; i < points.Length; i++)
        {
            Transform GraphPoint = Instantiate(GraphPointPrefab);
            GraphPoint.SetParent(transform, false);
            points[i] = GraphPoint.transform;

            GraphPoint.localScale = Vector3.one * (2f/Resolution);
            GraphPoint.localPosition = new Vector3((i * (2f / Resolution)) - ((NumberOfPoints) / 2), 0f, 0f);
        }
    }

    void Update()
    {
        for (int i = 0; i < points.Length; i++)
        {
            Transform GraphPoint = points[i];
            Vector3 Position = GraphPoint.localPosition;

            Position.y = MultiSine(Position.x, Time.time);
                //Mathf.Tan(Position.x+Time.time);

            GraphPoint.localPosition = Position;
        }
    }

    static float MultiSine(float _XPos, float _Time)
    {
        float Y;
        Y = 3 * Mathf.Sin(0.5f * _XPos + _Time);
        Y += Mathf.Sin(_XPos + (_Time * 9)) / 2f;
        Y += Mathf.Cos(_XPos + _Time + 1.8f) / 1.2f;

        return Y;
    }
}
