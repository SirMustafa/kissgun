using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class SplineLogger : MonoBehaviour
{
    private SplineFollower follower;
    [SerializeField] private GameObject point;

    private void Start()
    {
        follower = GetComponent<SplineFollower>();
    }

    public void LogMe()
    {
        SplinePoint[] points = follower.spline.GetPoints();

        Debug.Log("Follower Start : " + follower.EvaluatePosition(0));
        Debug.Log("Follower Start : " + follower.EvaluatePosition(1));

        for (int i = 0; i < points.Length; i++)
        {
            Debug.Log("Spline Point " + i + " : " + points[i].position);
        }

        Instantiate(point, follower.EvaluatePosition(0), Quaternion.identity).GetComponent<MeshRenderer>().material.color = Color.green;
        Instantiate(point, follower.EvaluatePosition(1), Quaternion.identity).GetComponent<MeshRenderer>().material.color = Color.red;
    }
}