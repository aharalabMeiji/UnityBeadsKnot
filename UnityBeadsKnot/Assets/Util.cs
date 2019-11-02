using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    /// <summary>
    /// (a,c)からみた(p,q)の角度。0-2PIで返す
    /// </summary>
    public static float Atan2Vec(float a, float c, float p, float q)
    {
        float b = -c;
        float d = a;
        float s = (p * d - b * q) / (a * d - b * c);
        float t = (a * q - p * c) / (a * d - b * c);
        float ret = Mathf.Atan2(t, s);
        if (ret < 0)
        {
            ret += 2 * Mathf.PI;
        }
        return ret;
    }


}
