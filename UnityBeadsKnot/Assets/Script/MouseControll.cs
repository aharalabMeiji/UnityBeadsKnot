using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DIR
{
    public const int O1 = 0;
    public const int U1 = 1;
    public const int O2 = 2;
    public const int U2 = 3;

}

public class CONST
{
    public static float BeadsDistance = 0.3f;

}

public class MouseControll : MonoBehaviour {


    public  GameObject ThisKnot;
    Knot thisKnot;

    Vector3 MouseDownVec;
    Node DraggedNode = null;
    Vector3 DraggedNodeStartPosition;

    public static bool ModifyNode = true;
    public static bool ModifyBeads = false;
    public static bool DisplayEdgeLineRenderer = true;

    // Use this for initialization
    void Start () {
    }

    void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDown();
        }
        if (Input.GetMouseButton(0))
        {
            OnMouseDrag();
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnMouseUp();
        }
    }


    public void OnMouseDown()
    {
        MouseDownVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MouseDownVec.z = 0f;
        //Debug.Log(MouseDownVec);
        thisKnot = ThisKnot.GetComponent<Knot>();
        thisKnot.GetAllThings();
        
        for(int n=0; n<thisKnot.AllNodes.Length; n++)
        {
            float dist = (MouseDownVec - thisKnot.AllNodes[n].Position).magnitude;
            if(dist < 0.25){
                DraggedNode = thisKnot.AllNodes[n];
                DraggedNodeStartPosition = thisKnot.AllNodes[n].Position;
                return;
            }
        }
    }

    public void OnMouseDrag()
    {
        if(DraggedNode != null)
        {
            Vector3 tmpPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tmpPos.z = 0f;
            float minDist = (tmpPos - DraggedNodeStartPosition).magnitude;
            int minNodeId = DraggedNode.ID;
            for (int n = 0; n < thisKnot.AllNodes.Length; n++)
            {
                Node nd = thisKnot.AllNodes[n];
                if (nd.ID != DraggedNode.ID)
                {
                    float dist = (tmpPos - nd.Position).magnitude;
                    if (dist < minDist)
                    {
                        return;
                    }
                }
            }
            // ドラッグされたBeadの座標を変える。
            DraggedNode.ThisBead.Position = tmpPos;
            // Nodeの座標も同期する
            DraggedNode.Position = tmpPos;
            //Debug.Log("mousePosition"+tmpPos);
            // エッジを作り直す。// Knot.UpdateBeadsを呼び出す。
            ThisKnot.GetComponent<Knot>().UpdateBeadsAtNode(DraggedNode);
            // ドラッグしているノードについて、回転して適正な位置にする。
        }
    }

    public void OnMouseUp()
    {
        DraggedNode = null;
    }


    //Node AddNode(float x, float y, float t)
    //{
    //    PreFab = Resources.Load("Prefabs/Node") as GameObject;
    //    GameObject go = Node.Instantiate<GameObject>(PreFab, Vector3.zero, Quaternion.identity);
    //    Node nd = go.GetComponent<Node>();
    //    nd.R = new float[4];
    //    nd.EdgeID = new int[4];
    //    nd.SetNodeCoord(x, y, t);
    //    for (int i = 0; i < 4; i++)
    //    {
    //        nd.R[i] = 1f;
    //    }
    //    //Debug.Log("R[3] = "+nd.R[3]);
    //    nd.ID = NodesID;
    //    NodesID++;
    //    return nd;
    //}

    //Edge AddEdge(int ID1, int ID2, int RID1, int RID2)
    //{
    //    PreFab = Resources.Load("Prefabs/Edge") as GameObject;
    //    GameObject go = Node.Instantiate<GameObject>(PreFab, Vector3.zero, Quaternion.identity);
    //    Edge ed = go.GetComponent<Edge>();
    //    ed.SetEdgePara(ID1,ID2, RID1, RID2);
    //    Debug.Log("" + ed.ANodeID + " " + ed.BNodeID);
    //    ed.ID = EdgesID;
    //    GetNodeAt(ID1).EdgeID[RID1] = EdgesID;
    //    GetNodeAt(ID2).EdgeID[RID2] = EdgesID;
    //    EdgesID++;
    //    return ed;
    //}
    //static public int GetNodeLength()
    //{
    //    if (Nodes == null) return 0;
    //    return Nodes.Length;
    //}

    //public static Node GetNodeAt(int i)
    //{
    //    for(int n=0; n< Nodes.Length; n++)
    //    {
    //        if(Nodes[n].ID==i)
    //        {
    //            return Nodes[n];
    //        }
    //    }
    //    return null;
    //}

    //public static Edge GetEdgeAt(int i)
    //{
    //    for (int n = 0; n < Edges.Length; n++)
    //    {
    //        if (Edges[n].ID == i)
    //        {
    //            return Edges[n];
    //        }
    //    }
    //    return null;
    //}


    //void ModifyR_Nodes()
    //{

    //}

    //void ModifyTh_Nodes()
    //{

    //}

    //void ModifyXY_Nodes()
    //{

    //}
}
