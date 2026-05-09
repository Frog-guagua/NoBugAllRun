using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = System.Object;
using Random = UnityEngine.Random;


//呱： 哦我的天哪我终于决定写这个了……
//    希望我能顺清楚这些逻辑
//    做游戏真开心
public class FightFlowManager : MonoBehaviour
{
    [Header("音效")]
    [Tooltip("对话点击时候的音效")]
    [SerializeField] AudioClip speakEffect1;
    [SerializeField] AudioClip speakEffect2;
    [SerializeField] AudioClip FightEffect;
    [SerializeField] AudioClip bugSing;
    [Tooltip("提示出现时候的音效")]
    [SerializeField] AudioClip hintEffect;
    
    [Header("战斗背景bgm")]
    [Tooltip("就这个战斗爽")]
    [SerializeField] AudioClip FightBGM;
    
    [Header("开の关")]
    public static bool OnGame1;
    public static bool OnGame2;
    public static bool OnGame3;

    [Header("俺の笼子")]
    [SerializeField]  GameObject Cage;
    
    [Header("Manager!集合！")]

    [SerializeField]  GameObject DialogueManager;
    [SerializeField]  GameObject GridManager;
    private GridManager gridManager;
    private DialogueForFight dialogue;
    [SerializeField] GameObject HintManager;
    [SerializeField] GameObject EndHintManager;
    [SerializeField] GameObject DialougeHintManager;
    private Hint hint;
    private Hint endHint;
    private Hint dialogueHint;
    [SerializeField]RoundManager roundManager;
    [SerializeField] GameObject WaitingBugs;
    private WaitingBug waitingBug;
    [SerializeField] FightDataManager fightDataManager;
    
    
    
    private GameObject Mask;
    [SerializeField] GameObject FoucsMask;
    private Transition mask;
    private SpriteRenderer foucaMask_SR;

    [Header("相机抖动")] 
    [SerializeField]  Camera mainCamera;
    private CamaraShake  cameraShake;
    private CameraFocus cameraFocus;
        
    [Header("虫虫们")]
    [SerializeField]List<GameObject> bugs;

    [Header("算盘")] 
    [SerializeField] GameObject abacus;

    [Header("大爷")] [SerializeField] 
    GameObject rival;
    
    [Header("特效")]
    [SerializeField] ParticleSystem particle;
    public static bool onTeachingRound = false;
    public static bool StartFight = false;
    public static int count = 0;

    [Header("结算界面")] 
    [SerializeField] GameObject Win;
    [SerializeField] GameObject Lose;
    [SerializeField] TextMeshProUGUI EndText;
    [SerializeField] TextMeshProUGUI ExText;
    [SerializeField] TextMeshProUGUI RepuText;

    private List<string> WinTest = new List<string>();
    private List<string> LoseTest = new List<string>();
    
    
    private float fadeTime = 1.5f;
    //呱：已经确认好 当堂的游戏类型   游戏流程 只单向执行一次 
    private bool haveCheckedFightType;
    private Grid[] Mygrids;
    private bool BugISOK = false;

    void Start()
    {
        
        #region 准备结算语句

        WinTest.Add("蛐蛐一胜，街坊喝彩，痛快！");
        WinTest.Add("不费吹灰之力，高下立判！");
        WinTest.Add("全程碾压，根本不给对手还手的余地！");
        WinTest.Add("蛐蛐给力，对手直接自闭！");
        
        LoseTest.Add("今日技差一招，来日定再分高下！");
        LoseTest.Add("今天我的蛐蛐好像没睡醒啊…");
        LoseTest.Add("蛐蛐儿：今天胃口不好，先下班了");

        #endregion
        
        waitingBug = WaitingBugs.GetComponent<WaitingBug>();
        cameraFocus = mainCamera.GetComponent<CameraFocus>();
      
        gridManager =  GridManager.GetComponent<GridManager>();
       
        foucaMask_SR = FoucsMask.GetComponent<SpriteRenderer>();
        Color color = foucaMask_SR.color;
        color.a = 0f;
        foucaMask_SR.color = color;
        
        dialogue = DialogueManager.GetComponent<DialogueForFight>();
        hint = HintManager.GetComponent<Hint>();
        endHint = EndHintManager.GetComponent<Hint>();
        dialogueHint = DialougeHintManager.GetComponent<Hint>();
        
        cameraShake = mainCamera.GetComponent<CamaraShake>();
    }

    
    void Update()
    {
        if (global::GridManager.Grids[4].bugOnGrid != null)
        {
            Debug.Log(global::GridManager.Grids[4].bugOnGrid.name);
        }
        else
        {
            Debug.Log("注意了注意了这是没有虫虫！！");
        }

        //呱：节省性能
        if(haveCheckedFightType) return;
        
        
            
        //呱： 用来判断现在是 第几次游戏
        if (OnGame1)
        { 
            onTeachingRound = true;
           StartCoroutine(Game1Flow());
           haveCheckedFightType = true;
           
        }
        else  if (OnGame2)
        {
            onTeachingRound = false;
            StartCoroutine(Game2Flow());
            haveCheckedFightType = true;
        }
        else if (OnGame3)
        {
            onTeachingRound = false;
            StartCoroutine(Game3Flow());
            haveCheckedFightType = true;
        }
    }


