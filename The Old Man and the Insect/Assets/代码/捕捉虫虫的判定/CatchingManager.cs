using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private bool cancontinue=true;
    
    public string successful = "success";
    public string failure = "fail";
    public TextMeshProUGUI counting;
    public GameObject hintobj;
    public Hint hint;
    public bool startCount = false;
    [Header("限时")]
    public float count;
    private float _count;
    public GameObject currentBug;
    public GameObject panel;
    public int SuccessCount;
    public int failureCount;
    
    private bool doUpdate = true;
    public List<TextMeshProUGUI> hp = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> atk = new List<TextMeshProUGUI>();
    private List<GameObject> currentBugs;

    public Button switchcase;
    // 确保场景中只有一个 CatchingManager 实例
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
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
                counting.text = "Count: " + _count.ToString();
            }
            else
            {
               
                startCount = false;
                catchBugDecision.StartCatch = false;
                catchBugDecision.canCatchBug = false;
                counting.text = "Count finished!";
               
                StartCoroutine(catchBugDecision.waitToClose(1f));
                counting.gameObject.SetActive(false);
                hint.ShowHint(failure);
                failureCount++;
            }
        }

        if (SuccessCount >= 3)
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
            _count = count;
            catchBugDecision.BugToCatch = bug;
            catchFather.SetActive(true);
            catchBugDecision.StartCatchBug();
            counting.gameObject.SetActive(true);
            currentBug = bug;
        }
        else
        {
            hint.ShowHint(failure);
        }
    }

    public void success()
    {
        BugToCatch bugToCatch = currentBug.GetComponent<BugToCatch>();
       startCount = false;
       hint.ShowHint(successful);
       _count = count;
       counting.gameObject.SetActive(false);
       SuccessCount++;
       panel.SetActive(true);
       hp[0].text = "HP: " + bugToCatch.insectHP;
       atk[0].text = "atk:" + bugToCatch.insectAtk;
    }

    public void switchscene()
    {
        Transition.Instance.SwitchSceneWithFade("HuTong1");
        Bag.canOpenBag = false;
    }

    IEnumerator givebug()
    {
        yield return new WaitForSeconds(1f);
         
        currentBugs = new List<GameObject>(GameObject.FindGameObjectsWithTag("EnemyBug"));
        panel.SetActive(true);
        for(int i=0;i<currentBugs.Count;i++)
        {
            InsectData data= currentBugs[i].GetComponent<BugToCatch>();
            data.gameObject.SetActive(false);
            hp[i].text = "hp:" + data.insectHP;
            atk[i].text = "atk:" + data.insectAtk;
        }
        switchcase.gameObject.SetActive(true);
        cancontinue=false;
        doUpdate = false;
    }
}
