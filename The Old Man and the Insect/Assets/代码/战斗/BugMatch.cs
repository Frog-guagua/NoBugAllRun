using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//呱：这个脚本意在让 待机虫虫 和 战斗虫虫 形成匹配关系
//   一山不容二虫
//   亲爱的小电脑小unity小rider今天心情怎么样
//   求求你们正常的跑起来吧……
public class BugMatch : MonoBehaviour
{
    [SerializeField]  GameObject idleBug;
    [SerializeField]  GameObject fightBug;
    
    
    void Start()
    {
        fightBug.SetActive(false);
    }

   
    void Update()
    {
        
    }

    public void StartFightBug()
    {
        fightBug.SetActive(true);
    }
}
