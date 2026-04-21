using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class FightDataManager : MonoBehaviour
{
    [Header("己方UI")]
    [SerializeField] List<TextMeshProUGUI> tagDatas = new List<TextMeshProUGUI>();   
   
    [SerializeField] List<TextMeshProUGUI> fightBugDatas = new List<TextMeshProUGUI>(); 
    
    //呱： 这是鼠鼠主人给的 虫虫数据
    /// <summary>
    /// 传入的虫虫数据
    /// </summary>
    [SerializeField] List<InsectData> bugs = DataBroker.Instance.datasFromCage;
    
    //呱： 这是用来装 自动生成的虫虫 的列表
    public static List<GameObject> newBugsPrefab = new List<GameObject>();
    

    [SerializeField]  List<InsectData> myFightBugs;

    [Header("敌方UI")]
    [SerializeField] List<TextMeshProUGUI> enemyDatas = new List<TextMeshProUGUI>();
    [SerializeField]List<InsectData> enemyBugs = new List<InsectData>();   

    [Header("虫虫预制体")]
    [SerializeField] GameObject BugPrefab;

    [Header("坐标")] 
    [SerializeField] List<Transform> BugPos;
    
    public static int ActionPoints = DataBroker.actionValue;


    
    
    //呱： 这个用来 根据传入的虫虫数据 来自动生成虫虫
    public void CreateBug()
    {
        List<InsectData> tempBugs = new List<InsectData>(7); 
        Random random = new Random();

        for (int i = 0; i < 7; i++)
        {
            InsectData newBug = new InsectData(); 
            newBug.insectAtk = 2;
            newBug.insectHP = 1;
            newBug.insectLevel = 1;
            newBug.bugType = random.Next(0, 2) == 0 ? E_BugType.A : E_BugType.B;
            tempBugs.Add(newBug);
        }

        DataBroker.Instance.give_datasFromCage(tempBugs);

      
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    /// <summary>
    /// 用来传战斗完以后的虫虫数据
    /// </summary>
    public static void DeliverData(params InsectData[] BugsToDeliver)
    {
        List<InsectData> PostFightBugs = new List<InsectData>();
        for (int i = 0; i < BugsToDeliver.Length; i++)
        {
            PostFightBugs.Add(BugsToDeliver[i]);
        }
        
        //呱：给小鼠老大传虫虫
        DataBroker.Instance.give_datasFromFight(PostFightBugs);
    }
    
    

    
    public void UpdateFightBugAtIndex(int index, InsectData bug)
    {
        //呱： 里面是战斗一的尊严
        #region 给战斗一写的屎山

        if (index == 10)
        {
            index = 0;
            bugs[index].insectHP = 3;
            bugs[index].insectAtk = 2;
            bugs[index].insectLevel = 2;
            bugs[index].isCompound = true;
            
            myFightBugs[index].insectHP = 2;
            myFightBugs[index].insectAtk = 2;
            myFightBugs[index].insectLevel = 2;
            myFightBugs[index].isCompound = true;
            
            myFightBugs[index+2].insectHP = 2;
            myFightBugs[index+2].insectAtk = 2;
            myFightBugs[index+2].insectLevel = 1;
            myFightBugs[index+2].isCompound = false;
            
            fightBugDatas[index].text = $"3\n\n\n2";
            return;
            
        }

        #endregion
       
        
        
        if (index < 0 || index >= fightBugDatas.Count)
        {
            Debug.LogError($"索引 {index} 超出范围，fightBugDatas 数量 = {fightBugDatas.Count}");
            return;
        }
        if (fightBugDatas[index] != null)
        {
            fightBugDatas[index].text = $"{bug.insectHP}\n\n\n{bug.insectAtk}";
            Debug.Log($"更新 fightBugDatas[{index}] 成功");
        }
        else
        {
            Debug.LogError($"fightBugDatas[{index}] 为 null，请检查 Inspector 赋值");
        }
    }
    
    public void UpdateEnenmyBugAtIndex(int index, InsectData enemyBug)
    {
        if (enemyDatas == null || index < 0 || index >= enemyDatas.Count)
        {
            Debug.LogError($"索引 {index} 超出 enemyDatas 范围");
            return;
        }
        if (enemyDatas[index] != null)
        {
            enemyDatas[index].text = $"{enemyBug.insectHP}\n\n\n{enemyBug.insectAtk}";
            Debug.Log($"更新 enemyDatas[{index}] 成功");
        }
    }
    
    void Start()
    {
        // 确保 myFightBugs 有足够容量
        while (myFightBugs.Count < bugs.Count)
        {
            myFightBugs.Add(null);
        }

        int i = 0;
        foreach (InsectData bug in bugs)
        {
            if (i < tagDatas.Count)
                tagDatas[i].text = $"{bug.insectHP}  {bug.insectAtk}";
            myFightBugs[i] = bugs[i];   
            // 改用 fightBugDatas 显示战斗数据
            if (i < fightBugDatas.Count)
                fightBugDatas[i].text = $"{bug.insectHP}\n\n\n{bug.insectAtk}";
            i++;
        }
    }

   
  

    public void UpdateMyFightBugs(List<InsectData> newMyBugs)
    {
        myFightBugs.Clear();
        myFightBugs.AddRange(newMyBugs);


        for (int i = 0; i < myFightBugs.Count && i < fightBugDatas.Count; i++)
        {
            if (fightBugDatas[i] != null)
            {
                string newText = $"{myFightBugs[i].insectHP}\n\n\n{myFightBugs[i].insectAtk}";
                fightBugDatas[i].text = newText;
                Debug.Log($"✅ 更新 fightBugDatas[{i}] 成功，新文本：{newText}");
                // 额外输出该 UI 组件的 GameObject 名称，方便确认绑定对象


            }
        }

        // 额外输出当前 myFightBugs 中每只虫子的实际 HP 和 ATK，确认数据修改是否成功
        for (int i = 0; i < myFightBugs.Count; i++)
        {
            if (myFightBugs[i] != null)
            {
                Debug.Log($"虫子[{i}] 名称：{myFightBugs[i].name}, HP={myFightBugs[i].insectHP}, ATK={myFightBugs[i].insectAtk}");
            }
            else
            {
                Debug.LogWarning($"虫子[{i}] 为 null");
            }
        }
     
    }

    /// <summary>
    /// 设置敌方虫子数据并刷新显示
    /// </summary>
    public void SetEnemyBugs(List<InsectData> enemies)
    {
        enemyBugs.Clear();
        enemyBugs.AddRange(enemies);
        UpdateEnemyDisplay();
    }

    /// <summary>
    /// 刷新所有显示（己方和敌方）- 仅用于战斗结束后同步笼子数据
    /// </summary>
    public void UpdateAllDisplay()
    {
        // 刷新己方笼子数据（tagDatas）
        int i = 0;
        foreach (InsectData bug in bugs)
        {
            if (bug.insectHP <= 0)
            {
                bug.gameObject.SetActive(false);
            }
            if (i < tagDatas.Count)
                tagDatas[i].text = $"{bug.insectHP}  {bug.insectAtk}";
            i++;
        }
        for (; i < tagDatas.Count; i++)
        {
            tagDatas[i].text = "";
        }

        // 刷新敌方
        UpdateEnemyDisplay();
    }

    void UpdateEnemyDisplay()
    {
        int i = 0;
        foreach (InsectData bug in enemyBugs)
        {
            if (bug.insectHP <= 0)
            {
                bug.gameObject.SetActive(false);
            }

            if (i < enemyDatas.Count)
            {
                enemyDatas[i].text = $"{bug.insectHP}\n\n\n{bug.insectAtk}"; 
                enemyBugs[i].insectHP = bug.insectHP;
                enemyBugs[i].insectAtk = bug.insectAtk;
            }
                
                
            i++;
        }
        for (; i < enemyDatas.Count; i++)
        {
            enemyDatas[i].text = "";
        }
    }
}