    //呱：这是每个战斗都通用的战前准备
    void PrepareForFight()
    {
        #region 禁用物体上的脚本

        //呱：这里想的是禁用 我们的笼子 的点击放大(CageZoom) 那个脚本 
        //   这样就能解决对面大爷在出牌/说话 的时候 我们在这边多动症
        //   给我认真看啊！ 玩游戏的玩家 给我认真 看啊！
        BanCage();

        abacus.GetComponent<Collider2D>().enabled = false;
        mainCamera.GetComponent<CameraFocus>().enabled = false;

       
        
        #endregion
        
        #region 战前氛围准备

       //呱： 音乐播放
        PlayFightBGM();
        
        
        #endregion

    }

    
    //呱： 第一次战斗……！ 大爷强强！！！
    IEnumerator Game1Flow()
    {
        
        
        #region 准备

        PrepareForFight();

        #endregion
        
        //呱：————————————————Round1—————————————————————
        //呱：好的，对手大爷准备对话 
        #region 对话 

        //呱：首先 等到遮幕渐隐完再说话
        yield return new WaitForSeconds(fadeTime);
        yield return StartCoroutine(Speak(2,"怂了吗","快把你的蛐蛐放上来！"));
        
        #endregion

        #region 提示和引导

        //呱： ①引导点击笼子 这时候我们顺便开放笼子权限
        yield return StartCoroutine(ShowHint("看到左下角的<b><color=#335EA4>笼子</color></b>了吗，快点击它！"));
        
        ReleseCage();
        AudioMgr.Instance.PlaySFX(bugSing);
        yield return StartCoroutine(TransportMask());
        
        //呱： ②引导 把虫虫放到正确的地方
        yield return new WaitUntil(() =>CageZoom.CageHasZoomed);
        bugs[1].GetComponent<SpriteRenderer>().color =
            new Color(89/255f, 116/255f, 77/255f, 1f);
        yield return StartCoroutine(ShowHint("<b><color=#335EA4>长按左键</color></b>拖动蛐蛐到象棋格中"));

        
        // 呱：等待玩家放置第一只虫子（count 从 0 变为 1）
        yield return new WaitUntil(() => count > 0);
        yield return StartCoroutine(ShowHint("很好，每回合放置<b><color=#335EA4>一级</color></b>蛐蛐都会消耗<b><color=#335EA4>一点</color></b>行动值\n现在尝试放置另外一只吧"));

        
        //呱：为了防止 两只A虫虫都被抓起来了 我们禁用一下
       
      
   
        // 呱：等待玩家放置第二只虫子（count 从 1 变为 2）
        yield return new WaitUntil(() => count > 1);
        yield return StartCoroutine(ShowHint("拨动<b><color=#335EA4>算盘</color></b>，结束回合"));


      
        
        
        #endregion
        
        #region 结算

        //呱： 打开咱们的算盘
        abacus.GetComponent<Collider2D>().enabled = true;
        yield return new WaitUntil(()=> AbacusAnim.Finsined==true);
        yield return new WaitForSeconds(0.3f);
        
        //呱：放大相机 聚焦在战局上面
        mainCamera.GetComponent<CameraFocus>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        abacus.GetComponent<Collider2D>().enabled = false;
        
        List<InsectData> enemyBugData = new List<InsectData>();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyBug");
        foreach (var enemy in enemies)
        {
            InsectData data = enemy.GetComponent<InsectData>();
            if (data != null) enemyBugData.Add(data);
        }

        FightDataManager fdm = FindObjectOfType<FightDataManager>();
        fdm.SetEnemyBugs(enemyBugData);
        cameraFocus.LetCameraFocus();
        yield return  new WaitForSeconds(0.5f);
        AudioMgr.Instance.PlaySFX(FightEffect);
        yield return gridManager.
            ExecuteBattle(new Vector3(0, 0.3f, 0), new Vector3(0, -0.3f, 0), 1f);

        fdm.UpdateAllDisplay();
        
        bugs[1].GetComponent<SpriteRenderer>().color =
            new Color(1, 1, 1, 1f);
        
        count = 0;
        StartFight = true;
        


        #endregion
        
        //呱：————————————————Round2—————————————————————
        //呱：好的，对手大爷准备  红温+对话

        RoundManager.nowRound = 2;
        BanCage();
        #region 对话
        yield return  new WaitForSeconds(0.3f);
        
        StartCoroutine(EffectHelper.AngryEffect(rival, 2f, 1f));
        yield return StartCoroutine(Speak(0,"可恶，再来！"));
        
        #endregion

        #region 大爷放虫

         waitingBug.BugUp(0,0);
        yield return new WaitForSeconds(0.3f);
        ActionPoint actionPoint = FindObjectOfType<ActionPoint>();
        FightDataManager.ActionPoints = 2;
        actionPoint.UpdatePoints(FightDataManager.ActionPoints);;
        
        
        #endregion

        #region 提示和引导

        yield return StartCoroutine(ShowHint("再次点击笼子"));
        ReleseCage();
        yield return new WaitUntil(() =>CageZoom.CageHasZoomed);
        
        yield return StartCoroutine(ShowHint("这次拖动蛐蛐放置在同一品种的<b><color=#335EA4>后方</color></b>"));
        bugs[1].GetComponent<Collider2D>().enabled = true;



        yield return new  WaitUntil(() => FightDataManager.ActionPoints == 1);
       
        cameraFocus.LetCameraFocus();
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        ps.Play(true);
        yield return StartCoroutine(ShowHint("同一种类的蛐蛐可以在战斗中<b><color=#335EA4>融合</color></b>，以获得更强的效果"));
        yield return new WaitForSeconds(0.1f);
        AbacusAnim.Finsined=false;
        yield return StartCoroutine(ShowHint("拨动算盘，结束回合"));
        
        
       
        
        #endregion
        
        #region 结算

        //呱： 打开咱们的算盘
        yield return new WaitForSeconds(0.5f);
        abacus.GetComponent<Collider2D>().enabled = true;
        
        yield return new WaitUntil(()=> AbacusAnim.Finsined==true);
        yield return new WaitForSeconds(0.3f);
        
        //呱：放大相机 聚焦在战局上面
        mainCamera.GetComponent<CameraFocus>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        abacus.GetComponent<Collider2D>().enabled = false;
        
        enemyBugData = new List<InsectData>();
        enemies = GameObject.FindGameObjectsWithTag("EnemyBug");
        foreach (var enemy in enemies)
        {
            InsectData data = enemy.GetComponent<InsectData>();
            if (data != null) enemyBugData.Add(data);
        }

        fdm = FindObjectOfType<FightDataManager>();
        fdm.SetEnemyBugs(enemyBugData);
        cameraFocus.LetCameraFocus();
        AudioMgr.Instance.PlaySFX(FightEffect);
        
        yield return gridManager.
            ExecuteBattle(new Vector3(0, 0.3f, 0), new Vector3(0, -0.3f, 0), 1f);

        fdm.UpdateAllDisplay();
        yield return StartCoroutine(ShowHint("别担心，战斗下场的蛐蛐会<b><color=#335EA4>重置属性</color></b>回到你的笼子里，\n在下一场战斗中还会见到它的"));
        
        int formal = DataBroker.experience;
        yield return SizeScale(Win);
        UpdateWinEndUI(formal,4);
       
        yield return  new WaitForSeconds(0.5f);
        count = 0;
        StartFight = true;
        


        #endregion
        
        yield return new WaitForSeconds(0.3f);
        cameraFocus.LetCameraFocus();

        #region 传值

        //呱：给小鼠老大传入 战斗后虫虫数据
        bugs[3].GetComponent<InsectData>().insectLevel = 2;
        bugs[3].GetComponent<InsectData>().insectId = 1;
        bugs[5].GetComponent<InsectData>().bugType = E_BugType.B; 
        FightDataManager.DeliverData(bugs[3].GetComponent<InsectData>(),bugs[5].GetComponent<InsectData>());
        
        //呱：给小鼠老大传入 战斗后经验值
        DataBroker.experience += 4;
        
       /* yield return ShowEndHint("战斗胜利");
        yield return new WaitForSeconds(1f);
        yield return ShowEndHint("经验值增加<b><color=#335EA4>4</color></b>点");*/
       
        yield return new WaitForSeconds(2f);
        #endregion

        OnGame1 = false;

        #region 切换场景

         yield return new WaitForSeconds(0.5f);
         AudioMgr.Instance.StopBGM();
         
         Transition.Instance.SwitchSceneWithFade("HuTong1");
       

        #endregion

        yield return null;
    }

