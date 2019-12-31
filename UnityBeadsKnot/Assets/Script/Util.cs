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

    public void MakeLog(Knot knot)
    {

    }

    public void MakeKnot(Knot knot)
    {

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


