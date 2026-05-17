using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 用来表示当前格子类型 （敌我方 / 前后排）
/// </summary>
public enum E_GridType
{
    EnemyFront,
    EnemyBack,
    MyFront,
    MyBack,
}


/// <summary>
/// 格子结构体 用来存格子数据和信息
/// </summary>
public struct Grid
{
    //呱：格子的编号 数组的下标
    public int index;
    
    //呱：格子的实际物体
    public GameObject gridObject;
    
    //呱：该格子对应的有效坐标
    public Vector3 matchedPos;
    
    //呱：该格子的类型
    public E_GridType gridType;
    
    //呱：该格子目前是否被占领
    public bool isOccupied;

    public bool isRight;

    public GameObject bugOnGrid;

    
}


//呱： woc我真快写疯了 我决定给待机虫虫 挂载这个美味脚本
//    代码求你自己跑起来
//    做法 # 做法
public class RoundManager : MonoBehaviour
{
    
    private bool aPlaced = false;
    private bool bPlaced = false;
    

    private BugMatch bugMatch;
    private Draggable draggable;
    public static int nowRound =1;
    
    private FightDataManager fightDataManager;
    private FightFlowManager fightFlowManager;
    private Hint hint;

    [Header("虫虫上前排")]
    [SerializeField]AnimationCurve moveCurveToFront;
    private float moveDuration = 1.5f;
    
    [Header("虫虫升级")]
    [SerializeField]AnimationCurve moveCurveForLevelUp;

    [Header("升级特效")]
    [SerializeField] ParticleSystem levelUpParticles;
    [SerializeField] AudioClip levelUpSound;
    private bool needCompound;
    private ActionPoint actionPoint;
    private GameObject frontBug;
    
    
    [SerializeField] List<Sprite>  levelUpSprite = new List<Sprite>();
    
    void Start()
    {
        
        //呱：在小鼠老大没有写好这个逻辑前 先用这个强制启动 方便我做实验
        //FightFlowManager.OnGame3 = true;
       // FightFlowManager.OnGame2 = true;
        //FightFlowManager.OnGame1 = true;
        fightFlowManager = FindObjectOfType<FightFlowManager>();
        fightDataManager = FindObjectOfType<FightDataManager>();
        hint = FindObjectOfType<Hint>();

        actionPoint = FindObjectOfType<ActionPoint>();
    }

    
    void Update()
    {
        draggable = GetComponent<Draggable>();
        bugMatch = GetComponent<BugMatch>();
        
        
    }

    //呱： 给第一关教学关卡 写的函数
    //    屎如山
    public void TeachingRound(GameObject nowBug)
    { 
       
        ActionPoint actionPoint = FindObjectOfType<ActionPoint>();
        
        if (nowBug == null) return;
       if(!FightFlowManager.OnGame1) return;
       if(FightDataManager.ActionPoints <= 0) return;
       
      
       
       if (nowRound == 1) 
       {
          
                if (Draggable.nowBugType == E_BugType.A)
                {
                    //呱 ： 注意这里数组下标 需要减一 原本对应的是 第五格
                    if (Draggable.nowGridIndex == 4 )
                    {
                        
                        bugMatch.StartFightBug();
                       nowBug.SetActive(false) ;
                       aPlaced = true;
                       FightDataManager.ActionPoints -= 1;
                        FightFlowManager.count++;
                        AudioMgr.Instance.PlaySFX(Id_To_Insect_Dic.IdToInsectDic[1].insectSound);
                        actionPoint.UpdatePoints(FightDataManager.ActionPoints);
                        if (aPlaced && bPlaced)
                        {
                            nowRound =2 ;
                            aPlaced = false;
                            bPlaced = false;
          
                            return;
           
                        }
                    }
                }
                else if (Draggable.nowBugType == E_BugType.B)
                {
                    if (Draggable.nowGridIndex == 5)
                    {
                        bugMatch.StartFightBug();
                        nowBug.SetActive(false) ;
                        bPlaced = true;
                        FightDataManager.ActionPoints -= 1;
                        FightFlowManager.count++;
                        AudioMgr.Instance.PlaySFX(Id_To_Insect_Dic.IdToInsectDic[2].insectSound);
                        actionPoint.UpdatePoints(FightDataManager.ActionPoints);
                    }
                    if (aPlaced && bPlaced)
                    {
                        nowRound =2 ;
                        aPlaced = false;
                        bPlaced = false;
          
                        return;
           
                    }
                    
                }

       }
       else if (nowRound == 2)
       {
       
           if (Draggable.nowBugType == E_BugType.A && Draggable.nowGridIndex == 0)
           {
            
               bugMatch.StartFightBug();
               
               Transform parentTransform = Draggable.nowBug.transform.parent;
              

              
                InsectData bugA = nowBug.GetComponent<InsectData>();
                     
               fightDataManager.UpdateFightBugAtIndex(10,bugA);
                       
                   
                 
               parentTransform.gameObject.SetActive(false);
             
               Debug.Log($"已禁用 {parentTransform.name}");
               
               
               nowBug.SetActive(false);
               FightDataManager.ActionPoints -= 1;
               FightFlowManager.count++;
               AudioMgr.Instance.PlaySFX(Id_To_Insect_Dic.IdToInsectDic[1].insectSound);
               actionPoint.UpdatePoints(FightDataManager.ActionPoints);
               
           }
           
       }
            
    }


