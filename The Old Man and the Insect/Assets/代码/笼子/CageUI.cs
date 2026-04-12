using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CageUI : MonoBehaviour
{
    private static CageUI _instance; // 静态变量，用于保存唯一实例
    
    public Button slot;

    public int slotCount = 20;

    public GameObject levelUpUI;

    public Text atk;
    public Text experience;
    public Text hp;

    public Button hpUpbtn;
    public Button atkUpbtn;
    // 私有构造函数，防止外部直接调用构造函数
    private CageUI() { }

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
    {    Image image=this.GetComponent<Image>();
        Color color = image.color;
        color.a = 1f;
        image.color = color;
        for (int i = 0; i < slotCount; i++)
        {
            Button newslot = Instantiate(slot, this.transform);
            newslot.transform.SetParent(this.transform);
            CageSlot cageSlot = newslot.GetComponent<CageSlot>();
            cageSlot.slotID = i;
            cageSlot.Data = new InsectData();
            cageSlot.Data.insectId = 0; // 0代表空
            CageManager.Instance.slotList.Add(newslot);
            //生成背包格子
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            setInactive();//非常神秘，我用escape退出不了，其他键就可以，先放着
            //unity你是对我的esc有什么意见吗
            Debug.Log("esc");
            DataBroker.Instance.datasFromCage = CageManager.Instance.insectDataList;
            //在关闭培养界面时同步数据给中间商。
        }
        
    }

    public void slotOnClick() // 处理点击逻辑
    {
        levelUpUI.SetActive(true);
        CheckExpAndLevel();
        atk.text = CageManager.Instance.currentChosenData.insectAtk.ToString();
        hp.text = CageManager.Instance.currentChosenData.insectHP.ToString();
        Debug.Log(CageManager.Instance.currentChosenData.insectAtk.ToString());
        experience.text ="exp:"+DataBroker.experience.ToString();
    }

    public void setAct() { 
        this.gameObject.SetActive(true);
        PlayerMove.canMove = false;
    }
    public void setInactive() { 
        this.gameObject.SetActive(false);
        PlayerMove.canMove = true;
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
        if (CageManager.Instance.currentChosenData.insectLevel>DataBroker.experience)
        {
            atkUpbtn.gameObject.SetActive(false);
        }
        else
        {
            atkUpbtn.gameObject.SetActive(true);
        }

        if (CageManager.Instance.currentChosenData.insectLevel>DataBroker.experience)
        {
            hpUpbtn.gameObject.SetActive(false);
        }
        else
        {
            hpUpbtn.gameObject.SetActive(true);
        }
    }//经验不够就不给升级按钮
}