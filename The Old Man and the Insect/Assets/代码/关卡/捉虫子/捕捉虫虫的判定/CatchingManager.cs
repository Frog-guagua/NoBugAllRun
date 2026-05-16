using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CatchingManager : MonoBehaviour
{
    public TextMeshProUGUI text0;
    public GameObject bugfather;
    public bool firsttime=true;
    bool firsttimestart=false;
    public AudioClip audio;
    private static CatchingManager _instance;
    public GameObject bugCatcher;
    public GameObject catchFather;
    public static CatchingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CatchingManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("CatchingManager");
                    _instance = singletonObject.AddComponent<CatchingManager>();
                }
            }
            return _instance;
        }
    }
    private CatchBugDecision catchBugDecision;
    public static bool canCatch = true;
    private bool cancontinue=true;
    
    public static event Action cancatch;
    public static event Action nocatch;
    public void callCancatch(){cancatch?.Invoke();}
    public void callNocatch(){nocatch?.Invoke();}
    public string successful = "success";
    public string failure = "fail";
    public TextMeshProUGUI counting;
    public GameObject hintobj;
    public Hint hint;
    public bool startCount = false;
    [Header("限时")]
    public float count;
    private float _count;
    public InsectData currentBug;
    
    public int SuccessCount;
    public int failureCount;
    private bool canshowsuccess=true;
    private bool doUpdate = true;
    
    public GameObject panel;
    private List<TextMeshProUGUI> hp = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> atk = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> name = new List<TextMeshProUGUI>();
    [Header("第一波")] public GameObject panel1;
    public List<TextMeshProUGUI>hp1=new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> atk1=new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> name1=new List<TextMeshProUGUI>();
    public List<SpriteRenderer>sprites1=new List<SpriteRenderer>();
    [Header("第二波")] public GameObject panel2;
    public List<TextMeshProUGUI>hp2=new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> atk2=new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> name2=new List<TextMeshProUGUI>();
    public List<SpriteRenderer>sprites2=new List<SpriteRenderer>();
    private List<GameObject> currentBugs;
    private List<SpriteRenderer> bugs = new List<SpriteRenderer>();
    public List<GameObject> pos = new List<GameObject>();
    public List<GameObject> data1 = new List<GameObject>();
    public List<GameObject> data2 = new List<GameObject>();
    bool canswitch = false;
    
    // === 新增：统一结算列表与状态标志 ===
    private List<InsectData> caughtBugs = new List<InsectData>();
    private bool hasSettled = false;
    // =====================================
    private GameObject tembug;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {   
        AudioMgr.Instance.PlayBGM(audio);
        Bag.canOpenBag = true;
        hint = hintobj.GetComponent<Hint>();
        catchBugDecision=bugCatcher.GetComponent<CatchBugDecision>();
        SuccessCount = 0;
        failureCount = 0;
        doUpdate = true;
        
        // === 新增：每局重置结算状态 ===
        caughtBugs.Clear();
        hasSettled = false;
        // =============================

        List<GameObject> dataList = DataBroker.catchTime == 0 ? data1 : data2;

        if (DataBroker.catchTime == 0)
        {
            panel = panel1;
            atk = atk1;
            hp=hp1;
            name=name1;
            bugs = sprites1;

        }
        else
        {
            panel = panel2;
            atk = atk2;
            hp=hp2;
            name=name2;
            bugs = sprites2;
        }
        
        int count = Mathf.Min(dataList.Count, pos.Count);

        for (int i = 0; i < count; i++)
        {
            GameObject dataInstance = Instantiate(
                dataList[i], 
                pos[i].transform.position, 
                Quaternion.identity,
                pos[i].transform
            );

            dataInstance.GetComponent<SpriteRenderer>().sortingOrder = -100;
            BugToCatch bugToCatch = dataInstance.GetComponent<BugToCatch>();
            if (bugToCatch == null)
            {
                bugToCatch = dataInstance.GetComponentInChildren<BugToCatch>();
            }

            if (bugToCatch == null)
            {
                Debug.LogError($"在生成的对象 {dataInstance.name} 上找不到 {nameof(BugToCatch)} 组件，无法初始化。", dataInstance);
            }
            else
            {
                bugToCatch.Init();
            }
        }
    }

    void Update()
    {
        
        if (startCount)
        {
            if (_count > 0)
            {
                _count-=Time.deltaTime;
                counting.text = "倒计时 " + _count.ToString();
            }
            else
            {
                startCount = false;
                catchBugDecision.StartCatch = false;
                catchBugDecision.canCatchBug = false;
                counting.text = "倒计时结束！";
                StartCoroutine(catchBugDecision.waitToClose(1f));
                counting.gameObject.SetActive(false);
                StartCoroutine(showhint(failure));
                StartCoroutine(cancatching());
                failureCount++;
            }
        }

        if (SuccessCount >= 3 && DataBroker.catchTime==0 && canshowsuccess)
        {
            canshowsuccess = false;
            StartCoroutine(showhint("其他虫虫跑掉了！"));
            cancontinue = false;
            // === 修改：不再单独放行，改为统一结算 ===
            SettleAllBugs();
            // =======================================
        }
    
        if (SuccessCount >= 4 && DataBroker.catchTime > 0 && canshowsuccess)
        {
            canshowsuccess = false;
            StartCoroutine(showhint ("其他虫虫跑掉了！"));
            cancontinue = false;
            // === 修改：不再单独放行，改为统一结算 ===
            SettleAllBugs();
            // =======================================
        }
        
        if(failureCount >= 4 && doUpdate)
        {   
            StartCoroutine(givebug());
        }

        if (Input.GetMouseButtonDown(0) && canswitch)
        {   
            DataBroker.catchTime++;
            Transition.Instance.SwitchSceneWithFade("BeforeCatch");
        }

        if (firsttime && firsttimestart&&Input.GetMouseButtonDown(0))
        {
            firsttimestart = false;
            firsttime = false;
           startCatch(tembug);
            
        }
    }

    public void startCatch(GameObject bug)
    {
        if (cancontinue)
        {
            if (firsttime)
            {
                hint.ShowHint("使用w和d键将虫虫控制在方格范围内");
                firsttimestart = true;
                tembug=bug;
                
            }
            else
            {   
                bug.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 200;

                _count = count;
                catchBugDecision.BugToCatch = bug;
                catchFather.SetActive(true);
                catchBugDecision.targettrs = bug.transform;
                catchBugDecision.StartCatchBug();

                counting.gameObject.SetActive(true);
                currentBug = bug.GetComponent<BugToCatch>();
                canCatch = false;
                bug.SetActive(false);
            }
        }
        else
        {
            StartCoroutine(showhint (failure));
        }
    }

    public void success()
    {   
        startCount = false;
        _count = count;
        counting.gameObject.SetActive(false);
        SuccessCount++;
        StartCoroutine(cancatching());
        
        // === 修改：不再立即弹面板，而是暂存到列表 ===
        StartCoroutine(showhint(successful));
        if (currentBug != null)
        {
            caughtBugs.Add(currentBug);
        }
        // =============================================
    }

    IEnumerator cancatching()
    {
        yield return new WaitForSeconds(0.6f);
        canCatch = true;
    }
    
    public void switchscene()
    {
        DataBroker.catchTime++;
        Transition.Instance.SwitchSceneWithFade("FlowerShop");
        Bag.canOpenBag = false;
    }

    // === 重写：收集剩余虫虫，统一结算 ===
    IEnumerator givebug()
    {
        yield return new WaitForSeconds(2f);
        
        currentBugs = new List<GameObject>(GameObject.FindGameObjectsWithTag("EnemyBug"));
        foreach (var bug in currentBugs)
        {
            BugToCatch btc = bug.GetComponent<BugToCatch>();
            if (btc == null) btc = bug.GetComponentInChildren<BugToCatch>();
            if (btc != null)
            {
                caughtBugs.Add(btc);
                bug.SetActive(false);
            }
        }
        
        SettleAllBugs();
        StartCoroutine(showhint ("抓住了剩下的虫虫！"));
    }
    // ====================================

    // === 新增：统一结算方法 ===
    void SettleAllBugs()
    {
        string names = "";
        bugfather.SetActive(false);
        if (hasSettled) return;
        hasSettled = true;
    
        int availableSlots = Mathf.Max(0, 8 - CageManager.Instance.checkcount());
        bool isOverLimit = caughtBugs.Count > availableSlots;
    
        if (isOverLimit)
        {
            StartCoroutine(showhint("虫虫数量已达上限（8只）无法获得更多"));
        }
    
        if (caughtBugs.Count > 0)
        {
            panel.SetActive(true);
            int showCount = Mathf.Min(caughtBugs.Count, hp.Count);
            int giveCount = Mathf.Min(caughtBugs.Count, availableSlots);
        
            for (int i = 0; i < showCount; i++)
            {
                if (caughtBugs[i].insectId != 0)
                {
                    hp[i].text = caughtBugs[i].insectHP.ToString();
                    atk[i].text = caughtBugs[i].insectAtk.ToString();
                    name[i].text = caughtBugs[i].Name;
                    bugs[i].sprite = caughtBugs[i].insectImage;
                    caughtBugs[i].gameObject.SetActive(false);
                
                    if (i < giveCount)
                    {
                        DataBroker.Instance.give_dataFromCatch(caughtBugs[i]);
                        names += caughtBugs[i].Name + "  ";
                    }
                }
            }
        
            // === 修改：根据是否超上限拼接不同文案 ===
            if (isOverLimit)
            {
                text0.text = "你捉住了：" + names + "（由于笼子只能装8只虫虫，你将多余虫虫放生）";
            }
            else
            {
                text0.text = "你捉住了：" + names;
            }
            // ======================================
        }
    
        canswitch = true;
        cancontinue = false;
        doUpdate = false;
    }
    // ==========================
    public IEnumerator showhint(string text)
    {
        hint.ShowHint(text);
        yield return new WaitForSeconds(1f);
        hint.HideHint();
    }
}