using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class NodeData
{
    public float PositionX;
    public float PositionY;
    public float Theta;
    public float R0;
    public float R1;
    public float R2;
    public float R3;
    public NodeData(float _PositionX, float _PositionY, float _Theta, float _R0, float _R1, float _R2, float _R3)
    {
        PositionX = _PositionX;
        PositionY = _PositionY;
        Theta = _Theta;
        R0 = _R0;
        R1 = _R1;
        R2 = _R2;
        R3 = _R3;
    }
}

public class EdgeData
{
    public int AID, ARID, BID, BRID;
    public EdgeData(int _AID, int _ARID, int _BID, int _BRID)
    {
        AID = _AID;
        ARID = _ARID;
        BID = _BID;
        BRID = _BRID;
    }
}

public class NodeEdgeLog
{
    List<NodeData> Nodes;
    List<EdgeData> Edges;
    public NodeEdgeLog()
    {
        Nodes = new List<NodeData>();
        Edges = new List<EdgeData>();
    }
    public NodeEdgeLog(Knot knot)
    {
        Nodes = new List<NodeData>();
        Edges = new List<EdgeData>();
        MakeLog(knot);
    }

    public void MakeLog(Knot knot)
    {
        knot.GetAllThings();
        for (int i = 0; i < knot.AllNodes.Length; i++)
        {
            Node nd = knot.AllNodes[i];
            Nodes.Add(new NodeData(
                (100f * nd.Position.x + 500f) ,
                (-100f * nd.Position.y + 500f) ,
                nd.Theta ,
                (100f * nd.R[0]) ,
                (100f * nd.R[1]) ,
                (100f * nd.R[2]) ,
                (100f * nd.R[3])
            ));
        }
        for (int i = 0; i < knot.AllEdges.Length; i++)
        {
            Edge ed = knot.AllEdges[i];
            Edges.Add(new EdgeData(
                ed.ANodeID ,
                ed.ANodeRID ,
                ed.BNodeID ,
                ed.BNodeRID
                )
            );
        }
    }

    public void MakeKnot(Knot knot)
    {
        return ;
    }

    public override string ToString()
    {
        string ret = "";
        ret += "Node," + Nodes.Count + "\n";
        for (int i = 0; i < Nodes.Count; i++)
        {
            NodeData nddt = Nodes[i];
            ret += (nddt.PositionX) + ","
                + (nddt.PositionY) + ","
                + nddt.Theta + ","
                + (nddt.R0) + ","
                + (nddt.R1) + ","
                + (nddt.R2) + ","
                + (nddt.R3) + "\n";
        }
        ret += "Edge," + Edges.Count + "\n";
        for (int i = 0; i < Edges.Count; i++)
        {
            EdgeData eddt = Edges[i];
            ret += ""
                + eddt.AID + ","
                + eddt.ARID + ","
                + eddt.BID + ","
                + eddt.BRID + "\n" ;
        }
        return ret;
    }
}

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

    public void SaveTxtFile(string path)
    {
        //string path = Crosstales.FB.FileBrowser.SaveFile("Save a BeadsKnot file", "", "BeadsKnotSample.txt", "txt");
        try
        {
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                //Debug.Log("start savelog : logLength = " + LogLength);
                //for (int i = 0; i < World.LogList.Count; i++)
                //{
                //    if (World.LogList[i].GetComponent<GameLog>().Active)
                //    {
                //        writer.WriteLine(World.LogList[i].GetComponent<GameLog>().ToString());
                //    }
                //}
                writer.Flush();
                writer.Close();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        //WorldObject.GetComponent<World>().MenuOffButtons();
        //World.Mode = MODE.ADD_POINT;
    }


}