    //呱：大爷战斗
    IEnumerator Game2Flow()
    {
        PrepareForFight();
        dialogue.background.SetActive(false);
        ActionPoint actionPoint = FindObjectOfType<ActionPoint>();
        FightDataManager.ActionPoints = DataBroker.actionValue;
        actionPoint.UpdatePoints(FightDataManager.ActionPoints);;
        //呱：————————————————Round1—————————————————————
        #region 上虫

        //呱：把C虫虫放在 第10格
        waitingBug.BugUp(0,9);
        //呱：把D虫虫放在 第9格
        waitingBug.BugUp(1,8);
        
        //呱：实验用 这是用来测试 敌方虫子升级逻辑的
        // waitingBug.BugUp(2,12);

        #endregion
        
        #region 对话

        yield return new WaitForSeconds(fadeTime);
        dialogue.background.SetActive(true);
        yield return Speak(0,"听我一句劝","现在认个栽","这事儿就算过去了");
        ReleseCage();
        
        yield return new WaitUntil(() =>CageZoom.CageHasZoomed);
        yield return new WaitForSeconds(0.5f);
        yield return Speak(0,"哼","<b>有种</b>");
        StartCoroutine(waitingBug.Shake(1, 0.1f, 0));
        StartCoroutine(waitingBug.Shake(1, 0.1f, 1));
        AbacusAnim.Finsined = false;
        int temp = FightDataManager.ActionPoints;
        yield return new WaitUntil(() =>FightDataManager.ActionPoints!=temp);
        #endregion

        #region 结算

   

        abacus.GetComponent<Collider2D>().enabled = true;  
        yield return new WaitUntil(()=> AbacusAnim.Finsined==true);
        while (!AbacusAnim.Finsined)
        {
            if (FightDataManager.ActionPoints == 0)
            {
                BanCage();
            }
        
            yield return new WaitForSeconds(1.5f);
            
        }
        
        
        
        yield return new WaitForSeconds(0.3f);
        AbacusAnim.Finsined = false;
        
        waitingBug.CountMyBugs();
        yield return new WaitForSeconds(0.1f);
        waitingBug.myBugCount = 0;              // 重置计数器
        waitingBug.CountMyBugs();  
        yield return waitingBug.FindRival(9);
        int movedIndex1 = waitingBug.lastMovedGridIndex;
        Debug.Log( waitingBug.GetComponent<WaitingBug>().myBugCount);
        
        
        yield return waitingBug.FindRival(8);
        Debug.Log( waitingBug.GetComponent<WaitingBug>().myBugCount);
        waitingBug.GetComponent<WaitingBug>().myBugCount = 0;
        
        int movedIndex2 = waitingBug.lastMovedGridIndex;
        Debug.Log( movedIndex2);
        
        //呱：放大相机 聚焦在战局上面
        mainCamera.GetComponent<CameraFocus>().enabled = true;
     
        abacus.GetComponent<Collider2D>().enabled = false;
        
        cameraFocus.LetCameraFocus();
        
        AudioMgr.Instance.PlaySFX(FightEffect);
        yield return GetComponent<BattleResover>().BattleResolve();
        if(DataBroker.WinGame2&& GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            //呱：处理胜利结算 UI
            int formal = DataBroker.experience;
            yield return SizeScale(Win);
            UpdateWinEndUI(formal,6);
            DataBroker.experience += 6;
       
            cameraFocus.LetCameraFocus();
            yield return new WaitForSeconds(1.5f);
            OnGame2 = false;
            FightDataManager.DeliverData();
            Transition.Instance.SwitchSceneWithFade("BeforeFight2");
        }

        #endregion
            
        //呱：————————————————Round2—————————————————————

        #region 上虫
        yield return Speak(3,"哟呵","我就不信了","再来！");
        waitingBug.BugUp(2,movedIndex2+4);
        yield return new WaitUntil(() => waitingBug.GetComponent<WaitingBug>().finishComposed == true);
        yield return new WaitForSeconds(2f);
        waitingBug.GetComponent<WaitingBug>().finishComposed=false;
        actionPoint = FindObjectOfType<ActionPoint>();
        FightDataManager.ActionPoints = DataBroker.actionValue;
        actionPoint.UpdatePoints(FightDataManager.ActionPoints);;

        #endregion

        #region 结算

        ReleseCage();
        
        abacus.GetComponent<Collider2D>().enabled = true;
        yield return new WaitUntil(()=> AbacusAnim.Finsined==true);
        while (!AbacusAnim.Finsined)
        {
            if (FightDataManager.ActionPoints == 0)
            {
                BanCage();
            }
        
            yield return new WaitForSeconds(1.5f);
            
        }

        
       
        AbacusAnim.Finsined = false;
        BanCage();
        yield return new WaitForSeconds(0.3f);
        waitingBug.CountMyBugs();
        yield return new WaitForSeconds(0.1f);
        waitingBug.myBugCount = 0;              // 重置计数器
        waitingBug.CountMyBugs();  
        Debug.Log(movedIndex1);

            yield return waitingBug.FindRival(movedIndex1);
            
            movedIndex1 = waitingBug.lastMovedGridIndex;
            Debug.Log(movedIndex1);
        
      
        
        
        Debug.Log(movedIndex2);
       
   
            yield return waitingBug.FindRival(movedIndex2);
            
            movedIndex2 = waitingBug.lastMovedGridIndex;
            Debug.Log(movedIndex2);
        
        //movedIndex2 = waitingBug.lastMovedGridIndex;
        //呱：放大相机 聚焦在战局上面
        mainCamera.GetComponent<CameraFocus>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        abacus.GetComponent<Collider2D>().enabled = false;
  
        cameraFocus.LetCameraFocus();
     
        AudioMgr.Instance.PlaySFX(FightEffect);
        yield return GetComponent<BattleResover>().BattleResolve();

        if ((!DataBroker.WinGame2 )&& GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            //呱：处理失败结算 UI
            int formal = DataBroker.reputation;
            yield return SizeScale(Lose);
            UpdateLoseEndUI(formal,6);
            DataBroker.reputation -= 6;
            cameraFocus.LetCameraFocus();
            yield return new WaitForSeconds(1.5f);
            OnGame2 = false;
            FightDataManager.DeliverData();
            Transition.Instance.SwitchSceneWithFade("BeforeFight2");
        }
        else if(DataBroker.WinGame2&& GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            //呱：处理胜利结算 UI
            int formal = DataBroker.experience;
            yield return SizeScale(Win);
            UpdateWinEndUI(formal,6);
            DataBroker.experience += 6;
       
            cameraFocus.LetCameraFocus();
            yield return new WaitForSeconds(1.5f);
            OnGame2 = false;
            FightDataManager.DeliverData();
            Transition.Instance.SwitchSceneWithFade("BeforeFight2");
        }


        #endregion
        
        //呱：————————————————Round3—————————————————————
     
        #region 上虫
        
        yield return Speak(3,"哟呵","我就不信了","再来蛙！");
        
        actionPoint = FindObjectOfType<ActionPoint>();
        FightDataManager.ActionPoints = DataBroker.actionValue;
        actionPoint.UpdatePoints(FightDataManager.ActionPoints);;

        #endregion

        #region 结算

        ReleseCage();
        AbacusAnim.Finsined = false;
        
        abacus.GetComponent<Collider2D>().enabled = true;
        yield return new WaitUntil(()=> AbacusAnim.Finsined==true);
        while (!AbacusAnim.Finsined)
        {
            if (FightDataManager.ActionPoints == 0)
            {
                BanCage();
            }
        
            yield return new WaitForSeconds(1.5f);
            
        }

        AbacusAnim.Finsined = false;
        BanCage();
        yield return new WaitForSeconds(0.3f);
        waitingBug.CountMyBugs();
        yield return new WaitForSeconds(0.1f);
        Debug.Log(movedIndex1);
        waitingBug.myBugCount = 0;              // 重置计数器
        waitingBug.CountMyBugs();  
            yield return waitingBug.FindRival(movedIndex1);
            
            movedIndex1 = waitingBug.lastMovedGridIndex;
            Debug.Log(movedIndex1);
        
       
        
        
        Debug.Log(movedIndex2);
       
   
            yield return waitingBug.FindRival(movedIndex2);
            
            movedIndex2 = waitingBug.lastMovedGridIndex;
            Debug.Log(movedIndex2);
        
        //movedIndex2 = waitingBug.lastMovedGridIndex;
        //呱：放大相机 聚焦在战局上面
        mainCamera.GetComponent<CameraFocus>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        abacus.GetComponent<Collider2D>().enabled = false;
  
        cameraFocus.LetCameraFocus();
     
        AudioMgr.Instance.PlaySFX(FightEffect);
        yield return GetComponent<BattleResover>().BattleResolve();

        if ((!DataBroker.WinGame2 )&& GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            //呱：处理失败结算 UI
            int formal = DataBroker.reputation;
            yield return SizeScale(Lose);
            UpdateLoseEndUI(formal,6);
            DataBroker.reputation -= 6;
            cameraFocus.LetCameraFocus();
            yield return new WaitForSeconds(1.5f);
            OnGame2 = false;
            FightDataManager.DeliverData();
            Transition.Instance.SwitchSceneWithFade("BeforeFight2");
        }
        else if(DataBroker.WinGame2&& GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            //呱：处理胜利结算 UI
            int formal = DataBroker.experience;
            yield return SizeScale(Win);
            UpdateWinEndUI(formal,6);
            DataBroker.experience += 6;
       
            cameraFocus.LetCameraFocus();
            yield return new WaitForSeconds(1.5f);
            OnGame2 = false;
            FightDataManager.DeliverData();
            Transition.Instance.SwitchSceneWithFade("BeforeFight2");
        }


        #endregion

        while (true)
        {
            
            #region 上虫
        
        
        
        actionPoint = FindObjectOfType<ActionPoint>();
        FightDataManager.ActionPoints = DataBroker.actionValue;
        actionPoint.UpdatePoints(FightDataManager.ActionPoints);;

        #endregion

            #region 结算

        ReleseCage();
        AbacusAnim.Finsined = false;
        
        abacus.GetComponent<Collider2D>().enabled = true;
        while (!AbacusAnim.Finsined)
        {
            if (FightDataManager.ActionPoints == 0)
            {
                BanCage();
            }
        
            yield return new WaitForSeconds(1.5f);
            
        }

        
        yield return new WaitUntil(()=> AbacusAnim.Finsined==true);
        BanCage();
        yield return new WaitForSeconds(0.3f);
        waitingBug.CountMyBugs();
        yield return new WaitForSeconds(0.1f);
        Debug.Log(movedIndex1);

        waitingBug.myBugCount = 0;           
        waitingBug.CountMyBugs();  
            yield return waitingBug.FindRival(movedIndex1);
            
            movedIndex1 = waitingBug.lastMovedGridIndex;
            Debug.Log(movedIndex1);
        
       
        
        
        Debug.Log(movedIndex2);
       
   
            yield return waitingBug.FindRival(movedIndex2);
            
            movedIndex2 = waitingBug.lastMovedGridIndex;
            Debug.Log(movedIndex2);
        
        //movedIndex2 = waitingBug.lastMovedGridIndex;
        //呱：放大相机 聚焦在战局上面
        mainCamera.GetComponent<CameraFocus>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        abacus.GetComponent<Collider2D>().enabled = false;
  
        cameraFocus.LetCameraFocus();
     
        AudioMgr.Instance.PlaySFX(FightEffect);
        yield return GetComponent<BattleResover>().BattleResolve();

        if ((!DataBroker.WinGame2 )&& GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            //呱：处理失败结算 UI
            int formal = DataBroker.reputation;
            yield return SizeScale(Lose);
            UpdateLoseEndUI(formal,6);
            DataBroker.reputation -= 6;
            cameraFocus.LetCameraFocus();
            yield return new WaitForSeconds(1.5f);
            OnGame2 = false;
            FightDataManager.DeliverData();
            Transition.Instance.SwitchSceneWithFade("BeforeFight2");
        }
        else if(DataBroker.WinGame2&& GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            //呱：处理胜利结算 UI
            int formal = DataBroker.experience;
            yield return SizeScale(Win);
            UpdateWinEndUI(formal,6);
            DataBroker.experience += 6;
       
            cameraFocus.LetCameraFocus();
            yield return new WaitForSeconds(1.5f);
            OnGame2 = false;
            FightDataManager.DeliverData();
            Transition.Instance.SwitchSceneWithFade("BeforeFight2");
        }


        #endregion
       
        }
     
       
        yield return null;
    }

