using UnityEngine;

public struct AdvancedMath 
{
    /// <summary>
    /// Yüzdelik değer alır
    /// </summary>
    /// <param name="value"></param>
    /// <param name="percent"></param>
    /// <returns></returns>
    public static float Percent(int value, int percent){
        percent = Mathf.Clamp(percent, 0, 100);
        float result = (value * percent / 100);
        return result;
    }

    /// <summary>
    /// Vector3 için inverse lerp methodu
    /// </summary>
    /// <param name="a">t:0</param>
    /// <param name="b">t:1</param>
    /// <param name="value">current value</param>
    /// <returns></returns>
    public static float Vector3InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
    }

    /// <summary>
    /// Bir sayıyı bir aralıktan diğer aralığa uyarlar
    /// </summary>
    /// <param name="a1">1.aralık t:0</param>
    /// <param name="b1">1.aralık t:1</param>
    /// <param name="a2">2.aralık t:0</param>
    /// <param name="b2">2.aralık t:1</param>
    /// <param name="value">1.aralığa göre değer</param>
    /// <returns></returns>
    public static float Remap(float a1, float b1, float a2, float b2, float value)
    {
        float t = Mathf.InverseLerp(a1, b1, value);
        return Mathf.Lerp(a2, b2, t);
    }

    /// <summary>
    /// Bir vektörü (1,0,1) ile scale eder
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector3 ScaleTopdown(Vector3 vector)
    {
        return Vector3.Scale(vector, new Vector3(1f, 0f, 1f));
    }

    /// <summary>
    /// vector3 için LerpAngle methodu
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector3 VectorLerpAngles(Vector3 a, Vector3 b, float t)
    {
        return new Vector3(Mathf.LerpAngle(a.x, b.x, t),
                            Mathf.LerpAngle(a.y, b.y, t),
                            Mathf.LerpAngle(a.z, b.z, t));
    }

/// <summary>
/// Ondalık sayıyı aralıktamı diye kontrol eder. a ve b dahil değil.
/// </summary>
/// <param name="value">Kontrol etmek istediğiniz sayı</param>
/// <param name="a">Birinci aralık değeri</param>
/// <param name="b">İkinci aralık değeri</param>
    public static bool IsBetween(float value, float a, float b){
        return value > Mathf.Min(a, b) && value < Mathf.Max(a,b);
    }

/// <summary>
/// Ondalık sayıyı aralıktamı diye kontrol eder. Aralığı belirten sayılar dahil değil.
/// </summary>
/// <param name="value">Kontrol etmek istediğiniz sayı</param>
/// <param name="ranges">Aralıklar</param>
    public static bool IsBetween(float value, Vector2[] ranges)
    {
        bool result = false;
        foreach(Vector2 range in ranges)
        {
            if (value > Mathf.Min(range.x, range.y) && value < Mathf.Max(range.x, range.y))
            {
                result = true;
                break;
            }
        }
        return result;
    }
    
/// <summary>
/// Bir dizi nokta içinde belirtilen referans noktasına en yakın olanını geri döndürür.
/// </summary>
/// <param name="referencePoint">Referans noktası</param>
/// <param name="points">Karşılaştırılacak noktalar</param>
/// <returns></returns>
    public static Vector2 ClosestPoint(Vector2 referencePoint, Vector2[] points)
    {
        Vector2 result = Vector2.zero;
        float distance = 0f;

        for (int i = 0; i < points.Length; i++)
        {
            float curDistance = Vector2.Distance(referencePoint, points[i]);
            if (curDistance < distance || i == 0)
            {
                distance = curDistance;
                result = points[i];
            }
        }
        return result;
    }

/// <summary>
/// Bir float değerine, belirtilen listedeki en yakın değeri döndürür.
/// </summary>
/// <param name="value"></param>
/// <param name="targets"></param>
/// <returns></returns>
    public static float ClosestValue(float value, float[] targets)
    {
        float result = 0;
        for (int i = 0; i < targets.Length; i++)
        {
            float distance = Mathf.Abs(value-targets[i]);
            if (distance < result || i == 0){
                result = distance;
            }
        }
        return result;
    }

