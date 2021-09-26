using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkAiDemo2 : BaseForkAi
{
    public string testAi = "fork_ai_demo2";
    private LinkedList<string> debugMsg=new LinkedList<string>();
   
    internal  void addMsg(string v)
    {
        debugMsg.AddLast(v);
        if (debugMsg.Count > 7) debugMsg.RemoveFirst();
    }

 
    void Start()
    {
       
        init(testAi);
        addNodeCmd(0, new EmptyNodeCmd(this));
        addNodeCmd(7, new RandomSwitchNodeCmd(this));
        addNodeCmd(6, new UseSkillNodeCmd(this));


        StartCoroutine(loop());
    }

    private IEnumerator loop()
    {
        var wait = new WaitForSeconds(0.5f);
        while (true)
        {
            yield return wait;
            executeCurrent();
        }
    }

    private void OnGUI()
    {

        GUILayout.BeginArea(new Rect(700, 100, 500, 200));
        GUILayout.Label("AI 规则:每次随机一个技能，但咆哮和普攻 不能连续出");
        foreach (var item in debugMsg)
        {
            GUILayout.Label(item);
        }
      
      
        GUILayout.EndArea();
    }

}