    //呱：终于……还是来了吗
    IEnumerator Game3Flow()
    {

        #region 战前准备

        //呱：通用准备
        PrepareForFight();
        
        //呱：禁用右侧两列格子
        RoundManager.BanGrids();
        
        //呱：让对话框背景先消失 需要的时候再出现
        dialogue.background.SetActive(false);
        
        //呱：根据记录的行动点数来初始化 行动点
        ActionPoint actionPoint = FindObjectOfType<ActionPoint>();
        FightDataManager.ActionPoints = DataBroker.actionValue;
        actionPoint.UpdatePoints(FightDataManager.ActionPoints);
        
        //呱：初始化我们的遮挡物的透明度（0.75） 目前是黑色阴影
        Color color = foucaMask_SR.color;
        color.a = 0.75f;
        foucaMask_SR.color = color;
        StartCoroutine(BanZone());

        #endregion
        
        
        //呱：————————————————Round1—————————————————————
        
        #region 上虫

      
        //呱：把D虫虫放在 第9格
        waitingBug.BugUp(0,8);
        //呱：把F虫虫放在 第10格
        waitingBug.BugUp(1,9);

        #endregion
        
        #region 旁人对话

        yield return new WaitForSeconds(1.5f);
        yield return ShowDialogueHint("窃窃私语A:又有好戏看了……");
        yield return new WaitForSeconds(0.5f);
        yield return ShowDialogueHint("李四爷：义父加油，咬死他的虫！");
        yield return new WaitForSeconds(0.5f);
        yield return ShowDialogueHint("窃窃私语B：诶？你们有没有感觉这老人瞧着有些眼熟，\n想不起来在哪见过了……");
        

        #endregion
        
        #region 对话

        
        dialogue.background.SetActive(true);
        yield return Speak(5,"今儿个大家伙儿","都在这儿作证——","我这蛐蛐纯靠养","没喂过半点药","使得全是真本事");
        StartCoroutine(waitingBug.Shake(1, 0.1f, 0));
        StartCoroutine(waitingBug.Shake(1, 0.1f, 1));
        
        ReleseCage();
        
        AbacusAnim.Finsined = false;
        int temp = FightDataManager.ActionPoints;
        yield return new WaitUntil(() =>FightDataManager.ActionPoints!=temp);
        #endregion
        
        #region 结算
        
        //呱：这里考虑到玩家可以不上虫就结算 所以对话一结束就开放了算盘的权限
        abacus.GetComponent<Collider2D>().enabled = true;  
        
        //呱：如果行动点归零 那么将无法打开笼子
        while (!AbacusAnim.Finsined)
        {
            if (FightDataManager.ActionPoints == 0)
            {
                BanCage();
            }
            yield return new WaitForSeconds(1f);
        }
        
        //呱：只要算盘被触发 那么开始结算 
        yield return new WaitUntil(()=> AbacusAnim.Finsined==true);
        yield return new WaitForSeconds(0.3f);
        
        //呱：及时关闭算盘 防止误触
        abacus.GetComponent<Collider2D>().enabled = false;
        AbacusAnim.Finsined = false;
        
        //呱：关掉笼子 防止误触
        BanCage();
        
        //呱：这边先统计一下我方虫子 方便敌方进行数量判断
        //   如果我方没有虫子 那么FindRival内部直接返回 不进行索敌
        waitingBug.myBugCount = 0;              
        waitingBug.CountMyBugs();  
        
        
        //呱：这是让F虫索敌 写的是F虫目前的格子索引 
        yield return waitingBug.FindRival(9);
        //呱：记录下索敌后F虫的位置 （无问题）
        int movedIndexF = waitingBug.lastMovedGridIndex;
        Debug.Log($"这是第一回合：F虫在索敌后的位置为：{movedIndexF}");
        
        
        //呱：这是让D虫索敌 写的是D虫目前的格子索引 
        yield return waitingBug.FindRival(8);
        //呱：记录下索敌后D虫的位置 （无问题）
        int movedIndexD = waitingBug.lastMovedGridIndex;
        Debug.Log($"这是第一回合：D虫在索敌后的位置为：{movedIndexD}");
        
        
        //呱：手动归零 防止数据污染
        waitingBug.GetComponent<WaitingBug>().myBugCount = 0;
        
        
        //呱：放大相机 聚焦在战局上面
        mainCamera.GetComponent<CameraFocus>().enabled = true;
        cameraFocus.LetCameraFocus();
        
        //呱：播放战斗的特效音乐
        AudioMgr.Instance.PlaySFX(FightEffect);
        
        //呱：调用结算函数 进行虫子的位移 结算 胜负判断
        yield return GetComponent<BattleResover>().BattleResolve();
        
        //呱：担心玩家被打死了
        yield return CheckLose();
        
        #endregion

        #region 刷新行动点

        yield return new WaitForSeconds(1.5f);
        
        //呱：刷新行动点
        FightDataManager.ActionPoints = DataBroker.actionValue;
        actionPoint.UpdatePoints(FightDataManager.ActionPoints);

       #endregion

        
        
        
        //呱：————————————————Round2—————————————————————
        RoundManager.ReleaseGrids();
        #region 对话

        //呱:依旧是解禁这个对话背景  开始对话
        dialogue.background.SetActive(true);
        yield return Speak(0,"有能耐","不过这才刚刚开始");
        
        #endregion
        
        #region 上虫

        //呱：把D虫虫放在 上一个D虫虫的身后
        waitingBug.BugUp(2,movedIndexD+4);
        
        //呱：把E虫虫放在 第11格
        waitingBug.BugUp(3,10);

        //呱：这里在E虫虫登场以后 解禁了右边部分的限制
        BugISOK = true;
        RoundManager.ReleaseGrids();
        
        Debug.Log("____________Round2_____________");
        waitingBug.CheckBugOnWhichGrid();
        Debug.Log("_________________________");
        
        //呱：在敌方上虫成功以后 解禁笼子
        ReleseCage();
        #endregion
        
        #region 结算
        
        //呱：解禁算盘 等待结算
        AbacusAnim.Finsined = false;
        abacus.GetComponent<Collider2D>().enabled = true;
        yield return new WaitUntil(() => AbacusAnim.Finsined == true);

        //呱：行动点归零的时候 强制禁用笼子
        while (!AbacusAnim.Finsined)
        {
            if (FightDataManager.ActionPoints == 0)
            {
                BanCage();
            }
        
            yield return new WaitForSeconds(1.5f);
            
        }
        
        //呱：禁用算盘 防止误触
        abacus.GetComponent<Collider2D>().enabled = false;

        //呱：禁用笼子 防止误触
        BanCage();
        
        //呱：依旧统计我方虫子数量 如果归零就不进行索敌了
        yield return new WaitForSeconds(0.3f);
        waitingBug.myBugCount = 0;              
        waitingBug.CountMyBugs();  
        
        //呱：让F虫虫开始索敌 利用上次记录到的F虫索敌后的位置moveIndexF来找到F虫
        yield return waitingBug.FindRival(movedIndexF);
        movedIndexF = waitingBug.lastMovedGridIndex;
        Debug.Log($"这是第二回合：F虫在索敌后的位置为：{movedIndexF}");
  
        //呱：让E虫虫开始索敌 这里因为E是我们这局放上去的 所以我们知道它放在哪里的
        //   依旧记录下E虫虫索敌后的位置 方便后面第二只E虫虫上虫
        int movedIndexE = 0;
        yield return waitingBug.FindRival(10);
        movedIndexE = waitingBug.lastMovedGridIndex;
        Debug.Log($"这是第二回合：E虫在索敌后的位置为：{movedIndexE}");
        
        //呱：让D虫虫开始索敌 
        yield return waitingBug.FindRival(movedIndexD);
        movedIndexD = waitingBug.lastMovedGridIndex;
        Debug.Log($"这是第二回合：D虫在索敌后的位置为：{movedIndexD}");

        
        
        //呱：放大相机 聚焦在战局上面
        mainCamera.GetComponent<CameraFocus>().enabled = true;
        cameraFocus.LetCameraFocus();
        
        //呱：播放战斗音效
        AudioMgr.Instance.PlaySFX(FightEffect);
        
        //呱：调用结算函数进行结算
        yield return GetComponent<BattleResover>().BattleResolve();
        AbacusAnim.Finsined = false;
        
        //呱：担心玩家被打死了
        yield return CheckLose();
        
        #endregion

        #region 刷新行动点

        yield return new WaitForSeconds(1.5f);
        FightDataManager.ActionPoints = DataBroker.actionValue;
        actionPoint.UpdatePoints(FightDataManager.ActionPoints);

        #endregion
        
        
        
        
        
        //呱：————————————————Round3—————————————————————
        
        #region 对话

        //呱：依旧是开始对话
        dialogue.background.SetActive(true);
        yield return Speak(0,"有点东西","还没结束呢！");
        
        #endregion
        
        #region 上虫

      
        //呱：把A虫虫放在 第12格
        waitingBug.BugUp(5,11);
        
        //呱：把E虫虫放在 上一个E虫虫的身后
        waitingBug.BugUp(4,movedIndexE+4);
        //waitingBug.BugUp(4,waitingBug.FindE()+4);
        
        
        Debug.Log("____________Round3_____________");
        waitingBug.CheckBugOnWhichGrid();
        Debug.Log("_________________________");
        
        //呱：在上完虫虫后打开笼子
        ReleseCage();
        
        #endregion
        
        #region 结算
        
        //呱：打开算盘 直到结算 
        abacus.GetComponent<Collider2D>().enabled = true;  
        yield return new WaitUntil(() => AbacusAnim.Finsined == true);
        AbacusAnim.Finsined = false;
        
        //呱：行动点归零就强制关掉笼子
        while (!AbacusAnim.Finsined)
        {
            if (FightDataManager.ActionPoints == 0)
            {
                BanCage();
            }
        
            yield return new WaitForSeconds(1.5f);
            
        }

        //呱：禁用算盘 防止误触
        abacus.GetComponent<Collider2D>().enabled = false;
        
        //呱：禁用笼子 防止误触
        BanCage();
        
        //呱：统计我方虫子数量
        yield return new WaitForSeconds(0.3f);
        waitingBug.myBugCount = 0;              
        waitingBug.CountMyBugs();  
        
        //呱：让A虫虫索敌 A虫虫是这一轮才上的虫 我们是知道A虫虫在第12格上的
        yield return waitingBug.FindRival(11);
        
        //呱：让F虫虫索敌
        yield return waitingBug.FindRival(movedIndexF);
        movedIndexF = waitingBug.lastMovedGridIndex;
       
        //呱：让D虫虫索敌
        yield return waitingBug.FindRival(movedIndexD);
        movedIndexD = waitingBug.lastMovedGridIndex;

        //呱：让E虫虫索敌
        yield return waitingBug.FindRival(movedIndexE);
        movedIndexE = waitingBug.lastMovedGridIndex;
        
       
        //呱：放大相机 聚焦在战局上面
        mainCamera.GetComponent<CameraFocus>().enabled = true;
        cameraFocus.LetCameraFocus();
        
        //呱：播放战斗的特殊音效
        AudioMgr.Instance.PlaySFX(FightEffect);
        
        //呱：利用函数进行结算
        yield return GetComponent<BattleResover>().BattleResolve();
        
        //呱：从这一轮以后 因为敌方不再上新虫虫 所以到这里就可以开始考虑结算
        yield return EndGame();

        #endregion

        #region 刷新行动点

        yield return new WaitForSeconds(1.5f);
        FightDataManager.ActionPoints = DataBroker.actionValue;
        actionPoint.UpdatePoints(FightDataManager.ActionPoints);;

        #endregion
  
        
        
        
        //呱：————————————————Round4—————————————————————
        
        #region 对话

        yield return new WaitForSeconds(fadeTime);
        dialogue.background.SetActive(true);
        yield return ShowDialogueHint("李大爷:老马小心！这虫儿不对劲，开始耍阴的了！");
        yield return new WaitForSeconds(1);
        yield return Speak(3,"什么阴招？","这是我这虫儿斗性上来了","越斗越猛！");
        yield return StartCoroutine(ShowHint("章节Boss特殊机制[<b><color=#335EA4>后期发力</color></b>]：\n从该回合开始，敌方场上的蛐蛐每回合+1点攻击，永久叠加 "));
        
        ReleseCage();
        
        AbacusAnim.Finsined = false;
        
        #endregion

        #region 敌方虫虫增强

        //呱：敌方虫虫攻击力+1
        waitingBug.HightenBugs();
        yield return new WaitForSeconds(1f);

        #endregion
        
        #region 结算
        
        //呱：解禁算盘 随时可以开始结算
        abacus.GetComponent<Collider2D>().enabled = true;  
        yield return new WaitUntil(() => AbacusAnim.Finsined == true);
        AbacusAnim.Finsined = false;
        
        //呱：如果行动点为0 禁用笼子
        while (!AbacusAnim.Finsined)
        {
            if (FightDataManager.ActionPoints == 0)
            {
                BanCage();
            }
        
            yield return new WaitForSeconds(1.5f);
            
        }
        
        //呱：禁用算盘 防止误触
        abacus.GetComponent<Collider2D>().enabled = false;  
        
        //呱：禁用笼子 防止误触
        BanCage();
        
        //呱：对我方虫虫数量进行统计
        yield return new WaitForSeconds(0.3f);
        waitingBug.myBugCount = 0;              
        waitingBug.CountMyBugs();  
        
        //呱：检查每个前排 让每个前排去索敌
        yield return waitingBug.FindRival(8);
        yield return waitingBug.FindRival(11);
        yield return waitingBug.FindRival(10);
        yield return waitingBug.FindRival(9);

        //呱：放大相机 聚焦在战局上面
        mainCamera.GetComponent<CameraFocus>().enabled = true;
        cameraFocus.LetCameraFocus();
        
        //呱：播放战斗的特殊音效
        AudioMgr.Instance.PlaySFX(FightEffect);
        
        //呱：进行结算
        yield return GetComponent<BattleResover>().BattleResolve();
        
        //呱：进行战斗胜负的判断
        yield return EndGame();

        #endregion

        #region 刷新行动点

        yield return new WaitForSeconds(1.5f);
        FightDataManager.ActionPoints = DataBroker.actionValue;
        actionPoint.UpdatePoints(FightDataManager.ActionPoints);

        #endregion
     
        
        
        
        //呱：————————————————Round ? —————————————————————
        //   后面轮次都是一样的 直接循环吧
        while (true)
        {

             #region 敌方虫虫增强

                 //呱：增强是每一轮都要增强的
                 waitingBug.HightenBugs();
                 yield return new WaitForSeconds(1f);

             #endregion
        
             #region 结算
        
                 //呱：算盘解禁
                 abacus.GetComponent<Collider2D>().enabled = true;  
                 yield return new WaitUntil(() => AbacusAnim.Finsined == true);
                 AbacusAnim.Finsined = false;
                 
                 //呱：如果行动点归零 就强制关闭笼子
                 while (!AbacusAnim.Finsined)
                 {
                        if (FightDataManager.ActionPoints == 0)
                        {
                                BanCage();
                        }   
        
                        yield return new WaitForSeconds(1.5f);
            
                 }
        
                 //呱：禁用算盘 防止误触
                 abacus.GetComponent<Collider2D>().enabled = false;  
        
                 //呱：禁用笼子 防止误触
                 BanCage();

                 //呱：统计我方虫虫数量
                 yield return new WaitForSeconds(0.3f);
                 waitingBug.myBugCount = 0;              
                 waitingBug.CountMyBugs();  
      
                //呱：让敌方前排每一个都尝试索敌
                 yield return waitingBug.FindRival(11);
                 yield return waitingBug.FindRival(10);
                 yield return waitingBug.FindRival(9);
                 yield return waitingBug.FindRival(8);

        
                 //呱：放大相机 聚焦在战局上面
                 mainCamera.GetComponent<CameraFocus>().enabled = true;
                 cameraFocus.LetCameraFocus();
        
                 //呱：播放战斗特殊音效
                  AudioMgr.Instance.PlaySFX(FightEffect);
                  
                  //呱：战斗结算
                  yield return GetComponent<BattleResover>().BattleResolve();
                 
                  //呱：进行游戏胜负的判断
                  yield return EndGame();

        #endregion

            #region 刷新行动点

            yield return new WaitForSeconds(1.5f);
            FightDataManager.ActionPoints = DataBroker.actionValue;
            actionPoint.UpdatePoints(FightDataManager.ActionPoints);

            #endregion

        }
        yield return null;
    }
    
    
    //呱：————————————————————————————————以下为工具函数们————————————————————————————————————————
    
