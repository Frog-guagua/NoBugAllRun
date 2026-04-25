using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FlowerManager : MonoBehaviour
{
    private static FlowerManager _instance; // 静态变量，用于保存唯一实例
    public DialogueData flowerData;
    public Hint hint;
    public Button btn;
    public bool SecondIn=false;
    public string hint1;
    private bool canShowHint2=false;
    public string hint2;
    public DialogueData finishflower1;
    public DialogueData finishflower2;
    private bool checkCage=false;
    public DialogueData flowerData2;
    public bool canstartflow=true;
    public bool canleave=false;
    
    [Header("临时")]
    public InsectDataSO insect1;
    public InsectDataSO insect2;
    public InsectDataSO insect3;
    public InsectDataSO insect4;
    // 提供一个公共的静态属性，以便其他类可以访问这个实例
    public static FlowerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("FlowerManager instance is not initialized yet. Please ensure it's set up in the scene.");
            }

            return _instance;
        }
    }

    // 私有构造函数，防止外部直接调用构造函数
    private FlowerManager() { }

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
    }

    // Start is called before the first frame update
    void Start()
    {   
        //临时


       // StartCoroutine(test());



        //临时
    }

    IEnumerator test()
    {   
        
        DataBroker.experience = 99;
        CageUI.Instance.setAct();
        yield return new WaitForSeconds(0.4f);
        CageUI.Instance.setInactive();
        CageManager.Instance.AddInsect(insect1);
        CageManager.Instance.AddInsect(insect2);
        CageManager.Instance.AddInsect(insect3);
        CageManager.Instance.AddInsect(insect4);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canShowHint2 && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(showhint2());
            canShowHint2 = false;
        }

        if (checkCage && CageUI.Instance.thisIsActive == false)
        {
            checkCage = false;
            if (SecondIn == false)
            {
                DialogueManager.Instance.StartDialogue(finishflower1);
            }
            else
            {
                DialogueManager.Instance.StartDialogue(finishflower2);
            }
            canleave = true;
        }
       
    }

    public void StartFlow()
    {
        if (canstartflow)
        {
            if (SecondIn == false)
            {
                DialogueManager.Instance.StartDialogue(flowerData, setshopbtn);
                Debug.Log("hint2");
            }
            else
            {
                DialogueManager.Instance.StartDialogue(flowerData2, setshopbtn);
                
            }

            canstartflow = false;
        }
    }

    public void setshopbtn()
    {
        btn.gameObject.SetActive(true);
    }

    public void clickbtn()
    {
        CageUI.Instance.setAct();
        if (SecondIn == false)
        {
            hint.ShowHint(hint1);
            canShowHint2 = true;
            
        }
        btn.gameObject.SetActive(false);
        checkCage = true;
       
    }

    IEnumerator showhint2()
    {
        yield return new WaitForSeconds(1f);
        hint.ShowHint(hint2);
    }

   
}