/// <summary>
/// Bir ikili float dizisinde, farkın en az olduğu değeri döndürür
/// </summary>
/// <param name="value"></param>
/// <param name="targets"></param>
/// <returns></returns>
    public static Vector2 ClosestDisValue(Vector2[] values)
    {
        int result = 0;
        float dis = values[0].y - values[0].x;

        for (int i = 1; i < values.Length; i++)
        {
            float newDis = values[i].y - values[i].x;
            if (newDis < dis){
                result = i;
            }
        }
        return values[result];
    }

/// <summary>
/// Bir dizi sayının aritmetik ortalamasını alır.
/// </summary>
/// <param name="values">Sayı dizisi</param>
/// <returns></returns>
    public static float ArithmeticMean (float[] values)
    {
        float result = 0f;
        for(int i = 0; i < values.Length; i ++)
        {
            result += values[i];
            Debug.Log(i);
        }
        result /= values.Length;
        return result;
    }

/// <summary>
/// Sağlıklı bir şekilde bir vektörü diğer vektöre belirtilen hızla çevirir. [ Vector3 ]
/// </summary>
/// <param name="current">Değişecek vektör</param>
/// <param name="target">Hedef vektör</param>
/// <param name="speed">Hız</param>
/// <returns></returns>
    public static Vector3 ChangeVectorBySpeed(Vector3 current, Vector3 target, float speed, float deltaTime)
    {
        Vector3 newMovePosition = target;
        Vector3 dir = (target - current).normalized;
        float dis = Vector3.Distance(current, target);
        if (dis > 0)
        {
            newMovePosition = current + dir * dis * speed * deltaTime;

            float distanceAfterMoving = Vector3.Distance(current, newMovePosition);

            if (distanceAfterMoving > dis)
            {
                newMovePosition = target;
            }
        }

        return newMovePosition;
    }

/// <summary>
/// Sağlıklı bir şekilde bir ondalık değeri diğer ondalık değere belirtilen hızla çevirir. [ Vector3 ]
/// </summary>
/// <param name="current">Değişecek vektör</param>
/// <param name="target">Hedef vektör</param>
/// <param name="speed">Hız</param>
/// <returns></returns>
    public static float ChangeFloatBySpeed(float current, float target, float speed)
    {
        float newMovePosition = target;
        float dir = Mathf.Sign(target - current);
        float dis = Mathf.Abs(target - current);
        if (dis > 0)
        {
            newMovePosition = current + dir * dis * speed * Time.deltaTime;

            float distanceAfterMoving = Mathf.Abs(current - newMovePosition);

            if (distanceAfterMoving > dis)
            {
                newMovePosition = target;
            }
        }

        return newMovePosition;
    }

/// <summary>
/// Sağlıklı bir şekilde bir vektörü diğer vektöre belirtilen hızla çevirir. [ Vector2 ]
/// </summary>
/// <param name="current">Değişecek vektör</param>
/// <param name="target">Hedef vektör</param>
/// <param name="speed">Hız</param>
/// <returns></returns>
    public static Vector2 ChangeVectorBySpeed(Vector2 current, Vector2 target, float speed)
    {
        Vector2 newMovePosition = target;
        Vector2 dir = (target - current).normalized;
        float dis = Vector2.Distance(current, target);
        if (dis > 0)
        {
            newMovePosition = current + dir * dis * speed * Time.deltaTime;

            float distanceAfterMoving = Vector2.Distance(current, newMovePosition);

            if (distanceAfterMoving > dis)
            {
                newMovePosition = target;
            }
        }

        return newMovePosition;
    }

/// <summary>
/// Euler bir açı değerini negatif açıları dahil ederek hesaplar [açılar -180 ile 180 arasında kalır] 
/// </summary>
/// <param name="angle">Açı değerini girin</param>
    public static float NegativeAngle(float angle)
    {
        return (angle > 180) ? angle - 360 : angle;
    }

/// <summary>
/// Rasgele bir yön belirtir
/// </summary>
/// <returns></returns>
    public static int RandomSign()
    {
        return Random.Range(0, 2) == 1 ? 1 : -1;
    }
}