using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FightDataManager : MonoBehaviour
{
    [Header("己方UI")]
    [SerializeField] List<TextMeshProUGUI> tagDatas = new List<TextMeshProUGUI>();   // 原用于己方
    [SerializeField] List<TextMeshProUGUI> datas = new List<TextMeshProUGUI>();       // 原用于己方
    [SerializeField] List<InsectData> bugs = DataBroker.Instance.datasFromCage;       // 己方虫子

    [Header("敌方UI")]
    
    [SerializeField] List<TextMeshProUGUI> enemyDatas = new List<TextMeshProUGUI>();
    private List<InsectData> enemyBugs = new List<InsectData>();   // 敌方虫子，需要外部赋值

    public static int ActionPoints = DataBroker.actionValue;

    void Start()
    {
        // 初始化己方显示（保持原逻辑）
        int i = 0;
        foreach (InsectData bug in bugs)
        {
            if (bug.insectHP == 0)
            {
                bug.gameObject.SetActive(false);
            }
            if (i < tagDatas.Count)
                tagDatas[i].text = $"{bug.insectHP}    {bug.insectAtk}";
            if (i < datas.Count)
                datas[i].text = $"{bug.insectHP}\n\n\n{bug.insectAtk}";
            i++;
        }
        // 敌方显示需要外部调用 SetEnemyBugs 来初始化
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
    /// 刷新所有显示（己方和敌方）
    /// </summary>
    public void UpdateAllDisplay()
    {
        // 刷新己方
        int i = 0;
        foreach (InsectData bug in bugs)
        {
            if (bug.insectHP <= 0)
            {
                bug.gameObject.SetActive(false);
            }
            if (i < tagDatas.Count)
                tagDatas[i].text = $"{bug.insectHP}    {bug.insectAtk}";
            if (i < datas.Count)
                datas[i].text = $"{bug.insectHP}\n\n\n{bug.insectAtk}";
            i++;
        }
        // 清空多余的己方格子
        for (; i < tagDatas.Count; i++)
        {
            tagDatas[i].text = "";
            datas[i].text = "";
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
                enemyDatas[i].text = $"{bug.insectHP}\n\n\n{bug.insectAtk}";
            i++;
        }
        for (; i < enemyDatas.Count; i++)
        {
           
            enemyDatas[i].text = "";
        }
    }
}