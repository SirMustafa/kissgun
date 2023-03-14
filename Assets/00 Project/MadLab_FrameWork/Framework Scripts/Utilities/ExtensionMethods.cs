using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct LaunchData{
    public readonly Vector3 initialVelocity;
    public readonly float timeToTarget;

    public LaunchData(Vector3 initialVelocity, float timeToTarget)
    {
        this.initialVelocity = initialVelocity;
        this.timeToTarget = timeToTarget;
    }
}

public static class ExtensionMethods
{
    public static void RotateAround(this Rigidbody rb, Vector3 origin, Vector3 axis, float angle)
    {
        Quaternion q = Quaternion.AngleAxis(angle, axis);
        rb.MovePosition(q * (rb.transform.position - origin) + origin);
        rb.MoveRotation(rb.transform.rotation * q);
    }

    #region Projectile Launcher - Rigidbody Extension

    public static void ProjectileLauncher(this Rigidbody rb, float h, Vector3 targetPosition)
    {
        float gravity = Physics.gravity.y;
        float displacementY = targetPosition.y - rb.position.y;
        Vector3 displacementXZ = new Vector3(targetPosition.x - rb.position.x, 0f, targetPosition.z - rb.position.z);
        float time = Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(2 * (displacementY - h) / gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
        Vector3 velocityXZ = displacementXZ / time;
        
        LaunchData launchData = new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);

        if (!rb.useGravity)
            rb.useGravity = true;
        if (rb.isKinematic)
            rb.isKinematic = false;
        rb.velocity = launchData.initialVelocity;
    }

    public static void ProjectileLauncher(this Rigidbody rb, float h, Vector3 targetPosition, out float time2Target)
    {
        float gravity = Physics.gravity.y;
        float displacementY = targetPosition.y - rb.position.y;
        Vector3 displacementXZ = new Vector3(targetPosition.x - rb.position.x, 0f, targetPosition.z - rb.position.z);
        float time = Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(2 * (displacementY - h) / gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
        Vector3 velocityXZ = displacementXZ / time;
        
        LaunchData launchData = new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
        time2Target = time;

        if (!rb.useGravity)
            rb.useGravity = true;
        if (rb.isKinematic)
            rb.isKinematic = false;
        rb.velocity = launchData.initialVelocity;
    }

    public static void ProjectileLauncher(this Rigidbody rb, float h, Vector3 targetPosition, out LaunchData launchData)
    {
        float gravity = Physics.gravity.y;
        float displacementY = targetPosition.y - rb.position.y;
        Vector3 displacementXZ = new Vector3(targetPosition.x - rb.position.x, 0f, targetPosition.z - rb.position.z);
        float time = Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(2 * (displacementY - h) / gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
        Vector3 velocityXZ = displacementXZ / time;
        
        launchData = new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);

        if (!rb.useGravity)
            rb.useGravity = true;
        if (rb.isKinematic)
            rb.isKinematic = false;
        rb.velocity = launchData.initialVelocity;
    }

    #endregion

    public static float TimeFromValue(this AnimationCurve c, float value, float precision = 1e-6f)
    {
        float minTime = c.keys[0].time;
        float maxTime = c.keys[c.keys.Length-1].time;
        float best = (maxTime + minTime) / 2;
        float bestVal = c.Evaluate(best);
        int it=0;
        const int maxIt = 1000;
        float sign = Mathf.Sign(c.keys[c.keys.Length-1].value -c.keys[0].value);
        while(it < maxIt && Mathf.Abs(minTime - maxTime) > precision) {
            if((bestVal - value) * sign > 0) {
                maxTime = best;
            } else {
                minTime = best;
            }
            best = (maxTime + minTime) / 2;
            bestVal = c.Evaluate(best);
            it++;
        }
        return best;
    }
}
