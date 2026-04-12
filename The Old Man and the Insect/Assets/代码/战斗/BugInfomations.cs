using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_BugType
{
    /// <summary>
    /// A品种虫虫 初始生命为2 攻击力为1
    /// </summary>
    A,
    
    /// <summary>
    /// B品种虫虫 初始生命为2 攻击力为2
    /// </summary>
    B,
    
    /// <summary>
    /// C品种虫虫 初始生命为1 攻击力为3
    /// </summary>
    C,
    
    /// <summary>
    /// D品种虫虫 初始生命为4 攻击力为1
    /// </summary>
    D,
    
    /// <summary>
    /// E品种虫虫 初始生命为3 攻击力为2
    /// </summary>
    E,
    
    /// <summary>
    /// F品种虫虫 初始生命为1 攻击力为4
    /// </summary>
    F
}


public class BugInfomations : MonoBehaviour
{
    [Tooltip("填入\"A\"/\"B\"/\"C\"/\"D\"/\"E\"/\"F\"")]
    public  E_BugType bugType;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    
    
    
}