    //呱：这是给Boss战打的补丁 这是那个让黑色遮幕显现出来的函数
    IEnumerator BanZone()
    {
        Color color = foucaMask_SR.color;
        color.a = 0.75f;
        foucaMask_SR.color = color;
        yield return new WaitForSeconds(0.7f);
        yield return new WaitUntil(() => BugISOK == true);
        color.a = 0f;
        foucaMask_SR.color = color;
    }
  
    //呱：这是战斗1的时候的Focus遮罩
    IEnumerator TransportMask()
    {
        Color color = foucaMask_SR.color;
        color.a = 0.75f;
        foucaMask_SR.color = color;
        yield return StartCoroutine(Cage.GetComponent<ObjectShake>().Shake(1f,0.25f));
        yield return new WaitForSeconds(0.7f);
        color.a = 0f;
        foucaMask_SR.color = color;
        
    }
    
    
    IEnumerator Speak(int speacial, params string[] content)
    {
        
        for (int i = 0; i < content.Length; i++)
        {
            if (i % 1 == 0)
            {
                AudioMgr.Instance.PlaySFX(speakEffect2);
            }
            else
            {
                AudioMgr.Instance.PlaySFX(speakEffect1);
            }
            
            if (speacial != 0 && (speacial-1) == i)
                cameraShake.ShakeStart(0.5f, 0.5f);

            dialogue.Speak(content[i]);
            yield return dialogue.WaitForClose(); 
        }
    }

