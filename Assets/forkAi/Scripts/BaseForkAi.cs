
using System.Collections;
using System.IO;
using System.Collections.Generic;

using System;
using Random = UnityEngine.Random;
 
using UnityEngine;


public class BaseForkAi : MonoBehaviour
{

    private static void loadAi(String aiResName, Dictionary<int, ForkAiNodeVo> aiNodeMap, out ForkAiNodeVo initNode)
    {
        initNode = null;
     
         MemoryStream ms = new MemoryStream(Resources.Load<TextAsset>(aiResName).bytes);
        StreamReader sr = new StreamReader(ms);
     
        
        while (!sr.EndOfStream)
        {
            ForkAiNodeVo vo = new ForkAiNodeVo();
            vo.aiType = int.Parse(sr.ReadLine());
            sr.ReadLine();
            sr.ReadLine();
            vo.id = int.Parse(sr.ReadLine());
            vo.param = sr.ReadLine().Split(',');
            vo.targertID = int.Parse(sr.ReadLine());
            vo.targertID2 = int.Parse(sr.ReadLine());
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();

            aiNodeMap[vo.id] = vo;

        }


        sr.Close();
        ms.Close();
        foreach (var item in aiNodeMap.Values)
        {
            if (item.targertID >= 0) item.targertVo = aiNodeMap[item.targertID];
            if (item.targertID2 >= 0) item.targertVo2 = aiNodeMap[item.targertID2];
            if (item.id == 0)
            {
                initNode = item;

            }
        }
    }

    internal void moveTo(int nodeID)
    {
        currentNode = aiNodeMap[nodeID];
    }   internal void executeTo(int nodeID)
    {
        moveTo(nodeID);
        executeCurrent();
    }

    Dictionary<int, ForkAiNodeVo> aiNodeMap = new Dictionary<int, ForkAiNodeVo>();
   internal ForkAiNodeVo currentNode;
    internal ForkAiNodeVo initNode;
    Dictionary<int, IForkAiNodeCmd> aiNodeCmds = new Dictionary<int, IForkAiNodeCmd>();
   internal void addNodeCmd(int aiTypeID, IForkAiNodeCmd cmd)
    {
        aiNodeCmds[aiTypeID] = cmd;
    }
    internal void removeNodeCmd(int aiTypeID)
    {
        aiNodeCmds.Remove(aiTypeID);
    }

    internal void init(string aiResName)
    {
        loadAi(aiResName, aiNodeMap, out initNode);
        currentNode = initNode;
    }


    internal string getParam(int index)
    {

        if (index >= currentNode.param.Length)
            return null;
        return currentNode.param[index];
    }
    internal int getParamInt(int index)
    {
        string str = getParam(index);
        if (str == null)
            return 0;
        if (str.StartsWith("r"))
        {
            string[] strArr = str.Split('_');
            return Random.Range(int.Parse(strArr[1]), int.Parse(strArr[2]));
        }
        else
        {
            return int.Parse(str);
        }

    }
    internal void executeNext(bool condition = true) {
        moveNext(condition);
        executeCurrent();
    }
    internal void executeCurrent()
    {

        
        if (currentNode == null)
        {
            return;
        }
        if (aiNodeCmds.ContainsKey(currentNode.aiType) == false) return;
        aiNodeCmds[currentNode.aiType].execute();
        
    }


    internal void executeNextCondition(float setVal, float realVal)
    {

        bool condition = false;
        switch (getParam(0))
        {
            case ">":
                condition = realVal > setVal;
                break;
            case "<":
                condition = realVal < setVal;
                break;
            case "=":
                condition = realVal == setVal;
                break;
            case "%=":
                condition = ((int)realVal + 1) % (int)setVal == 0;
                break;
        }


        executeNext(condition);
    }

    internal void moveNext(bool condition=true)
    {
        currentNode = condition ? currentNode.targertVo : currentNode.targertVo2;

    }
}
