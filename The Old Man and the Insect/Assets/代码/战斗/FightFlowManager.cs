using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    
    public static bool onTeachingRound = false;
    public static bool StartFight = false;
    public static int count = 0;
    
    
    private float fadeTime = 1.5f;
    //呱：已经确认好 当堂的游戏类型   游戏流程 只单向执行一次 
    private bool haveCheckedFightType;
    
    
    void Start()
    {
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
        
        PrepareForFight();
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
        yield return StartCoroutine(ShowHint("长按左键拖动蛐蛐到象棋格中"));
        
        // 呱：等待玩家放置第一只虫子（count 从 0 变为 1）
        yield return new WaitUntil(() => count > 0);
        yield return StartCoroutine(ShowHint("很好，每回合放置一级蛐蛐都会消耗一点行动值\n现在尝试放置另外一只吧"));

        
        //呱：为了放置两只A虫虫都被抓起来了 我们禁用一下
        
        if (bugs[0].activeSelf == false)  // 第一只放的是 bugs[0]
        {
            DisableOtherBugDrag(bugs[1]);
        }
        else if (bugs[1].activeSelf == false)
        {
            DisableOtherBugDrag(bugs[0]);
        }
      
   
        // 呱：等待玩家放置第二只虫子（count 从 1 变为 2）
        yield return new WaitUntil(() => count > 1);
        yield return StartCoroutine(ShowHint("拨动算盘，结束回合"));
        
        abacus.GetComponent<Collider2D>().enabled = true;
        yield return new WaitUntil(()=> AbacusAnim.Finsihed==true);

        mainCamera.GetComponent<CameraFocus>().enabled = true;
        cameraFocus.LetCameraFocus();
        yield return new WaitForSeconds(0.5f);
        cameraShake.ShakeStart(0.4f, 0.05f);
        gridManager.GetComponent<GridManager>().
            MoveHitObjectsWithReturn(new Vector3(0, 0.3f, 0), new Vector3(0, -0.3f, 0), 1f);
        
        AudioMgr.Instance.PlaySFX(FightEffect);
        count = 0;
        StartFight = true;
        yield return  new WaitForSeconds(1.5f);
      
        
        #endregion
        
        
        yield return null;
    }

    void Game2Flow()
    {
        
    }

    void Game3Flow()
    {
        
    }

   
    void DisableOtherBugDrag(GameObject otherBug)
    {
        var drag = otherBug.GetComponent<Draggable>();
        drag.ForceStopDrag();           
        drag.enabled = false;          
     
    }
    IEnumerator TransportMask()
    {
        Color color = foucaMask_SR.color;
        color.a = 0.5f;
        foucaMask_SR.color = color;
        yield return StartCoroutine(Cage.GetComponent<ObjectShake>().Shake(1f,0.25f));
        yield return new WaitForSeconds(1f);
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
