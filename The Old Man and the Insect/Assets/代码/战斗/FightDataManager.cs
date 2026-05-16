using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = System.Random;

public class FightDataManager : MonoBehaviour
{
    [Header("己方UI")]
    [SerializeField] List<TextMeshProUGUI> tagDatas = new List<TextMeshProUGUI>();   
    [SerializeField]  List<GameObject>  myBugs = new List<GameObject>();
    [SerializeField]public List<TextMeshProUGUI> fightBugDatas = new List<TextMeshProUGUI>(); 
    
    [Header("己方虫虫名字")]
    [SerializeField]  List<TextMeshProUGUI> myBugNames = new List<TextMeshProUGUI>();
    
    
    //呱： 这是鼠鼠主人给的 虫虫数据
    /// <summary>
    /// 传入的虫虫数据
    /// </summary>
    [SerializeField] List<InsectData> bugs =new List<InsectData>(DataBroker.Instance.datasFromCage) ;
    
    //呱： 这是用来装 自动生成的虫虫 的列表
    public static List<GameObject> newBugsPrefab = new List<GameObject>();
    

    [SerializeField]  List<InsectData> myFightBugs;

    [Header("敌方UI")]
    [SerializeField] List<TextMeshProUGUI> enemyDatas = new List<TextMeshProUGUI>();
    [SerializeField]List<InsectData> enemyBugs = new List<InsectData>();   


    [Header("待机虫图片")]
    [SerializeField] Sprite[] IdleBugSprites= new Sprite[6];
    
    [Header("战斗虫图片")] 
    [SerializeField] Sprite[] FightBugSprites = new Sprite[6];
    
    public static int ActionPoints = DataBroker.actionValue;

    private static List<InsectData> DeliverBugs = new List<InsectData>();
    
    
    //呱： 这个用来 根据传入的虫虫数据 来自动生成虫虫
    public void TestBug()
    {
        List<InsectData> tempBugs = new List<InsectData>(8); 
        Random random = new Random();

        for (int i = 0; i < 8; i++)
        {
            InsectData newBug = new InsectData();
            if (i == 0)
            {
                newBug.bugType = E_BugType.A;
                newBug.insectHP = 2;
                newBug.insectAtk = 2;
                newBug.insectLevel = 2;
                newBug.isCompound = true;
            }
            else if (i == 1)
            {
                newBug.bugType = E_BugType.B;
                newBug.insectHP = 2;
                newBug.insectAtk = 2;
                newBug.insectLevel = 1;
                newBug.isCompound = false;
            }
            else
            {
                switch (random.Next(1, 7))
                {
                    case 1:
                        newBug.insectAtk = 1;
                        newBug.insectHP = 2;
                        newBug.insectLevel = 1;
                        newBug.bugType = E_BugType.A;
                        break;
                    case 2:
                        newBug.insectAtk = 2;
                        newBug.insectHP = 2;
                        newBug.insectLevel = 1;
                        newBug.bugType = E_BugType.B;
                        break;
                    
                    case 3:
                        newBug.insectAtk = 3;
                        newBug.insectHP = 1;
                        newBug.insectLevel = 1;
                        newBug.bugType = E_BugType.C;
                        break;
                    case 4:
                        newBug.insectAtk = 1;
                        newBug.insectHP = 4;
                        newBug.insectLevel = 1;
                        newBug.bugType = E_BugType.D;
                        break;
                    case 5:
                        newBug.insectAtk = 2;
                        newBug.insectHP = 3;
                        newBug.insectLevel = 1;
                        newBug.bugType = E_BugType.E;
                        break;
                    case 6:
                        newBug.insectAtk = 4;
                        newBug.insectHP = 1;
                        newBug.insectLevel = 1;
                        newBug.bugType = E_BugType.F;
                        break;
                        
                    
                }
            }
       
            tempBugs.Add(newBug);
        }

        //DataBroker.Instance.give_datasFromCage(tempBugs);
        //InitMyBugsFromData(DataBroker.Instance.datasFromCage);
        InitMyBugsFromData(tempBugs);
    }

