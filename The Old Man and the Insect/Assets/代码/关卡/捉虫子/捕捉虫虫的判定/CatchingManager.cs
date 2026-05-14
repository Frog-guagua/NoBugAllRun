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
    public AudioClip audio;
    // 静态实例变量，用于存储单例
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
                    // 如果没有找到实例，则创建一个新的 GameObject 并附加 CatchingManager
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
    
    private bool doUpdate = true;
    public List<TextMeshProUGUI> hp = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> atk = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> name = new List<TextMeshProUGUI>();
    private List<GameObject> currentBugs;

    public List<GameObject> pos = new List<GameObject>();
    public List<GameObject> data1 = new List<GameObject>();
    public List<GameObject> data2 = new List<GameObject>();
    public Button switchcase;

    
    // 确保场景中只有一个 CatchingManager 实例
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
    
    // Start is called before the first frame update
    void Start()
    {   
        AudioMgr.Instance.PlayBGM(audio);
        Bag.canOpenBag = true;
        hint = hintobj.GetComponent<Hint>();
        catchBugDecision=bugCatcher.GetComponent<CatchBugDecision>();
        SuccessCount = 0;
        failureCount = 0;
        doUpdate = true;

        // 根据 time 值决定生成 data1 还是 data2
        List<GameObject> dataList = DataBroker.catchTime == 0 ? data1 : data2;

        // 确保 pos 有足够的位置来放置数据
        int count = Mathf.Min(dataList.Count, pos.Count);

        for (int i = 0; i < count; i++)
        {
            // 生成在当前场景 + 正确位置 + 不切换场景带走
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

    // Update is called once per frame
    void Update()
    {
        if (startCount)
        {
            // 倒计时逻辑
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

        if (SuccessCount >= 3&&DataBroker.catchTime==0)
        {
            switchcase.gameObject.SetActive(true);
            cancontinue=false;
        }

        if (SuccessCount >= 4 && DataBroker.catchTime > 0)
        {
            switchcase.gameObject.SetActive(true);
            cancontinue=false;
        }
        if(failureCount >= 4&&doUpdate)
        {
            StartCoroutine(givebug());
        }
    }

    public void startCatch(GameObject  bug)
    {
        if (cancontinue)
        {   
            bug.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 100;
            _count = count;
            catchBugDecision.BugToCatch = bug;
            catchFather.SetActive(true);
            catchBugDecision.StartCatchBug();
            counting.gameObject.SetActive(true);
            currentBug = bug.GetComponent<BugToCatch>();
            
            canCatch = false;
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
        if (CageManager.Instance.checkcount() > 7)
        {   
            hint.ShowHint("虫虫数量已达上限（8只）无法获得更多");
            return;
        }
        else
        {
            
            hint.ShowHint(successful);
            panel.SetActive(true);
            hp[1].text = "生命值：" + currentBug.insectHP;
            atk[1].text = "攻击力：" + currentBug.insectAtk;
            name[1].text=currentBug.Name;
        }
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

    IEnumerator givebug()
    {
        yield return new WaitForSeconds(1f);
         
        currentBugs = new List<GameObject>(GameObject.FindGameObjectsWithTag("EnemyBug"));
        int count = 8 - CageManager.Instance.checkcount();
        if (currentBugs.Count > count)
        {   
            hint.ShowHint("虫虫数量已达上限（8只）无法获得更多");
            Debug.Log("装满了不能抓");
            currentBugs.RemoveRange(count, currentBugs.Count - count);
        }
        panel.SetActive(true);
        for(int i=0;i<currentBugs.Count;i++)
        {
            InsectData data= currentBugs[i].GetComponent<BugToCatch>();
            data.gameObject.SetActive(false);
            hp[i].text = "生命值：" + data.insectHP;
            atk[i].text = "攻击力：" + data.insectAtk;
            name[i].text=data.Name;
            DataBroker.Instance.give_dataFromCatch(data);
        }
        switchcase.gameObject.SetActive(true);
        cancontinue=false;
        doUpdate = false;
    }
}
