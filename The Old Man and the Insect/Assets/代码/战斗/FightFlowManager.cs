using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


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
    private Hint hint;
    [SerializeField]RoundManager roundManager;
    [SerializeField] GameObject WaitingBugs;
    private WaitingBug waitingBug;
    [SerializeField] FightDataManager fightDataManager;
    
    
    [Header("遮幕")]
    [SerializeField] GameObject Mask;
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
    
    
    void Start()
    {
        
        waitingBug = WaitingBugs.GetComponent<WaitingBug>();
        cameraFocus = mainCamera.GetComponent<CameraFocus>();
        mask = Mask.GetComponent<Transition>();
        gridManager =  GridManager.GetComponent<GridManager>();
       
        foucaMask_SR = FoucsMask.GetComponent<SpriteRenderer>();
        Color color = foucaMask_SR.color;
        color.a = 0f;
        foucaMask_SR.color = color;
        
        dialogue = DialogueManager.GetComponent<DialogueForFight>();
        hint = HintManager.GetComponent<Hint>();
        cameraShake = mainCamera.GetComponent<CamaraShake>();
    }

    
    void Update()
    {
        //呱：节省性能
        if(haveCheckedFightType) return;
            
        //呱： 用来判断现在是 第几次游戏
        if (OnGame1)
        { 
            onTeachingRound = true;
           StartCoroutine(Game1Flow());
           haveCheckedFightType = true;
           
        }
        else if (OnGame2)
        {
            onTeachingRound = false;
            haveCheckedFightType = true;
        }
        else if (OnGame3)
        {
            onTeachingRound = false;
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
        
       mask.FadeOut(fadeTime);
        
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
        yield return StartCoroutine(ShowHint("看到左下角的笼子了吗，快点击它！"));
        
        ReleseCage();
        AudioMgr.Instance.PlaySFX(bugSing);
        yield return StartCoroutine(TransportMask());
        
        //呱： ②引导 把虫虫放到正确的地方
        yield return new WaitUntil(() =>CageZoom.CageHasZoomed);
        bugs[1].GetComponent<SpriteRenderer>().color =
            new Color(89/255f, 116/255f, 77/255f, 1f);
        yield return StartCoroutine(ShowHint("长按左键拖动蛐蛐到象棋格中"));

        
        // 呱：等待玩家放置第一只虫子（count 从 0 变为 1）
        yield return new WaitUntil(() => count > 0);
        yield return StartCoroutine(ShowHint("很好，每回合放置一级蛐蛐都会消耗一点行动值\n现在尝试放置另外一只吧"));

        
        //呱：为了防止 两只A虫虫都被抓起来了 我们禁用一下
       
      
   
        // 呱：等待玩家放置第二只虫子（count 从 1 变为 2）
        yield return new WaitUntil(() => count > 1);
        yield return StartCoroutine(ShowHint("拨动算盘，结束回合"));


      
        
        
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

         waitingBug.BugUp(0);
        yield return new WaitForSeconds(0.3f);
        ActionPoint actionPoint = FindObjectOfType<ActionPoint>();
        FightDataManager.ActionPoints = 2;
        actionPoint.UpdatePoints(FightDataManager.ActionPoints);;
        
        
        #endregion

        #region 提示和引导

        yield return StartCoroutine(ShowHint("再次点击笼子"));
        ReleseCage();
        yield return new WaitUntil(() =>CageZoom.CageHasZoomed);
        
        yield return StartCoroutine(ShowHint("这次拖动蛐蛐放置在同一品种的后方"));
        bugs[1].GetComponent<Collider2D>().enabled = true;



        yield return new  WaitUntil(() => FightDataManager.ActionPoints == 1);
        
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        ps.Play(true);
        yield return StartCoroutine(ShowHint("同一种类的蛐蛐可以在战斗中融合，以获得更强的效果"));
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
        yield return  new WaitForSeconds(0.5f);
        AudioMgr.Instance.PlaySFX(FightEffect);
        yield return gridManager.
            ExecuteBattle(new Vector3(0, 0.3f, 0), new Vector3(0, -0.3f, 0), 1f);

        fdm.UpdateAllDisplay();
        
        count = 0;
        StartFight = true;
        


        #endregion
        
        yield return new WaitForSeconds(0.3f);
        cameraFocus.LetCameraFocus();

        #region 传值

        //呱：给小鼠老大传入 战斗后虫虫数据
        FightDataManager.DeliverData(bugs[3].GetComponent<InsectData>(),bugs[5].GetComponent<InsectData>());
        
        //呱：给小鼠老大传入 战斗后经验值
        DataBroker.experience += 4;

        #endregion

        OnGame1 = false;

        #region 切换场景

        yield return new WaitForSeconds(0.5f);
        Transition.Instance.SwitchSceneWithFade("BeforeCatch");
        SceneManager.LoadScene("BeforeCatch");

        #endregion

        yield return null;
    }

    IEnumerator Game2Flow()
    {
        
        yield return null;
    }

    void Game3Flow()
    {
        
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
