using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//呱： woc我真快写疯了 我决定给待机虫虫 挂载这个美味脚本
//    代码求你自己跑起来
//    做法 # 做法
public class RoundManager : MonoBehaviour
{
    
    private bool aPlaced = false;
    private bool bPlaced = false;
    

    private BugMatch bugMatch;
    private Draggable draggable;
    public static int nowRound =1;
    
    private FightDataManager fightDataManager;
    private FightFlowManager fightFlowManager;
    private Hint hint;

    
    
    
    void Start()
    {
        //呱：在小鼠老大没有写好这个逻辑前 先用这个强制启动 方便我做实验
        FightFlowManager.OnGame1 = true;
        fightFlowManager = FindObjectOfType<FightFlowManager>();
        fightDataManager = FindObjectOfType<FightDataManager>();
        hint = FindObjectOfType<Hint>();
    }

    
    void Update()
    {
        draggable = GetComponent<Draggable>();
        bugMatch = GetComponent<BugMatch>();
    }

    //呱： 给第一关教学关卡 写的函数
    public void TeachingRound(GameObject nowBug)
    { 
        Debug.Log("进关卡了！");
        ActionPoint actionPoint = FindObjectOfType<ActionPoint>();
        
        if (nowBug == null) return;
       if(!FightFlowManager.OnGame1) return;
       if(FightDataManager.ActionPoints <= 0) return;
       
      
       
       if (nowRound == 1) 
       {
           Debug.Log("在第一回合！");
                if (Draggable.nowBugType == E_BugType.A)
                {
                    //呱 ： 注意这里数组下标 需要减一 原本对应的是 第五格
                    if (Draggable.nowGridIndex == 4 )
                    {
                        
                        bugMatch.StartFightBug();
                       nowBug.SetActive(false) ;
                       aPlaced = true;
                       FightDataManager.ActionPoints -= 1;
                        FightFlowManager.count++;
                        AudioMgr.Instance.PlaySFX(Id_To_Insect_Dic.IdToInsectDic[1].insectSound);
                        actionPoint.UpdatePoints(FightDataManager.ActionPoints);
                        if (aPlaced && bPlaced)
                        {
                            nowRound =2 ;
                            aPlaced = false;
                            bPlaced = false;
          
                            return;
           
                        }
                    }
                }
                else if (Draggable.nowBugType == E_BugType.B)
                {
                    if (Draggable.nowGridIndex == 5)
                    {
                        bugMatch.StartFightBug();
                        nowBug.SetActive(false) ;
                        bPlaced = true;
                        FightDataManager.ActionPoints -= 1;
                        FightFlowManager.count++;
                        AudioMgr.Instance.PlaySFX(Id_To_Insect_Dic.IdToInsectDic[2].insectSound);
                        actionPoint.UpdatePoints(FightDataManager.ActionPoints);
                    }
                    if (aPlaced && bPlaced)
                    {
                        nowRound =2 ;
                        aPlaced = false;
                        bPlaced = false;
          
                        return;
           
                    }
                    
                }

       }
       else if (nowRound == 2)
       {
           Debug.Log("进入第二回合了！");
           if (Draggable.nowBugType == E_BugType.A && Draggable.nowGridIndex == 0)
           {
               Debug.Log("放对拉！");
               bugMatch.StartFightBug();
               
               Transform parentTransform = Draggable.nowBug.transform.parent;
               if (parentTransform == null)
               {
                   Debug.LogWarning("nowBug 没有父物体");
                   return;
               }

              
              InsectData bugA = nowBug.GetComponent<InsectData>();
                     
              fightDataManager.UpdateFightBugAtIndex(10,bugA);
                       
                   
                 
               parentTransform.gameObject.SetActive(false);
               Debug.Log($"已禁用 {parentTransform.name}");
               
               
               nowBug.SetActive(false);
               FightDataManager.ActionPoints -= 1;
               FightFlowManager.count++;
               AudioMgr.Instance.PlaySFX(Id_To_Insect_Dic.IdToInsectDic[1].insectSound);
               actionPoint.UpdatePoints(FightDataManager.ActionPoints);
               
           }
           
       }
            
    }

    
}
