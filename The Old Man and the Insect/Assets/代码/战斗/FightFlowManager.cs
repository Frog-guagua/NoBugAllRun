using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//呱： 哦我的天哪我终于决定写这个了……
//    希望我能顺清楚这些逻辑
//    做游戏真开心
public class FightFlowManager : MonoBehaviour
{
    [Header("音效")]
    [SerializeField] AudioClip MovingSound;
    
    [Header("战斗背景bgm")]
    [Tooltip("就这个战斗爽")]
    [SerializeField] AudioClip FightBGM;
    
    [Header("开の关")]
    public static bool OnGame1;
    public static bool OnGame2;
    public static bool OnGame3;

    [Header("俺の笼子")]
    [SerializeField]  GameObject Cage;
    
    [SerializeField]  GameObject RoundManager;
    
    private Transition mask;
    void Awake()
    {
        mask = FindObjectOfType<Transition>(); 
    }
    
    
    //呱：已经确认好 当堂的游戏类型   游戏流程 只单向执行一次 
    private bool haveCheckedFightType;
    void Start()
    {
        
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

    //呱： 第一次战斗……！ 大爷强强！！！
    IEnumerator Game1Flow()
    {
        #region 禁用物体上的脚本

        //呱：这里想的是禁用 我们的笼子 的点击放大(CageZoom) 那个脚本 
        //   这样就能解决对面大爷在出牌/说话 的时候 我们在这边多动症
        //   给我认真看啊！ 玩游戏的玩家 给我认真 看啊！
        BanCage();

        #endregion
        
        #region 战前氛围准备

        //呱： 首先是遮幕渐显
        mask.FadeIn(1.5f);
        
        //呱： 然后是音乐播放
        PlayFightBGM();

        #endregion

        #region 对话

        

        #endregion
        
        RoundManager.GetComponent<RoundManager>().TeachingRound(Draggable.nowBug);
        
        yield return null;
    }

    void Game2Flow()
    {
        
    }

    void Game3Flow()
    {
        
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
}
