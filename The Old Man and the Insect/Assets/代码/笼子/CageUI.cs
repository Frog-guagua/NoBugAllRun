using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CageUI : MonoBehaviour
{
    private static CageUI _instance; // 静态变量，用于保存唯一实例
    
    public Button slot;
    public List<GameObject> listToCreate = new List<GameObject>();
    public int slotCount = 20;

    public GameObject levelUpUI;
    public GameObject confirmPanel;
    public TextMeshProUGUI atk;
    public TextMeshProUGUI experience;
    public TextMeshProUGUI hp;
    public Image im1;
    public Image im2;
    public Image im3;
    public Button hpUpbtn;
    public Button atkUpbtn;
    public TextMeshProUGUI description;
    public TextMeshProUGUI name;
    // 私有构造函数，防止外部直接调用构造函数
    private CageUI() { }
    private bool hasBeenActivated = false;

    public bool thisIsActive
    {
        get
        {
            return this.gameObject.activeSelf;
        }
    }
    // 提供一个公共的静态属性，以便其他类可以访问这个实例
    public static CageUI Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("CageUI instance is not initialized yet.");
            }
            
            return _instance;
        }
    }

    // 在Awake方法中初始化实例
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // 保证实例不会被销毁
        }
       
        setInactive();
    }

    // Start is called before the first frame update
    void Start()
    {   
        im1.color = new Color(255,255,255,255);
        im2.color=new Color(255,255,255,255);
        im3.color=new Color(255,255,255,255);
       

        // 生成背包格子 —— 修复版
        for (int i = 0; i < slotCount; i++)
        {
            // 1. 实例化格子（不要直接传父物体）
            Button newslot = Instantiate(slot);

            // 2. 设置父物体 + 保持世界坐标不变 = UI 正确位置
            newslot.transform.SetParent(listToCreate[i].transform, false);

            // 3. 强制重置本地坐标、旋转、缩放（关键！防止错位）
            newslot.transform.localPosition = Vector3.zero;
            newslot.transform.localRotation = Quaternion.identity;
            newslot.transform.localScale = Vector3.one;

            // 4. 赋值格子数据
            CageSlot cageSlot = newslot.GetComponent<CageSlot>();
            cageSlot.slotID = i;
            cageSlot.Data = new InsectData();
            cageSlot.Data.insectId = 0; // 0代表空

            CageManager.Instance.slotList.Add(newslot);
        }

        hasBeenActivated = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(LevelStateManager.canquit)
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (DataBroker.experience > 0)
                {
                    confirmPanel.SetActive(true);
                }

                else
                {

                    QuitCage();

                }
            }
        }
        
    }

    public void slotOnClick() // 处理点击逻辑
    {   
        
        levelUpUI.SetActive(true);
        CheckExpAndLevel();
        atk.text = CageManager.Instance.currentChosenData.insectAtk.ToString();
        hp.text = CageManager.Instance.currentChosenData.insectHP.ToString();
        Debug.Log(CageManager.Instance.currentChosenData.insectAtk.ToString());
        experience.text ="当前经验"+DataBroker.experience.ToString();
        name.text=CageManager.Instance.currentChosenData.Name.ToString();
        description.text=CageManager.Instance.currentChosenData.description.ToString();
    }

    public void setAct() { 
        this.gameObject.SetActive(true);
        
        PlayerMove.canMove = false;
        if (hasBeenActivated)
        {
            for (int i = 0; i < slotCount; i++)
            {
                CageManager.Instance.refreshSlot(i);
            }
        } 
    }
    public void setInactive() { 
        this.gameObject.SetActive(false);
        PlayerMove.canMove = true;
        confirmPanel.SetActive(false);
    }

    public void hpLevelUp()//升级点击事件
    {
       
        this.hp.text=CageManager.Instance.currentChosenData.LetHPUp().ToString();
        Debug.Log( CageManager.Instance.currentChosenData.HpUpConsumption);
        CheckExpAndLevel(); 
        experience.text ="exp:"+DataBroker.experience.ToString();
        
    }
    public void atkLevelUp()
    {
       
        this.atk.text=CageManager.Instance.currentChosenData.LetAtkUp().ToString();
        Debug.Log( CageManager.Instance.currentChosenData.AtkUpConsumpution);
        CheckExpAndLevel(); 
        experience.text ="exp:"+DataBroker.experience.ToString();
    }

    public void CheckExpAndLevel()
    {
        
        // 攻击按钮判断（你的原逻辑）
        if (CageManager.Instance.currentChosenData.insectLevel>DataBroker.experience)
        {
            atkUpbtn.gameObject.SetActive(false);
        }
        else
        {
            atkUpbtn.gameObject.SetActive(true);
        }

        // 血量按钮判断（你的原逻辑，单独写，不被攻击按钮影响）
        if (CageManager.Instance.currentChosenData.insectLevel>DataBroker.experience)
        {
            hpUpbtn.gameObject.SetActive(false);
        }
        else
        {
            hpUpbtn.gameObject.SetActive(true);
        }
    }//经验不够就不给升级按钮

    public void QuitCage()
    {
        setInactive();//非常神秘，我用escape退出不了，其他键就可以，先放着
        //unity你是对我的esc有什么意见吗
        Debug.Log("esc");
        DataBroker.Instance.give_datasFromCage(CageManager.Instance.insectDataList);
        PlayerMove.canMove = true;
        levelUpUI.gameObject.SetActive(false);
        //在关闭培养界面时同步数据给中间商。
    }
}