    IEnumerator CheckLose()
    {
        if (!DataBroker.WinGame3&&GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            DataBroker.WinGame3= false;
            
            //呱：处理失败结算 UI
            int formal = DataBroker.reputation;
            yield return SizeScale(Lose);
            UpdateLoseEndUI(formal,formal);
            DataBroker.reputation = 0;
            
        
            yield return new WaitForSeconds(1.5f);
            OnGame3 = false;
            FightDataManager.DeliverData();
            yield return new WaitForSeconds(1.5f);
            StopFightBGM();
            Transition.Instance.SwitchSceneWithFade("BeforeBoss");
        }
    }
    
    IEnumerator EndGame ()
    {
        if (!DataBroker.WinGame3&&GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            DataBroker.WinGame3= false;
            
            //呱：处理失败结算 UI
            int formal = DataBroker.reputation;
            yield return SizeScale(Lose);
            UpdateLoseEndUI(formal,formal);
            DataBroker.reputation = 0;
            
        
            yield return new WaitForSeconds(1.5f);
            OnGame3 = false;
            FightDataManager.DeliverData();
            yield return new WaitForSeconds(1.5f);
            StopFightBGM();
            Transition.Instance.SwitchSceneWithFade("BeforeBoss");
        }
        else if(DataBroker.WinGame3&& GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            DataBroker.WinGame3= true;
            
            //呱：处理胜利结算 UI
            int formal = DataBroker.experience;
            yield return SizeScale(Win);
            UpdateWinEndUI(formal,10);
            DataBroker.experience += 10;
            
           
            yield return new WaitForSeconds(1.5f);
            OnGame3 = false;
            FightDataManager.DeliverData();
            yield return new WaitForSeconds(1.5f);
            StopFightBGM();
            
            Transition.Instance.SwitchSceneWithFade("BeforeBoss");
        }
    }
    
