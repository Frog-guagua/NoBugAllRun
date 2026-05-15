using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bag : MonoBehaviour
{
    public List<Button> slotList = new List<Button>();
    [SerializeField] public static bool canOpenBag =false;

    public GameObject panel;

    public bool isopen = false;

    // 加一个按键冷却，防止连续触发
    private float keyCooldown = 0.2f;
    private float lastPressTime;

    void Start()
    {
        // 初始化格子
        for (int i = 0; i < slotList.Count; i++)
        {
            CageSlot cageSlot = slotList[i].gameObject.GetComponent<CageSlot>();
            cageSlot.Data = new InsectData();
            cageSlot.slotID = i;
            
        }

        // 初始隐藏背包
        panel.SetActive(false);
        isopen = false;
    }

    void Update()
    {
        // 冷却时间没到 → 不响应
        if (Time.time < lastPressTime + keyCooldown)
            return;

        // 用 GetKeyDown 【只响应按下瞬间】，不会连触发
       // if (Input.GetKeyDown(KeyCode.B) && canOpenBag)
       // {   
        //   ;
         //   lastPressTime = Time.time; // 记录按下时间
         //   ToggleBag(); // 开关背包
       // }
    }

    /// <summary>
    /// 统一开关背包
    /// </summary>
    void ToggleBag()
    {
        isopen = !isopen;
        panel.SetActive(isopen);
        PlayerMove.canMove = !isopen;
        // 打开时才刷新数据
        if (isopen)
        {
            refreshData();
        }
    }

    /// <summary>
    /// 刷新背包格子数据
    /// </summary>
    void refreshData()
    {
        if (CageManager.Instance == null)
        {
            Debug.LogError("CageManager 不存在！");
            return;
        }

        for (int i = 0; i < slotList.Count; i++)
        {
            if (i >= CageManager.Instance.insectInCage.Count)
            {
                Debug.LogWarning("格子数量超过笼子容量：" + i);
                continue;
            }

            CageSlot cageSlot = slotList[i].GetComponent<CageSlot>();
            cageSlot.Data = CageManager.Instance.insectInCage[i];

            // 安全刷新图片
            if (cageSlot.Data != null && cageSlot.Data.insectImage != null)
            {
                cageSlot.btn.image.sprite = cageSlot.Data.insectImage;
            }
            else
            {
                // 空数据 → 清空图片
                cageSlot.btn.image.sprite = null;
            }

            // 强制刷新格子显示
            cageSlot.refreshSlot();
        }
    }
}