using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    
    
    private float fadeTime = 1.5f;
    //呱：已经确认好 当堂的游戏类型   游戏流程 只单向执行一次 
    private bool haveCheckedFightType;
    private Grid[] Mygrids;
    

    void Start()
    {
        
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
 
        //呱： 首先是遮幕渐隐
        
       //mask.FadeOut(fadeTime);
        
       //呱： 然后是音乐播放
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
        yield return ShowEndHint("战斗胜利");
        yield return new WaitForSeconds(1f);
        yield return ShowEndHint("经验值增加<b><color=#335EA4>4</color></b>点");
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
        while (!AbacusAnim.Finsined)
        {
            if (FightDataManager.ActionPoints == 0)
            {
                BanCage();
            }
        
            yield return new WaitForSeconds(1.5f);
            
        }
        
        
        yield return new WaitUntil(()=> AbacusAnim.Finsined==true);
        yield return new WaitForSeconds(0.3f);
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
        AbacusAnim.Finsined = false;
        if (!DataBroker.WinGame2&&GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            DataBroker.WinGame2 = false;
            DataBroker.reputation -= 6;
            yield return ShowHint("战斗失败");
            yield return ShowHint("声誉值减少<b><color=#335EA4>6</color></b>点");
            cameraFocus.LetCameraFocus();
            yield return new WaitForSeconds(1.5f);
            OnGame2 = false;
            FightDataManager.DeliverData();
            Transition.Instance.SwitchSceneWithFade("BeforeFight2");
        }
        else if(DataBroker.WinGame2&& GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            DataBroker.experience += 6;
            yield return ShowHint("战斗胜利");
            yield return ShowHint("经验值增加<b><color=#335EA4>6</color></b>点");
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
            DataBroker.reputation -= 6;
            yield return ShowHint("战斗失败");
            yield return ShowHint("声誉值减少<b><color=#335EA4>6</color></b>点");
            cameraFocus.LetCameraFocus();
            yield return new WaitForSeconds(1.5f);
            OnGame2 = false;
            FightDataManager.DeliverData();
            Transition.Instance.SwitchSceneWithFade("BeforeFight2");
        }
        else if(DataBroker.WinGame2&& GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            DataBroker.experience += 6;
            yield return ShowHint("战斗胜利");
            yield return ShowHint("经验值增加<b><color=#335EA4>6</color></b>点");
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
            DataBroker.reputation -= 6;
            yield return ShowHint("战斗失败");
            yield return ShowHint("声誉值减少<b><color=#335EA4>6</color></b>点");
            cameraFocus.LetCameraFocus();
            yield return new WaitForSeconds(1.5f);
            OnGame2 = false;
            FightDataManager.DeliverData();
            Transition.Instance.SwitchSceneWithFade("BeforeFight2");
        }
        else if(DataBroker.WinGame2&& GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            DataBroker.experience += 6;
            yield return ShowHint("战斗胜利");
            yield return ShowHint("经验值增加<b><color=#335EA4>6</color></b>点");
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
            DataBroker.reputation -= 6;
            DataBroker.reputation -= 6;
            yield return ShowHint("战斗失败");
            yield return ShowHint("声誉值减少<b><color=#335EA4>6</color></b>点");
            cameraFocus.LetCameraFocus();
            yield return new WaitForSeconds(1.5f);
            OnGame2 = false;
            FightDataManager.DeliverData();
            Transition.Instance.SwitchSceneWithFade("BeforeFight2");
        }
        else if(DataBroker.WinGame2&& GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            DataBroker.experience += 6;
            yield return ShowHint("战斗胜利");
            yield return ShowHint("经验值增加<b><color=#335EA4>6</color></b>点");
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

    IEnumerator Game3Flow()
    {
       
        PrepareForFight();
        dialogue.background.SetActive(false);
        ActionPoint actionPoint = FindObjectOfType<ActionPoint>();
        FightDataManager.ActionPoints = DataBroker.actionValue;
        actionPoint.UpdatePoints(FightDataManager.ActionPoints);

        //呱：————————————————Round1—————————————————————
        #region 旁人对话

        yield return new WaitForSeconds(1.5f);
        yield return ShowDialogueHint("窃窃私语A:又有好戏看了……");
        yield return new WaitForSeconds(0.5f);
        yield return ShowDialogueHint("李四爷：义父加油，咬死他的虫！");
        yield return new WaitForSeconds(0.5f);
        yield return ShowDialogueHint("窃窃私语B：诶？你们有没有感觉这老人瞧着有些眼熟，\n想不起来在哪见过了……");
        

        #endregion

        #region 上虫

      
        //呱：把D虫虫放在 第9格
        waitingBug.BugUp(0,8);
        //呱：把F虫虫放在 第10格
        waitingBug.BugUp(1,9);

        #endregion
        
        #region 对话

        yield return new WaitForSeconds(fadeTime);
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
        yield return new WaitForSeconds(0.3f);
        waitingBug.CountMyBugs();
        yield return new WaitForSeconds(0.1f);
        waitingBug.myBugCount = 0;              
        waitingBug.CountMyBugs();  
        //呱：这是让F虫索敌
        yield return waitingBug.FindRival(9);
        int movedIndexF = waitingBug.lastMovedGridIndex;
        
        Debug.Log($"这是第一回合：F虫在索敌后的位置为：{movedIndexF}");
        Debug.Log( waitingBug.GetComponent<WaitingBug>().myBugCount);
        
        
        //呱：这是让D虫索敌
        yield return waitingBug.FindRival(8);
        Debug.Log( waitingBug.GetComponent<WaitingBug>().myBugCount);
        waitingBug.GetComponent<WaitingBug>().myBugCount = 0;
        
        int movedIndexD = waitingBug.lastMovedGridIndex;
        Debug.Log($"这是第一回合：D虫在索敌后的位置为：{movedIndexD}");
        
        //呱：放大相机 聚焦在战局上面
        mainCamera.GetComponent<CameraFocus>().enabled = true;
     
        abacus.GetComponent<Collider2D>().enabled = false;
        
        cameraFocus.LetCameraFocus();
        
        AudioMgr.Instance.PlaySFX(FightEffect);
        yield return GetComponent<BattleResover>().BattleResolve();
        AbacusAnim.Finsined = false;
       
        #endregion
       
        yield return new WaitForSeconds(1.5f);
        FightDataManager.ActionPoints = DataBroker.actionValue;
        actionPoint.UpdatePoints(FightDataManager.ActionPoints);;
        //呱：————————————————Round2—————————————————————
        
        #region 上虫

      
        //呱：把E虫虫放在 第11格
        waitingBug.BugUp(3,10);
        
        //呱：把D虫虫放在 上一个D虫虫的身后
        waitingBug.BugUp(2,movedIndexD+4);

        #endregion
        
        #region 对话

        yield return new WaitForSeconds(fadeTime);
        dialogue.background.SetActive(true);
        yield return Speak(0,"有能耐","不过这才刚刚开始");
        
        
        ReleseCage();
        
        AbacusAnim.Finsined = false;
        
        Debug.Log("卡在这儿！");
        
        Debug.Log("五");
        #endregion
        
        #region 结算
        
        AbacusAnim.Finsined = false;
       
        abacus.GetComponent<Collider2D>().enabled = true;
        yield return new WaitUntil(() => AbacusAnim.Finsined == true);
        Debug.Log("——————————算盘自由！————————");
        while (!AbacusAnim.Finsined)
        {
            if (FightDataManager.ActionPoints == 0)
            {
                BanCage();
            }
        
            yield return new WaitForSeconds(1.5f);
            
        }
        
        
        yield return new WaitUntil(()=> AbacusAnim.Finsined==true);
        yield return new WaitForSeconds(0.3f);
        waitingBug.CountMyBugs();
        yield return new WaitForSeconds(0.1f);
        waitingBug.myBugCount = 0;              
        waitingBug.CountMyBugs();  
        yield return waitingBug.FindRival(movedIndexF);
        
        movedIndexF = waitingBug.lastMovedGridIndex;
        Debug.Log($"这是第二回合：F虫在索敌后的位置为：{movedIndexF}");
  
        int movedIndexE = 0;
        yield return waitingBug.FindRival(10);
     
        waitingBug.GetComponent<WaitingBug>().myBugCount = 0;
        
        
        movedIndexE = waitingBug.lastMovedGridIndex;
        Debug.Log($"这是第二回合：E虫在索敌后的位置为：{movedIndexE}");
        
        yield return waitingBug.FindRival(movedIndexD);
     
        waitingBug.GetComponent<WaitingBug>().myBugCount = 0;
        
        movedIndexD = waitingBug.lastMovedGridIndex;
        Debug.Log($"这是第二回合：D虫在索敌后的位置为：{movedIndexD}");

        
        
        //呱：放大相机 聚焦在战局上面
        mainCamera.GetComponent<CameraFocus>().enabled = true;
     
        abacus.GetComponent<Collider2D>().enabled = false;
        
        cameraFocus.LetCameraFocus();
        
        AudioMgr.Instance.PlaySFX(FightEffect);
        yield return GetComponent<BattleResover>().BattleResolve();
        AbacusAnim.Finsined = false;
        

        #endregion
       
        yield return new WaitForSeconds(1.5f);
        FightDataManager.ActionPoints = DataBroker.actionValue;
        actionPoint.UpdatePoints(FightDataManager.ActionPoints);;
        //呱：————————————————Round3—————————————————————
        
        #region 上虫

      
        //呱：把A虫虫放在 第12格
        waitingBug.BugUp(5,11);
        //呱：把E虫虫放在 上一个E虫虫的身后
        waitingBug.BugUp(4,movedIndexE+4);

        #endregion
        
        #region 对话

        yield return new WaitForSeconds(fadeTime);
        dialogue.background.SetActive(true);
        yield return Speak(0,"有点东西","还没结束呢！");
        
        
        ReleseCage();
        
        AbacusAnim.Finsined = false;

        #endregion
        
        #region 结算
        
        abacus.GetComponent<Collider2D>().enabled = true;  
        yield return new WaitUntil(() => AbacusAnim.Finsined == true);
        while (!AbacusAnim.Finsined)
        {
            if (FightDataManager.ActionPoints == 0)
            {
                BanCage();
            }
        
            yield return new WaitForSeconds(1.5f);
            
        }
        
        
        yield return new WaitUntil(()=> AbacusAnim.Finsined==true);
        yield return new WaitForSeconds(0.3f);
        waitingBug.CountMyBugs();
        yield return new WaitForSeconds(0.1f);
        waitingBug.myBugCount = 0;              
        waitingBug.CountMyBugs();  
        
        yield return waitingBug.FindRival(11);
        yield return waitingBug.FindRival(movedIndexF);
        movedIndexF = waitingBug.lastMovedGridIndex;
        Debug.Log( waitingBug.GetComponent<WaitingBug>().myBugCount);
        
        
        yield return waitingBug.FindRival(movedIndexD);
        Debug.Log( waitingBug.GetComponent<WaitingBug>().myBugCount);
        waitingBug.GetComponent<WaitingBug>().myBugCount = 0;
        
        movedIndexD = waitingBug.lastMovedGridIndex;

   
        yield return waitingBug.FindRival(movedIndexE);
        Debug.Log( waitingBug.GetComponent<WaitingBug>().myBugCount);
        waitingBug.GetComponent<WaitingBug>().myBugCount = 0;
        
        movedIndexE = waitingBug.lastMovedGridIndex;
        
        
       
        //呱：放大相机 聚焦在战局上面
        mainCamera.GetComponent<CameraFocus>().enabled = true;
     
        abacus.GetComponent<Collider2D>().enabled = false;
        
        cameraFocus.LetCameraFocus();
        
        AudioMgr.Instance.PlaySFX(FightEffect);
        yield return GetComponent<BattleResover>().BattleResolve();
        AbacusAnim.Finsined = false;
        if (!DataBroker.WinGame2&&GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            DataBroker.WinGame3= false;
            DataBroker.reputation = 0;
            yield return ShowHint("战斗失败");
            yield return ShowHint("声誉值<b><color=#335EA4>清零</color></b>");
            cameraFocus.LetCameraFocus();
            yield return new WaitForSeconds(1.5f);
            OnGame3 = false;
            FightDataManager.DeliverData();
            StopFightBGM();
            Transition.Instance.SwitchSceneWithFade("BeforeBoss");
        }
        else if(DataBroker.WinGame2&& GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            DataBroker.WinGame3= true;
            DataBroker.experience += 10;
            yield return ShowHint("战斗胜利");
            yield return ShowHint("经验值增加<b><color=#335EA4>10</color></b>点");
            cameraFocus.LetCameraFocus();
            yield return new WaitForSeconds(1.5f);
            OnGame3 = false;
            FightDataManager.DeliverData();
            StopFightBGM();
            Transition.Instance.SwitchSceneWithFade("BeforeBoss");
        }

        #endregion
        
        yield return new WaitForSeconds(1.5f);
        FightDataManager.ActionPoints = DataBroker.actionValue;
        actionPoint.UpdatePoints(FightDataManager.ActionPoints);;
        //呱：————————————————Round4—————————————————————
        
        #region 对话

        yield return new WaitForSeconds(fadeTime);
        dialogue.background.SetActive(true);
        yield return ShowDialogueHint("李大爷:老马小心！这虫儿不对劲，开始耍阴的了！");
        yield return Speak(3,"什么阴招？","这是我这虫儿斗性上来了","越斗越猛！");
        yield return StartCoroutine(ShowHint("章节Boss特殊机制[<b><color=#335EA4>后期发力</color></b>]：\n从该回合开始，敌方场上的蛐蛐每回合+1点攻击，永久叠加 "));
        
        ReleseCage();
        
        AbacusAnim.Finsined = false;
        
        #endregion

        #region 敌方虫虫增强

        waitingBug.HightenBugs();
        yield return new WaitForSeconds(1f);

        #endregion
        
        #region 结算
        
        abacus.GetComponent<Collider2D>().enabled = true;  
        yield return new WaitUntil(() => AbacusAnim.Finsined == true);
        while (!AbacusAnim.Finsined)
        {
            if (FightDataManager.ActionPoints == 0)
            {
                BanCage();
            }
        
            yield return new WaitForSeconds(1.5f);
            
        }
        
        
        yield return new WaitUntil(()=> AbacusAnim.Finsined==true);
        yield return new WaitForSeconds(0.3f);
        waitingBug.CountMyBugs();
        yield return new WaitForSeconds(0.1f);
        waitingBug.myBugCount = 0;              
        waitingBug.CountMyBugs();  
        yield return waitingBug.FindRival(11);
        yield return waitingBug.FindRival(10);
        yield return waitingBug.FindRival(9);
        yield return waitingBug.FindRival(8);
       
        
        
        //呱：放大相机 聚焦在战局上面
        mainCamera.GetComponent<CameraFocus>().enabled = true;
     
        abacus.GetComponent<Collider2D>().enabled = false;
        
        cameraFocus.LetCameraFocus();
        
        AudioMgr.Instance.PlaySFX(FightEffect);
        yield return GetComponent<BattleResover>().BattleResolve();
        AbacusAnim.Finsined = false;
        if (!DataBroker.WinGame2&&GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            DataBroker.WinGame3= false;
            DataBroker.reputation = 0;
            yield return ShowHint("战斗失败");
            yield return ShowHint("声誉值<b><color=#335EA4>清零</color></b>");
            cameraFocus.LetCameraFocus();
            yield return new WaitForSeconds(1.5f);
            OnGame3 = false;
            FightDataManager.DeliverData();
            StopFightBGM();
            Transition.Instance.SwitchSceneWithFade("BeforeFight2");
        }
        else if(DataBroker.WinGame2&& GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            DataBroker.WinGame3= true;
            DataBroker.experience += 10;
            yield return ShowHint("战斗胜利");
            yield return ShowHint("经验值增加<b><color=#335EA4>10</color></b>点");
            cameraFocus.LetCameraFocus();
            yield return new WaitForSeconds(1.5f);
            OnGame3 = false;
            FightDataManager.DeliverData();
            StopFightBGM();
            Transition.Instance.SwitchSceneWithFade("BeforeFight2");
        }

        #endregion
    
        yield return new WaitForSeconds(1.5f);
        FightDataManager.ActionPoints = DataBroker.actionValue;
        actionPoint.UpdatePoints(FightDataManager.ActionPoints);;
        //呱：————————————————Round ? —————————————————————

        while (true)
        {
            
            
             #region 敌方虫虫增强

        waitingBug.HightenBugs();
        yield return new WaitForSeconds(1f);

        #endregion
        
             #region 结算
        
        abacus.GetComponent<Collider2D>().enabled = true;  
        yield return new WaitUntil(() => AbacusAnim.Finsined == true);
        while (!AbacusAnim.Finsined)
        {
            if (FightDataManager.ActionPoints == 0)
            {
                BanCage();
            }
        
            yield return new WaitForSeconds(1.5f);
            
        }
        
        
        yield return new WaitUntil(()=> AbacusAnim.Finsined==true);
        yield return new WaitForSeconds(0.3f);
        waitingBug.CountMyBugs();
        yield return new WaitForSeconds(0.1f);
        waitingBug.myBugCount = 0;              
        waitingBug.CountMyBugs();  
      
        
        yield return waitingBug.FindRival(11);
        yield return waitingBug.FindRival(10);
        yield return waitingBug.FindRival(9);
        yield return waitingBug.FindRival(8);

        
        //呱：放大相机 聚焦在战局上面
        mainCamera.GetComponent<CameraFocus>().enabled = true;
     
        abacus.GetComponent<Collider2D>().enabled = false;
        
        cameraFocus.LetCameraFocus();
        
        AudioMgr.Instance.PlaySFX(FightEffect);
        yield return GetComponent<BattleResover>().BattleResolve();
        AbacusAnim.Finsined = false;
        if (!DataBroker.WinGame2&&GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            DataBroker.WinGame3= false;
            DataBroker.reputation = 0;
            yield return ShowHint("战斗失败");
            yield return ShowHint("声誉值<b><color=#335EA4>清零</color></b>");
            cameraFocus.LetCameraFocus();
            yield return new WaitForSeconds(1.5f);
            OnGame3 = false;
            FightDataManager.DeliverData();
            StopFightBGM();
            Transition.Instance.SwitchSceneWithFade("BeforeBoss");
        }
        else if(DataBroker.WinGame2&& GetComponent<BattleResover>().Nobug)
        {
            GetComponent<BattleResover>().Nobug = false;
            DataBroker.WinGame3= true;
            DataBroker.experience += 10;
            yield return ShowHint("战斗胜利");
            yield return ShowHint("经验值增加<b><color=#335EA4>10</color></b>点");
            cameraFocus.LetCameraFocus();
            yield return new WaitForSeconds(1.5f);
            OnGame3 = false;
            FightDataManager.DeliverData();
            StopFightBGM();
            Transition.Instance.SwitchSceneWithFade("BeforeBoss");
        }

        #endregion
        
       
        
        yield return new WaitForSeconds(1.5f);
        FightDataManager.ActionPoints = DataBroker.actionValue;
        actionPoint.UpdatePoints(FightDataManager.ActionPoints);;
        }
        yield return null;
    }

   
  
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
