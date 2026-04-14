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
    
    
    public static int reputation ;//声望
    //严肃背诵单词
    
    
    private static int _actionValue;
    public static int actionValue
    {   
        get { return _actionValue; }
        set{_actionValue = value;}
        
    }
    //对不起我真不知道行动值怎么取
    //主人可以在属性里写方法直接与自己那边的变量同步。
    
    
    
    public static DataBroker Instance => _instance;
    public List<InsectData> datasFromCage = new List<InsectData>();//这个是鼠给呱呱的
    public  static int experience = 0;//malooooooooo！
    public List<InsectData> datasFromFight = new List<InsectData>();//这个是呱呱给鼠的
    //目前设想为每次传递清空list里的原有数据，只作为中间商传递
    
    
    public void give_datasFromCage(List<InsectData> datas)
    {   
        datasFromCage.Clear();
        datasFromCage = datas;
        Debug.Log("同步");
    }

    public void give_datasFromFight(List<InsectData> datas)
    {
        datasFromFight.Clear();
        datasFromFight = datas;
        CageManager.Instance.ReplaceInsects(datas);//在呱呱结束战斗之后把数据传给鼠时，会更新笼子里的数据
    }

    public void clearAllBroker()
    {
        datasFromCage.Clear();
        datasFromFight.Clear();
    }

}