    //呱： 正式关卡
    public void FormolRound(GameObject nowBug)
    {
        Grid nowGrid = GridManager.Grids[Draggable.nowGridIndex];
        //呱：如果 该格子上面已经有虫虫了 那么就不进判断
        if (nowGrid.isOccupied) return;

        needCompound = false;
        
        //呱：获取待机虫的父物体 再迂回的利用父物体获取战斗虫
        GameObject temp = nowBug.transform.parent.gameObject;
        
        //呱：获取战斗虫
        GameObject fightBug = temp.transform.GetChild(0).gameObject;

        
        //呱： 首先判断 当前虫子在 哪种格子的 上空
        //呱： 我方前排
        if (nowGrid.gridType == E_GridType.MyFront)
        {
    
            //呱： 标记 该格子上有虫虫了
            GridManager.Grids[Draggable.nowGridIndex].isOccupied = true;
            GridManager.Grids[Draggable.nowGridIndex].bugOnGrid = fightBug;
            
            //呱：让战斗虫虫的坐标 在这个格子上面
            fightBug.transform.position = nowGrid.matchedPos;
            
            bugMatch.StartFightBug();
            FightDataManager.ActionPoints -= nowBug.transform.GetComponentInParent<InsectData>().insectLevel;
            actionPoint.UpdatePoints(FightDataManager.ActionPoints);
            Debug.Log($"{nowBug.name}被放在了{nowGrid.index}号格子上");
            //呱：献祭虫虫……
            nowBug.SetActive(false) ;
            
        }
        //呱：我方后排
        else if (nowGrid.gridType == E_GridType.MyBack)
        {
         
            //呱：获得真正的格子索引 前面有虫就不变  前面没虫就前移
            int realIndex;
            
            //呱：如果前方没有虫子
            if (NoFrontBug(nowGrid, out realIndex))
            {
               
                Vector3 targetPos;
                //呱：为在格子上的虫虫赋值
                GridManager.Grids[realIndex].isOccupied = true;
                GridManager.Grids[realIndex].bugOnGrid = fightBug;
                
                Debug.Log(fightBug.name);
                Debug.Log(GridManager.Grids[realIndex].bugOnGrid.name);
                targetPos = GridManager.Grids[realIndex].matchedPos;
                
                
                //呱:设置战斗虫虫的出现位置
                fightBug.transform.position = GridManager.Grids[Draggable.nowGridIndex].matchedPos;
              
                bugMatch.StartFightBug();
                FightDataManager.ActionPoints -= nowBug.GetComponent<InsectData>().insectLevel;
                Transparent(nowBug);
                GameObject Tag = nowBug.transform.GetChild(0).gameObject;
                Transparent(Tag);
                Debug.Log($"{nowBug.name}被放在了{realIndex}号格子上");
                StartCoroutine(MoveAndDisable(targetPos, fightBug, nowBug,moveCurveToFront,realIndex));
                
                
            }
            //呱：如果前面有虫子 需要检测前面虫虫的种类 还有等级 看是否可以合成
            else
            {
        
                Grid frontGrid = GridManager.Grids[realIndex+4]; 
                frontBug =frontGrid.bugOnGrid;

                if (NeedCompound(fightBug, frontBug))
                {
                  
                    //呱：残忍的杀死这个虫虫！！！！（成为我的养分吧！！！）
                   
                    fightBug.transform.position = GridManager.Grids[realIndex].matchedPos;
                    levelUpParticles.transform.position= GridManager.Grids[realIndex+4].matchedPos;
                    FightDataManager.ActionPoints -= nowBug.transform.GetComponentInParent<InsectData>().insectLevel;
                    actionPoint.UpdatePoints(FightDataManager.ActionPoints);
                    needCompound = true;
                    Transparent(nowBug);
                    GameObject Tag = nowBug.transform.GetChild(0).gameObject;
                    Transparent(Tag);
                    bugMatch.StartFightBug();
                    
                    Vector3 targetPos;
                    targetPos = GridManager.Grids[realIndex + 4].matchedPos;
                    Debug.Log($"{nowBug.name}被放在了{realIndex}号格子上");
                    StartCoroutine(MoveAndDisable(targetPos, fightBug, nowBug,moveCurveForLevelUp,realIndex));
                    
                   
                }
                else
                {
                
                    //呱：那就走正常逻辑
                    //呱：为在格子上的虫虫赋值
                    GridManager.Grids[realIndex].isOccupied = true;
                    GridManager.Grids[realIndex].bugOnGrid = fightBug;
                
                    //呱:设置战斗虫虫的出现位置
                    fightBug.transform.position = GridManager.Grids[realIndex].matchedPos;
                
                    bugMatch.StartFightBug();
                    FightDataManager.ActionPoints -= nowBug.transform.GetComponentInParent<InsectData>().insectLevel;
                    actionPoint.UpdatePoints(FightDataManager.ActionPoints);
                    Debug.Log($"{nowBug.name}被放在了{realIndex}号格子上");
                    nowBug.SetActive(false);
                }
                
            }
           
        }
      
        
    }