    public void InitMyBugsFromData(List<InsectData> experimentData)
    {
        for (int i = 0; i < experimentData.Count; i++)
        {
            Debug.Log(experimentData.Count);
            DeliverBugs.Add(experimentData[i]);
        }

        Debug.Log("——准备初始化虫虫——");
        for (int i = 0; i < experimentData.Count; i++)
        {
            Debug.Log("当前虫虫种类为"+experimentData[i].bugType);
        }
        if (experimentData.Count < 8)
        {
            int temp = myBugs.Count - experimentData.Count;
            for (int j = 0; j < temp; j++)
            {
                //呱：卧槽好牛逼 结尾索引式
                Destroy(myBugs[myBugs.Count-1-j]);
            }
        }
        for (int i = 0; i < experimentData.Count; i++)
        {
            Debug.Log(i);
            GameObject FightBug = myBugs[i].transform.GetChild(0).gameObject;
            InsectData FightBugData = FightBug.GetComponent<InsectData>();
            
            Debug.Log(FightBug.name);
           
            GameObject IdleBug = myBugs[i].transform.GetChild(1).gameObject;
            IdleBug.transform.localScale = new Vector3(0.334212452f,0.334212452f,0.334212452f);
            InsectData IdleBugData = IdleBug.GetComponent<InsectData>();
            Debug.Log(IdleBug.name);

            FightBugData.insectHP = experimentData[i].insectHP;
            FightBugData.insectAtk = experimentData[i].insectAtk;
            FightBugData.insectLevel = experimentData[i].insectLevel;
            FightBugData.bugType = experimentData[i].bugType;
            
            
            IdleBugData.insectHP = experimentData[i].insectHP;
            IdleBugData.insectAtk = experimentData[i].insectAtk;
            IdleBugData.insectLevel = experimentData[i].insectLevel;
            IdleBugData.bugType = experimentData[i].bugType;

            // 根据品种设置图片
            SpriteRenderer Idle_SR = IdleBug.GetComponent<SpriteRenderer>();
            SpriteRenderer Fight_SR = FightBug.GetComponent<SpriteRenderer>();
        
            Debug.Log(FightBugData.bugType.ToString());
                switch (FightBugData.bugType)
                {
                    case  E_BugType.A:
                        Debug.Log("A虫虫图片准备中");
                        Idle_SR.sprite = IdleBugSprites[0];
                        Fight_SR.sprite = FightBugSprites[0];
                        
                        break;
                    
                    case  E_BugType.B:
                        Debug.Log("B虫虫图片准备中");
                        Idle_SR.sprite = IdleBugSprites[1];
                        Fight_SR.sprite = FightBugSprites[1];
                        break;
                    
                    case  E_BugType.C:
                        Debug.Log("C虫虫图片准备中");
                        Idle_SR.sprite = IdleBugSprites[2];
                        Fight_SR.sprite = FightBugSprites[2];
                        break;
                    
                    case  E_BugType.D:
                        Debug.Log("D虫虫图片准备中");
                        Idle_SR.sprite = IdleBugSprites[3];
                        Fight_SR.sprite = FightBugSprites[3];
                        break;
                    
                    case  E_BugType.E:
                        Debug.Log("E虫虫图片准备中");
                        Idle_SR.sprite = IdleBugSprites[4];
                        Fight_SR.sprite = FightBugSprites[4];
                        break;
                    
                    case  E_BugType.F:
                        Debug.Log("F虫虫图片准备中");
                        Idle_SR.sprite = IdleBugSprites[5];
                        Fight_SR.sprite = FightBugSprites[5];
                        break;
                }

             

            // 刷新对应的 UI（fightBugDatas）
            if (i < fightBugDatas.Count && fightBugDatas[i] != null)
            {
                fightBugDatas[i].text = $"{FightBugData.insectHP}\n\n\n{FightBugData.insectAtk}";
            }

            //呱：刷新标签ui
            if (i<tagDatas.Count&&tagDatas[i]!=null)
            {
                tagDatas[i].text = $"{FightBugData.insectHP}\n\n{FightBugData.insectAtk}";
            }

            if (i < myBugNames.Count && myBugNames[i] != null)
            {
                myBugNames[i].text = $"{IdleBugData.Name}";
            }
        }
        
      
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
            Debug.Log("给出虫虫种类为"+BugsToDeliver[i].bugType);
        }
       
