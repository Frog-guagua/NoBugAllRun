using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;




public class InsectData:MonoBehaviour//这个玩意作为正式使用的虫虫数据集
//呱：强 抑郁症模型小鼠老大 强

//主人一会别看我石山看吐了
{   
    private int _insectId=0;

    public string Name
    {
        get
        {
            if (insectId > 0 && insectId < 7)
            {
                return Id_To_Insect_Dic.IdToInsectDic[insectId].insectName;
            }
            return "";
        }
    }
    public int insectId
    {
        get
        {
            if (_insectId == 0)
            {
                switch (_bugType)
                {
                    
                    case E_BugType.A:
                        return 1;
                    case E_BugType.B:
                        return 2;
                    case E_BugType.C:
                        return 3;
                    case E_BugType.D:
                        return 4;
                    case E_BugType.E:
                        return 5;
                    case E_BugType.F:
                        return 6;
                }
            }
            return _insectId;
            
            
            
        }
        set
        {
            _insectId = value;
            switch (value)
            {
                case 1:
                _bugType = E_BugType.A;
                break;
            case 2:
                _bugType = E_BugType.B;
                break;
            case 3:
                _bugType = E_BugType.C;
               break;
            case 4:
                _bugType = E_BugType.D;
                break;
            case 5:
                _bugType = E_BugType.E;
                break;
            case 6:
                _bugType = E_BugType.F;
                break;
                
            }
        }
    }//abcdef对应123456
   
    public int insectHP=0;
    public int insectAtk=0;
    public int insectLevel=1; //呱：虫子的等级 
    public bool isCompound; //呱： 标记 用来确认虫虫是不是合成怪 现用现开 反正默认false
    private E_BugType _bugType=E_BugType.Default;

    public E_BugType bugType
    {
        get
        {
            if (_bugType == E_BugType.Default)
            {
                switch (_insectId)
                {
                    case 1:
                        return E_BugType.A;
                    case 2:
                        return E_BugType.B;
                    case 3:
                        return E_BugType.C;
                    case 4:
                        return E_BugType.D;
                    case 5:
                        return E_BugType.E;
                    case 6:
                        return E_BugType.F;
                    
                }
            }
           
                
            return _bugType;
          
        }
        set
        {   
            _bugType = value;
            switch (value)
            {
                case E_BugType.A:
                    _insectId = 1;
                    break;
                case E_BugType.B:
                    _insectId = 2;
                    break;
                case E_BugType.C:
                    _insectId = 3;
                    break;
                case E_BugType.D:
                    _insectId = 4;
                    break;
                case E_BugType.E:
                    _insectId = 5;
                    break;
                case E_BugType.F:
                    _insectId = 6;
                    break;
            }
        }
    }
    
    public Sprite insectImage;

    public string description
    {
        get
        {
            if (insectId > 0 && insectId < 7)
            {
                return Id_To_Insect_Dic.IdToInsectDic[insectId].description;
            }
            return "";
        }


    }
    
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

    public void GetSoData(InsectDataSO data)
    {
        insectId = data.insectId;
        insectAtk=data.insectAtk;
        insectHP=data.insectHP;
        bugType = data.bugType;
        insectImage = data.insectImage;
      
    }
    
}
