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
    private DialogueForFight dialogue;
    [SerializeField] GameObject HintManager;
    private Hint hint;
    [SerializeField]RoundManager roundManager;
    
    
    [Header("遮幕")]
    [SerializeField] GameObject Mask;
    private Transition mask;

    [Header("相机抖动")] 
    [SerializeField]  Camera mainCamera;
    private CamaraShake  cameraShake;

    [Header("干扰物")] [SerializeField] GameObject button;
    [SerializeField]  List<GameObject> bugs;

    private float fadeTime = 1.5f;
    //呱：已经确认好 当堂的游戏类型   游戏流程 只单向执行一次 
    private bool haveCheckedFightType;
    
    
    void Start()
    {
        mask = Mask.GetComponent<Transition>();
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
           StartCoroutine(Game1Flow());
           haveCheckedFightType = true;
        }
        else if (OnGame2)
        {
            haveCheckedFightType = true;
        }
        else if (OnGame3)
        {
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

        foreach (var bug in bugs)
        {
            bug.SetActive(false);
        }

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
        /*yield return StartCoroutine(Speak(2,"怂了吗","快把你的蛐蛐放上来！"));
        button.SetActive(false);
        #endregion

        #region 提示和引导

        //呱： ①引导点击笼子 这时候我们顺便开放笼子权限
        yield return StartCoroutine(ShowHint("看到左下角的笼子了吗，快点击它！"));
        
        ReleseCage();
        foreach (var bug in bugs)
        {
            bug.SetActive(true);
        }
        yield return new WaitForSeconds(2);
        
       */ 
       
        while (!RoundManager.finishDrag)
        {
            roundManager.GetComponent<RoundManager>().TeachingRound(Draggable.nowBug);
        }
        #endregion
        
        
        yield return null;
    }

    void Game2Flow()
    {
        
    }

    void Game3Flow()
    {
        
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

    IEnumerator ShowHint(string hintContent)
    {
        AudioMgr.Instance.PlaySFX(hintEffect);
        hint.ShowHint(hintContent);
        yield return null;
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