    //呱：为Boss战虫虫重合问题打的补丁
    public static void BanGrids()
    {
       GridManager.Grids[2].gridObject.GetComponent<Collider2D>().enabled = false; 
       GridManager.Grids[3].gridObject.GetComponent<Collider2D>().enabled = false; 
       GridManager.Grids[6].gridObject.GetComponent<Collider2D>().enabled = false; 
       GridManager.Grids[7].gridObject.GetComponent<Collider2D>().enabled = false; 
    }
    
    public static void ReleaseGrids()
    {
        GridManager.Grids[2].gridObject.GetComponent<Collider2D>().enabled = true; 
        GridManager.Grids[3].gridObject.GetComponent<Collider2D>().enabled = true; 
        GridManager.Grids[6].gridObject.GetComponent<Collider2D>().enabled = true; 
        GridManager.Grids[7].gridObject.GetComponent<Collider2D>().enabled = true; 
    }

    //呱：用来检测前面格子 有没有虫子 没有就自动前移
    private bool NoFrontBug(Grid nowGrid , out int index)
    {
        Grid frontGrid = GridManager.Grids[Draggable.nowGridIndex + 4];
        
        //呱：前面格子有虫就不动 前面格子没虫就往前移(或者合成)
        index = frontGrid.isOccupied ?Draggable.nowGridIndex : Draggable.nowGridIndex+4;

        //呱：返回判断值
        return !frontGrid.isOccupied;
    }

