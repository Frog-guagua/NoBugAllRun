using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//呱： woc我真快写疯了 我决定给待机虫虫 挂载这个美味脚本
//    代码求你自己跑起来
//    做法 # 做法
public class RoundManager : MonoBehaviour
{

    public static bool finishDrag;

    private BugMatch bugMatch;
    private int nowRound =1;
    void Start()
    {
        //呱：在小鼠老大没有写好这个逻辑前 先用这个强制启动 方便我做实验
        FightFlowManager.OnGame1 = true;
        
    }

    
    void Update()
    {
        
        bugMatch = GetComponent<BugMatch>();
    }

    //呱： 给第一关教学关卡 写的函数
    public void TeachingRound(GameObject nowBug)
    {
        if (nowBug == null) return;
       
        Debug.Log( "进入函数！");
       if(!FightFlowManager.OnGame1) return;
        
            if (nowRound == 1)
            {
                if (Draggable.nowBugType == E_BugType.A)
                {
                    //呱 ： 注意这里数组下标 需要减一 原本对应的是 第五格
                    if (Draggable.nowGridIndex == 4 )
                    {
                       Debug.Log("成功放置");
                       bugMatch.StartFightBug();
                       nowBug.SetActive(false) ;
                      
                    }
                }
                else if (Draggable.nowBugType == E_BugType.B)
                {
                    if (Draggable.nowGridIndex == 5)
                    {
                        bugMatch.StartFightBug();
                        nowBug.SetActive(false) ;
                    }
                }

                finishDrag= true;

            }
            else if (nowRound == 2)
            {
                
            }
            
    }
}
