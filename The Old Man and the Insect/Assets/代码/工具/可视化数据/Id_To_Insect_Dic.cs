    using System;
    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Id_To_Insect_Dic:MonoBehaviour
{   
    
    public static Dictionary<int, InsectDataSO> IdToInsectDic = new Dictionary<int, InsectDataSO>();
    public List<InsectDataSO> InsectDataList = new List<InsectDataSO>();

    private void Awake()
    {   
        IdToInsectDic.Clear();
        for (int i = 1; i <= InsectDataList.Count; i++)
        {
            IdToInsectDic.Add(i, InsectDataList[i-1]);
        }
    }
    
}