        //呱：给小鼠老大传虫虫
        DataBroker.Instance.give_datasFromFight(PostFightBugs);
    }

    public static  void DeliverData()
    {   
        Debug.Log(FightFlowManager.OnGame2);
        Debug.Log(FightFlowManager.OnGame3);
        Debug.Log("传递数量为"+DeliverBugs.Count);
        DataBroker.Instance.give_datasFromFight(DeliverBugs);
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
            
            fightBugDatas[index].text = $"3\n\n2";
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
            fightBugDatas[index].text = $"{bug.insectHP}\n\n{bug.insectAtk}";
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
            enemyDatas[index].text = $"{enemyBug.insectHP}\n\n{enemyBug.insectAtk}";
            Debug.Log($"更新 enemyDatas[{index}] 成功");
        }
        
    }
    
    void Start()
    {
        
        if (FightFlowManager.OnGame1)
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
                    tagDatas[i].text = $"{bug.insectHP}\n\n{bug.insectAtk}";
                myFightBugs[i] = bugs[i];   
                // 改用 fightBugDatas 显示战斗数据
                if (i < fightBugDatas.Count)
                    fightBugDatas[i].text = $"{bug.insectHP}\n\n{bug.insectAtk}";
                i++;
            }
            
        }

        if (FightFlowManager.OnGame2)
        {
            List<InsectData> copy = new List<InsectData>(enemyBugs);
            SetEnemyBugs(copy);
           
            InitMyBugsFromData(DataBroker.Instance.datasFromCage);
        }
        
        if (FightFlowManager.OnGame3)
        {
            List<InsectData> copy = new List<InsectData>(enemyBugs);
            SetEnemyBugs(copy);
           
            //呱：实验用数据
            //TestBug();
            InitMyBugsFromData(DataBroker.Instance.datasFromCage);
        }
        
    }
    
    //呱：战斗2用的
    public void UpdateBugUI(GameObject bugObj)
    {
        for (int i = 0; i < myBugs.Count; i++)
        {
            Transform parent = myBugs[i].transform;
            
            foreach (Transform child in parent)
            {
                if (child.gameObject == bugObj)
                {
                    InsectData data = bugObj.GetComponent<InsectData>();
                    if (data != null && i < fightBugDatas.Count)
                    {
                        fightBugDatas[i].text = $"{data.insectHP}\n\n{data.insectAtk}";
                        
                    }
                    return;
                }
            }
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
                string newText = $"{myFightBugs[i].insectHP}\n\n{myFightBugs[i].insectAtk}";
                fightBugDatas[i].text = newText;
            

            }
        }

        // 额外输出当前 myFightBugs 中每只虫子的实际 HP 和 ATK，确认数据修改是否成功
        for (int i = 0; i < myFightBugs.Count; i++)
        {
            if (myFightBugs[i] != null)
            {
                Debug.Log($"虫子[{i}] 名称：{myFightBugs[i].Name}, HP={myFightBugs[i].insectHP}, ATK={myFightBugs[i].insectAtk}");
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
                tagDatas[i].text = $"{bug.insectHP}\n\n{bug.insectAtk}";
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
                enemyDatas[i].text = $"{bug.insectHP}\n\n{bug.insectAtk}"; 
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