using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nbhd : MonoBehaviour
{
    public Bead ABead = null, BBead = null;
    public int ID=-1, AID=-1, BID=-1;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        LineRenderer LR = GetComponent<LineRenderer>();
        if (ABead != null && BBead != null)
        {
            LR.enabled = true;
            LR.positionCount = 2;
            LR.SetPosition(0, ABead.Position);
            LR.SetPosition(1, BBead.Position);
        }
        else
        {
            LR.enabled = false;
        }
    }
}