    IEnumerator SizeScale(GameObject scaleObj)
    {
        float scaleTime = 0.5f;
        float time = 0;
        
        scaleObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        scaleObj.SetActive(true);
        while (time < scaleTime)
        {
            float percentage = time / scaleTime;
            time += Time.deltaTime;
            scaleObj.transform.localScale = new  Vector3(1*percentage, 1*percentage, 1*percentage);
            yield return null;
        }
        scaleObj.transform.localScale = new Vector3(1,1,1);
    }

    void UpdateWinEndUI(int formal,int add)
    {
        EndText.text= RandomWinEndSenten();
        ExText.text = $"{formal} +<b><color=#335EA4>{add}</color></b>";
        RepuText.text = $"{DataBroker.reputation}";
    }

    string RandomWinEndSenten()
    {
        int random =Random.Range(0,WinTest.Count);
        return WinTest[random];
    }
    
    void UpdateLoseEndUI(int formal,int add)
    {
        EndText.text= RandomLoseEndSenten();
        ExText.text = $"{DataBroker.experience}";
        RepuText.text = $"{formal} -<b><color=#335EA4>{add}</color></b>";
    }

    string RandomLoseEndSenten()
    {
        int random =Random.Range(0,WinTest.Count);
        return LoseTest[random];
    }
    