    //呱：用来判断是否需要合成升级
    private bool NeedCompound(GameObject nowBug,GameObject frontBug)
    {

        
        InsectData nowBugData = nowBug.GetComponent<InsectData>();
        InsectData frontBugData = frontBug.GetComponent<InsectData>();

        if (frontBugData.insectLevel == nowBugData.insectLevel && frontBugData.bugType == nowBugData.bugType)
        {
            //呱： 直接在这里面处理升级数据处理的问题
            frontBugData.insectLevel += 1;
            frontBugData.insectAtk += nowBugData.insectAtk;
            frontBugData.insectHP += nowBugData.insectHP;
            frontBugData.isCompound = true;
            
            return true;
        }
        return false;
    }

    IEnumerator MoveAndDisable(Vector3 endPos, GameObject fightBug, GameObject nowBug,AnimationCurve curve,int realIndex)
    {
        
        yield return Move(endPos, fightBug,curve);
        
        if (needCompound)
        {
            
            AudioMgr.Instance.PlaySFX(levelUpSound);
            fightDataManager.UpdateBugUI(frontBug);
            levelUpParticles.Play();
            Destroy(fightBug);
            if (frontBug != null)
            {
                fightDataManager.UpdateBugUI(frontBug);
            
                InsectData data = frontBug.GetComponent<InsectData>();
                if (data != null)
                {
                    ChangeLevelUpSprite(data.bugType.ToString(), frontBug);
                }
                else
                {
                    Debug.LogError("frontBug 缺少 InsectData 组件");
                }
            } 
            
            // GridManager.Grids[Draggable.nowGridIndex+4].bugOnGrid.GetComponent<ObjectShake>().ShakeStart(0.3f,0.1f);
           
           Camera.main.GetComponent<CamaraShake>().ShakeStart(0.5f,0.3f);
           
            yield return new WaitForSeconds(levelUpSound.length+0.5f);
            levelUpParticles.Stop();
            
        }
      
        nowBug.SetActive(false);
        
    }


    IEnumerator Move(Vector3 endPos, GameObject fightBug,AnimationCurve curve)
    {
        
        Vector3 startPos = fightBug.transform.position;
        float time = 0;
        while (time < moveDuration)
        {
            time += Time.deltaTime;
            fightBug.transform.position = 
                Vector3.Lerp(startPos, endPos, curve.Evaluate(time / moveDuration));
            yield return null;
        }
        fightBug.transform.position =  endPos;
     
    }

    private void Transparent(GameObject Object)
    {
        Object.GetComponent<SpriteRenderer>().color =
            new Color(Object.GetComponent<SpriteRenderer>().color.r,
                Object.GetComponent<SpriteRenderer>().color.g,
                Object.GetComponent<SpriteRenderer>().color.b, 0);
    }

    private void ChangeLevelUpSprite(string levelUpSpriteName,GameObject nowBug)
    {
        SpriteRenderer sp = nowBug.GetComponent<SpriteRenderer>();
        Color color = Color.cyan;
        
        switch (levelUpSpriteName)
        {
            case "A":
                sp.sprite = levelUpSprite[0];
                break;
            case "B":
               // sp.sprite = levelUpSprite[1];
               sp.color = color;
                break;
            case "C":
               //sp.sprite = levelUpSprite[3];
               sp.color = color;
                break;
            case "D":
               // sp.sprite = levelUpSprite[4];
               sp.color = color;
                break;
            case "E":
               // sp.sprite = levelUpSprite[5];
               sp.color = color;
                break;
            case "F":
              //  sp.sprite = levelUpSprite[6];
              sp.color = color;
                break;
        }
    }

    public void deliverMyBugDataToCage()
    {
        List<InsectData> tempBugs = new List<InsectData>();
        for (int i = 0; i < 8; i++)
        {
            if (GridManager.Grids[i].bugOnGrid != null)
            {
                tempBugs.Add(GridManager.Grids[i].bugOnGrid.GetComponent<InsectData>());
                continue;
            }
        }
        
        if(tempBugs.Count > 0)FightDataManager.DeliverData(tempBugs);
        BattleResover.diedBugs.Clear();
    }
}
