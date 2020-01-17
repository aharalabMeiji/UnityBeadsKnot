using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLoop : MonoBehaviour
{
    public List<Vector3> FreeCurve;
    public GameObject CircleEffect;

    Vector3[] CircleEffectVec;
    float CircleEffectRadius;
    public Vector3 CircleEffectPosition;
    public bool CircleEffectEnable;

    public Knot ParentKnot;

    void Start()
    {
        FreeCurve = new List<Vector3>();
        GetComponent<LineRenderer>().enabled = false;
        CircleEffectVec = new Vector3[20];
        CircleEffect.GetComponent<LineRenderer>().enabled = CircleEffectEnable = false;
        CircleEffectPosition = Vector3.zero;
        CircleEffectRadius = 1f;
    }


    void Update()
    {
        RenderFreeCurve();
        RenderCircleEffect();
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
                LR.SetPosition(i, FreeCurve[i] * ParentKnot.GlobalRate);
            }
        }
        else
        {
            LR.enabled = false;
        }
    }

    public void RenderCircleEffect()
    {
        if (CircleEffectEnable)
        {
            LineRenderer LR = CircleEffect.GetComponent<LineRenderer>();
            LR.positionCount = 20;
            for (int i=0; i<20; i++)
            {
                CircleEffectVec[i] = CircleEffectRadius * (Vector3.right * Mathf.Cos(Mathf.PI * i / 10) + Vector3.up * Mathf.Sin(Mathf.PI * i / 10));
            }
            LR.SetPositions(CircleEffectVec);
            CircleEffect.transform.localPosition = CircleEffectPosition;
            CircleEffectRadius += 0.02f;
            if (CircleEffectRadius > .7f) CircleEffectRadius = 0.2f;
        }
    }
}