    public IEnumerator ShowHint(string hintContent)
    {
        AudioMgr.Instance.PlaySFX(hintEffect);
        hint.ShowHint(hintContent);
        yield return hint.WaitForClose();
    }
    
    public IEnumerator ShowEndHint(string hintContent)
    {
        AudioMgr.Instance.PlaySFX(hintEffect);
        endHint.ShowHint(hintContent);
        yield return endHint.WaitForClose();
    }

    public IEnumerator ShowDialogueHint(string dialogueContent)
    {
        AudioMgr.Instance.PlaySFX(hintEffect);
        dialogueHint.ShowHint(dialogueContent);
        yield return  dialogueHint.WaitForClose();
    }

    
    private void PlayFightBGM()
    {
        AudioMgr.Instance.PlayBGM(FightBGM);
    }

    private void StopFightBGM()
    {
        AudioMgr.Instance.StopBGM();
    }

    private void BanCage()
    {
        Cage.GetComponent<CageZoom>().enabled = false;
    }
    private void ReleseCage()
    {
        Cage.GetComponent<CageZoom>().enabled = true;
    }
    
}

public static class EffectHelper
{
    public static IEnumerator AngryEffect(GameObject obj, float duration = 0.5f, float shakeStrength = 0.1f)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Color originalColor = sr.color;
        Vector3 originalPos = obj.transform.position;

        float elapsed = 0f;
        float shakeInterval = 0.05f; // 每次抖动间隔
        float nextShake = 0f;

        while (elapsed < duration)
        {
            // 颜色逐渐变红（红色分量线性增加到1，绿色和蓝色减少）
            float t = elapsed / duration;
            Color targetColor = Color.Lerp(originalColor, Color.red, t);
            sr.color = targetColor;

            // 抖动：每隔一小段时间偏移一次位置（模拟一次抖动）
            if (Time.time >= nextShake)
            {
                nextShake = Time.time + shakeInterval;
                float offsetX = Random.Range(-shakeStrength, shakeStrength);
                float offsetY = Random.Range(-shakeStrength, shakeStrength);
                obj.transform.position = originalPos + new Vector3(offsetX, offsetY, 0);
                // 可以立即复位（或者延迟复位产生多次抖动）
                // 这里让每次抖动后立即复位，但会连续多次
                obj.transform.position = originalPos;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 恢复原始颜色和位置
        sr.color = originalColor;
        obj.transform.position = originalPos;
    }
}
