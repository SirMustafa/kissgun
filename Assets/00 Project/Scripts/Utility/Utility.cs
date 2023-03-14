using System;
using System.Collections;
using UnityEngine;

public enum GizmosDrawTypes
{
    Cube,
    WireCube,
    Sphere,
    WireSphere
}

public enum TimeScaleType
{
    Scaled,
    UnScaled
}


public delegate void simpleDelegate();

namespace MadLab.Utilities {
    
    public static class Utility
    {
    #if UNITY_EDITOR
            public static void PauseEditor()
            {
                UnityEditor.EditorApplication.isPaused = true;
            }
    #endif

        public static IEnumerator Tween(Action<float> param, float start, float to, float duration, AnimationCurve Ease, TimeScaleType timeScale = TimeScaleType.Scaled)
        {
            float t = 0f;
            float tSpeed = 1 / duration;

            while(t != duration)
            {
                t += timeScale == TimeScaleType.Scaled ? tSpeed * Time.deltaTime : tSpeed * Time.unscaledDeltaTime;
                t = Mathf.Clamp01(t);

                float temporaryParam = Mathf.Lerp(start, to, Ease.Evaluate(t));

                param.Invoke(temporaryParam);
                yield return null;
            }
        }


        /// <summary>
        /// Extension method to check if a layer is in a layermask
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static bool Contains(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }

        public static Transform[] GetTopLevelChildren(Transform Parent)
        {
            Transform[] Children = new Transform[Parent.childCount];
            for (int ID = 0; ID < Parent.childCount; ID++)
            {
                Children[ID] = Parent.GetChild(ID);
            }
            return Children;
        }

        public static T GetParamWithNameFromArray<T>(T param, T[] array)
        {
            T myParam = param;
            foreach(T arrayElement in array)
            {
                if (param.ToString() == arrayElement.ToString() && param.GetType() == array[0].GetType())
                {
                    myParam = arrayElement;
                    break;
                }
            }
            return myParam;
        }

    #if UNITY_EDITOR
        /// <summary>
        /// Gizmos Draw Cube With Rotation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        public static void GizmosDrawWithRotation(GizmosDrawTypes drawType, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Matrix4x4 cubeTransform = Matrix4x4.TRS(position, rotation, scale);
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

            Gizmos.matrix *= cubeTransform;

            switch(drawType)
            {
                case GizmosDrawTypes.Cube:
                    Gizmos.DrawCube(Vector3.zero, scale);
                    break;
                case GizmosDrawTypes.WireCube:
                    Gizmos.DrawWireCube(Vector3.zero, scale);
                    break;
                case GizmosDrawTypes.Sphere:
                    Gizmos.DrawSphere(Vector3.zero, scale.x);
                    break;
                case GizmosDrawTypes.WireSphere:
                    Gizmos.DrawWireSphere(Vector3.zero, scale.x);
                    break;
            }

            Gizmos.matrix = oldGizmosMatrix;
        }
    #endif
        public static void ChangeLayersRecursively(GameObject targetObject, string layerName)
        {
            targetObject.layer = LayerMask.NameToLayer(layerName);

            foreach (Transform child in targetObject.GetComponentsInChildren<Transform>(true))  
            {
                child.gameObject.layer = LayerMask.NameToLayer(layerName);  // add any layer you want. 
            }   
        }
    /// <summary>
    /// Görüntü kaybolur
    /// </summary>
        public static IEnumerator FadeIn(CanvasGroup fadePanel, float fadeSpeed, simpleDelegate successDelegate = null)
        {
            while (fadePanel.alpha > 0)
            {
                fadePanel.alpha -= fadeSpeed * Time.unscaledDeltaTime;
                yield return null;
            }

            if (successDelegate != null)
                successDelegate();
        }

        /// <summary>
        /// bir floata smoothStep uygular
        /// Action şöyle kullanılmalı : (x) => refValue = x
        /// </summary>
        public static IEnumerator FloatSmoothChanging(Action<float> refValue, float startV, float targetV, float duration, simpleDelegate successDelegate = null)
        {
            float sT = Time.time;
            float eT = sT + duration;

            while (Time.time < eT)
            {
                float t = (Time.time - sT) / duration;

                refValue(Mathf.SmoothStep(startV, targetV, t));

                yield return null;
            }

            if (successDelegate != null)
                successDelegate();
        }

    /// <summary>
    /// Görüntü gözükür
    /// </summary>
        public static IEnumerator FadeOut(CanvasGroup fadePanel, float fadeSpeed, simpleDelegate successDelegate = null)
        {
            while (fadePanel.alpha < 1)
            {
                fadePanel.alpha += fadeSpeed * Time.unscaledDeltaTime;
                yield return null;
            }

            if (successDelegate != null)
                successDelegate();
        }

        #region Color

        private static int Hex2Dec(string hex)
        {
            return System.Convert.ToInt32(hex, 16);
        } 

        private static string Dec2Hex(int value)
        {
            return value.ToString("X2");
        } 

        private static float Hex2FloatNormalized(string hex)
        {
            return Hex2Dec(hex) / 255f;
        }

        private static string FloatNormalized2Hex(float value)
        {
            return Dec2Hex(Mathf.RoundToInt(value * 255f));
        }

        public static Color GetColorFromString(string hex)
        {
            float r = Hex2FloatNormalized(hex.Substring(0,2));
            float g = Hex2FloatNormalized(hex.Substring(2,2));
            float b = Hex2FloatNormalized(hex.Substring(4,2));
            float a = 1f;
            if (hex.Length >= 8) {
                a = Hex2FloatNormalized(hex.Substring(6, 2));
            }

            return new Color(r, g, b, a);
        }

        public static string GetStringFromColor(Color color, bool useAlpha = false)
        {
            string r = FloatNormalized2Hex(color.r);
            string g = FloatNormalized2Hex(color.g);
            string b = FloatNormalized2Hex(color.b);
            if (!useAlpha)
            {
                return r + g + b;
            }
            else
            {
                string a = FloatNormalized2Hex(color.a);
                return r + g + b + a;
            }
        }

        #endregion
    }
}