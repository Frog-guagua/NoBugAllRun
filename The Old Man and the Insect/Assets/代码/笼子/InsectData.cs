using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class InsectData:MonoBehaviour//这个玩意作为正式使用的虫虫数据集
//呱：强 抑郁症模型小鼠老大 强

//主人一会别看我石山看吐了
{
    public int insectId=0;//abcdef对应123456
    public string insectName;
    public int insectHP=0;
    public int insectAtk=0;
    public int insectLevel=1; //呱：虫子的等级 
    
    public string description;
    public Image Image;
    /// <summary>
    /// 用于加小虫的战斗力 的经验值 （消耗）
    /// </summary>
     public int AtkUpConsumpution=1;
    
    /// <summary>
    /// 用于加小虫的生命值 的经验值 （消耗）
    /// </summary>
     public int HpUpConsumption = 1;



    public int UpdateExperience(int consumption)
    {
        DataBroker.experience -= consumption;
        Debug.Log("当前经验为"+DataBroker.experience);
        return DataBroker.experience;
    }
    
    /// <summary>
    /// 给攻击力 加点 的逻辑
    /// </summary>
    /// <returns></returns>
    public float LetAtkUp()
    {

        if (DataBroker.experience >= insectLevel)//你经验多，允许升级
        {
            insectAtk++;
            UpdateExperience(insectLevel);
        }
       
        
        return insectAtk;
    }

    
    /// <summary>
    /// 设置生命升级的逻辑
    /// </summary>
    /// <returns></returns>
    public float LetHPUp()
    {   
        
       
        if (DataBroker.experience >= insectLevel)//你经验多，允许升级
        {
            insectHP++;
            UpdateExperience(insectLevel);
        }
        
        
        
        return insectHP;
    }
    
}
