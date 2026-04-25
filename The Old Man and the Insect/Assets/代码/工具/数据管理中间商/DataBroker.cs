using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class DataBroker
{
    // 单例实例
    private static readonly DataBroker _instance = new DataBroker();
    
    
    private DataBroker()
    {
    }

    //呱 ： 注意 默认值 是输掉战斗拉
    public static bool WinGame2;
    public static bool WinGame3;
    
    public static int reputation ;//声望
    //严肃背诵单词
    public List<InsectData> datasToCatch1 = new List<InsectData>();
    
    private static int _actionValue = 2;
    public static int actionValue
    {   
        get { return _actionValue; }
        set{_actionValue = value;}
        
    }
    //对不起我真不知道行动值怎么取
    //主人可以在属性里写方法直接与自己那边的变量同步。
    
    
    
    public static DataBroker Instance => _instance;
    public List<InsectData> datasFromCage = new List<InsectData>();//这个是鼠给呱呱的
    
    //呱：用于养成的经验值！！！
    public  static int experience = 0;//malooooooooo！
    
    
    
    public List<InsectData> datasFromFight = new List<InsectData>();//这个是呱呱给鼠的
    //目前设想为每次传递清空list里的原有数据，只作为中间商传递
    
    
    public void give_datasFromCage(List<InsectData> datas)
    {   
        datasFromCage.Clear();

       
        datasFromCage = new List<InsectData>(datas);
        
     
        
            

        Debug.Log("同步");

        for (int i = datasFromCage.Count - 1; i >= 0; i--)
        {   
            
            if (datasFromCage[i].insectId == 0)
            {
                datasFromCage.RemoveAt(i);
              
                
            }
        }
        
      
        for (int i = 0; i < datasFromCage.Count; i++)
        {
            Debug.Log("当前来自笼子的数据种类为"+datasFromCage[i].bugType);
        }
        
    }

    
    public void give_datasFromFight(List<InsectData> datas)
    {
        datasFromFight.Clear();
        if (datas != null && datas.Count > 0)
        {
            datasFromFight = new List<InsectData>(datas);
        }

        // 必须倒序遍历！！！遍历中添加元素不会卡死
        for (int i = datasFromFight.Count - 1; i >= 0; i--)
        {
            InsectData bug = datasFromFight[i];
            
            // 只处理死掉的虫子
            if (bug.insectHP <= 0)
            {
              
                if (bug.insectLevel == 1)
                {
                    bug.GetSoData(Id_To_Insect_Dic.IdToInsectDic[bug.insectId]);
                    Debug.Log(bug.insectId);
                }
               
                else if (bug.insectLevel == 2)
                {
                   
                    bug.GetSoData(Id_To_Insect_Dic.IdToInsectDic[bug.insectId]);
                    bug.insectLevel = 1;

                    
                    GameObject newObj = GameObject.Instantiate(bug.gameObject);
                    InsectData newBug = newObj.GetComponent<InsectData>();
                    newBug.GetSoData(Id_To_Insect_Dic.IdToInsectDic[bug.insectId]);
                    newBug.insectLevel = 1;

                    
                    datasFromFight.Add(newBug);
                }
            }
        }
        Debug.Log("战斗后虫虫数量为"+datasFromFight.Count);
        CageManager.Instance.ReplaceInsects(datasFromFight);
    }

    public void clearAllBroker()
    {
        datasFromCage.Clear();
        datasFromFight.Clear();
    }

    public void give_dataFromCatch(InsectData data)
    {
            CageManager.Instance.AddInsect(data);
            Debug.Log("抓住的id为"+data.insectId);
            give_datasFromCage(CageManager.Instance.insectDataList);//抓虫专用
            
       
       
    }

}
