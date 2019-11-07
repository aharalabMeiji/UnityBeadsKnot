using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLoop : MonoBehaviour
{
    public List<Vector3> FreeCurve;

    void Start()
    {
        FreeCurve = new List<Vector3>();
        GetComponent<LineRenderer>().enabled = false;
    }


    void Update()
    {
        RenderFreeCurve();
    }

    public void AddPoint2FreeCurve(Vector3 v)
    {
        Vector3 vv = v;
        vv.z = 0;
        FreeCurve.Add(vv);
    }

    public void CloseFreeCurve()
    {
        FreeCurve.Add(FreeCurve[0]);
    }

    void RenderFreeCurve()
    {
        LineRenderer LR = GetComponent<LineRenderer>();
        if (FreeCurve.Count >= 2)
        {
            LR.enabled = true;
            LR.positionCount = FreeCurve.Count;
            for(int i=0; i < FreeCurve.Count; i++)
            {
                LR.SetPosition(i, FreeCurve[i]);
            }
        }
        else
        {
            LR.enabled = false;
        }
    }
}
