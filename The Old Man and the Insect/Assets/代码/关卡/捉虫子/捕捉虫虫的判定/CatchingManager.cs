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
    public GameObject panel;
    public int SuccessCount;
    public int failureCount;
    private bool canshowsuccess=true;
    private bool doUpdate = true;
    public List<TextMeshProUGUI> hp = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> atk = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> name = new List<TextMeshProUGUI>();
    private List<GameObject> currentBugs;
    
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
                hint.ShowHint(failure);
                StartCoroutine(cancatching());
                failureCount++;
            }
        }

        if (SuccessCount >= 3 && DataBroker.catchTime==0 && canshowsuccess)
        {
            canshowsuccess = false;
            hint.ShowHint("其他虫虫跑掉了！");
            cancontinue = false;
            // === 修改：不再单独放行，改为统一结算 ===
            SettleAllBugs();
            // =======================================
        }
    
        if (SuccessCount >= 4 && DataBroker.catchTime > 0 && canshowsuccess)
        {
            canshowsuccess = false;
            hint.ShowHint("其他虫虫跑掉了！");
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
            hint.ShowHint(failure);
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
        hint.ShowHint(successful);
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
        hint.ShowHint("抓住了剩下的虫虫！");
    }
    // ====================================

    // === 新增：统一结算方法 ===
    void SettleAllBugs()
    {
        if (hasSettled) return;
        hasSettled = true;
        
        int availableSlots = 8 - CageManager.Instance.checkcount();
        if (caughtBugs.Count > availableSlots)
        {
            hint.ShowHint("虫虫数量已达上限（8只）无法获得更多");
            caughtBugs.RemoveRange(availableSlots, caughtBugs.Count - availableSlots);
        }
        
        if (caughtBugs.Count > 0)
        {
            panel.SetActive(true);
            for (int i = 0; i < caughtBugs.Count && i < hp.Count; i++)
            {
                if (caughtBugs[i].insectId != 0)
                {
                    hp[i].text = "生命值：" + caughtBugs[i].insectHP;
                    atk[i].text = "攻击力：" + caughtBugs[i].insectAtk;
                    name[i].text = caughtBugs[i].Name;
                    DataBroker.Instance.give_dataFromCatch(caughtBugs[i]);
                    caughtBugs[i].gameObject.SetActive(false);
                }
            }
        }
        
        canswitch = true;
        cancontinue = false;
        doUpdate = false;
    }
    // ==========================
}