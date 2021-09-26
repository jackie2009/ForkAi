using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkAiDemo1 : BaseForkAi
{
    public string testAi = "dadoudou";
 
    internal void addHP(int v)
    {
        HP += v;
        HP = Mathf.Clamp(HP, 0, 100);
    }
    internal void addMP(int v)
    {
        MP += v;
        MP = Mathf.Clamp(MP, 0, 100);
    }
    [HideInInspector]
    public int HP;
    [HideInInspector]
    public int MP;
    [HideInInspector]
    public string stateMsg;
    // Start is called before the first frame update
    void Start()
    {
        HP = MP = 50;
        init(testAi);
          addNodeCmd(0,new EmptyNodeCmd(this));
          addNodeCmd(1,new EatNodeCmd(this));
          addNodeCmd(2,new SleepNodeCmd(this));
          addNodeCmd(3,new AttackNodeCmd(this));
          addNodeCmd(4,new HPCheckNodeCmd(this));
          addNodeCmd(5,new MPCheckNodeCmd(this));

        StartCoroutine(loop());
     }
    private IEnumerator loop()
    {
        var wait = new WaitForSeconds(0.1f);
        while (true)
        {
            yield return wait;
            executeCurrent();
        }
    }
   

private void OnGUI()
    {
        GUILayout.Label("AI 规则:HP==0时就吃饭 直到HP满100，MP==0时就睡觉直到MP满100，其他时候打豆豆");
        
        GUILayout.BeginArea(new Rect(100, 100, 500, 200));
        GUILayout.Label("HP:" +HP + new string('-',HP));
        GUILayout.Label("MP:" +MP + new string('-', MP));
        GUILayout.Label("正在" + stateMsg + "...");
        GUILayout.EndArea();
    }





}

 