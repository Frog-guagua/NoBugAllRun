using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// 定义一个 ScriptableObject 类
[CreateAssetMenu(fileName = "InsectData", menuName = "ScriptableObjects/InsectData", order = 1)]
public class InsectDataSO : ScriptableObject
{
    // 在这里声明你的公共变量
    public int insectId = 0; //abcdef对应123456
    public string insectName;
    public int insectHP = 0;
    public int insectAtk = 0;
    public int insectLevel=1; //呱：虫子的等级 

    public string description;
    public Image Image;
}
