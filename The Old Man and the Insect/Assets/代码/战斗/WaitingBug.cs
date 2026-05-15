using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaitingBug : MonoBehaviour
{
    //呱 ： 对手虫的集合 
    [SerializeField]List<GameObject> WaitingBugs = new List<GameObject>();
    
    [Header("虫虫升级")]
    [SerializeField]AnimationCurve moveCurveForLevelUp;

    [Header("升级特效")]
    [SerializeField] ParticleSystem levelUpParticles;
    
    [SerializeField] AnimationCurve moveCurve;
    [SerializeField] AudioClip levelUpSound;
    public LayerMask targetLayer;
    [SerializeField] Vector3 offset;
    private bool needCompound;

    public  int myBugCount = 0;
    private float moveDuration = 1f;
    private int gridIndex;
    public int lastMovedGridIndex = -1;
    public bool finishComposed;
    
    void Start()
    {
        foreach (var waitbug in  WaitingBugs)
        {
            
            waitbug.GetComponent<Collider2D>().enabled = false;
            waitbug.GetComponent<SpriteRenderer>().enabled = false;
            waitbug.transform.GetChild(0).gameObject.SetActive(false);
            
        }
    }

    
    void Update()
    {
        
    }

    //呱 ：敌方虫子上场
    public void BugUp(int BugIndex,int GridIndex)
    {
        gridIndex = GridIndex;
        //呱：后面的自由战斗 上虫要安排站立的格子位置
        if (FightFlowManager.OnGame2 || FightFlowManager.OnGame3)
        {
            if (GridManager.Grids[GridIndex].gridType == E_GridType.EnemyBack)
            {
                int realIndex = 0;
                //呱 ：如果前方没有虫
                if (NoFrontBug(GridManager.Grids[GridIndex], out realIndex))
                {
                    //呱：放置在 前排的位置上
                    WaitingBugs[BugIndex].transform.position = GridManager.Grids[realIndex].matchedPos+offset;
                    GridManager.Grids[realIndex].isOccupied = true;
                    GridManager.Grids[realIndex].bugOnGrid = WaitingBugs[BugIndex];
                    Debug.Log($"把{WaitingBugs[BugIndex].name}放在了{realIndex}上");
                }
                //呱：如果前方有虫 
                else
                {
                    NeedCompound(WaitingBugs[BugIndex], GridManager.Grids[GridIndex - 4].bugOnGrid);
                    //呱 ：需要合成
                    if (needCompound)
                    {
                        Debug.Log("需要合成");
                        needCompound = true;
                        WaitingBugs[BugIndex].transform.position = GridManager.Grids[realIndex].matchedPos+offset;
                        
                        levelUpParticles.transform.position = GridManager.Grids[realIndex-4].matchedPos+offset;
                        Vector3 targetPos;
                        targetPos = GridManager.Grids[realIndex - 4].matchedPos+offset;
                        WaitingBugs[BugIndex].GetComponent<Collider2D>().enabled = true;
                        WaitingBugs[BugIndex].GetComponent<SpriteRenderer>().enabled = true;
                        WaitingBugs[BugIndex].transform.GetChild(0).gameObject.SetActive(true);
                        
                 
                        StartCoroutine(MoveAndDisable(targetPos, WaitingBugs[BugIndex], moveCurveForLevelUp,GridManager.Grids[realIndex-4].bugOnGrid));
                        //if (!needCompound) return;

                    }
                    //呱：不需要合成
                    else
                    {
                        WaitingBugs[BugIndex].transform.position = GridManager.Grids[realIndex].matchedPos+offset;
                        GridManager.Grids[realIndex].isOccupied = true;
                        GridManager.Grids[realIndex].bugOnGrid = WaitingBugs[BugIndex];
                    }
                
                }
          
            }
            else if (GridManager.Grids[GridIndex].gridType == E_GridType.EnemyFront)
            {
                //呱 ：把虫子放到原本的格子上
                WaitingBugs[BugIndex].transform.position = GridManager.Grids[GridIndex].matchedPos+offset;
                GridManager.Grids[GridIndex].isOccupied = true;
                GridManager.Grids[GridIndex].bugOnGrid = WaitingBugs[BugIndex];
            }
        }
           
        WaitingBugs[BugIndex].GetComponent<Collider2D>().enabled = true;
        Color color = new Color();
        color.a = 0;
        Vector3 bugNowPos = WaitingBugs[BugIndex].transform.position;
        
        WaitingBugs[BugIndex].GetComponent<SpriteRenderer>().color = color;
        WaitingBugs[BugIndex].GetComponent<SpriteRenderer>().enabled = true;
        
        SpriteRenderer sr = WaitingBugs[BugIndex].GetComponent<SpriteRenderer>();
        sr.color = Color.white;         

        
        StartCoroutine(UpBugAnime(WaitingBugs[BugIndex], bugNowPos, 1f));
        
        WaitingBugs[BugIndex].transform.GetChild(0).gameObject.SetActive(true);

        
    }

    //呱 ：这是敌方索敌

    public void CountMyBugs()
    {
        for (int i = 4; i < 8; i++)
        {
            if (GridManager.Grids[i].bugOnGrid != null)
            {
                myBugCount++;
            }
        }
    }


    //呱：这是专门写给boss战的 敌方虫虫增强的函数
    public void HightenBugs()
    {
        for (int i = 8; i < 16; i++)
        {
            if (GridManager.Grids[i].bugOnGrid != null)
            {
                GameObject temp = GridManager.Grids[i].bugOnGrid;
                temp.transform.GetComponentInParent<InsectData>().insectAtk += 1;
                StartCoroutine(Shake(0.5f, 0.25f, temp));
                UpdateBugUI(temp.transform.GetComponentInParent<InsectData>());
                
            }
        }
    }


    public void CheckBugOnWhichGrid()
    {
        for (int i = 8; i < 16; i++)
        {
            if(GridManager.Grids[i].bugOnGrid == null) continue;
            
            Debug.Log(GridManager.Grids[i].bugOnGrid.name + "在"+$"第{i+1}格上");
        }
    }

    public int FindA()
    {
        for (int i = 8; i < 16; i++)
        {
            if(GridManager.Grids[i].bugOnGrid == null) continue;

            if (GridManager.Grids[i].bugOnGrid.name == "A")
            {
                return i;
            }
        }

        return 11;
    }
    
    
    
    //呱：此函数为敌方索敌使用的
public IEnumerator FindRival(int GridIndex)
{
   
    //呱：这里是想用 场上玩家方虫虫 的数量 
    //   来判断 是否还需要索敌
    //   我们会在索敌前就统计一次虫虫的数量 所以不用担心
    if (myBugCount == 0)
    {
        //呱：如果是 在玩家没有虫虫了以后 依旧尝试索敌
        //   那么就原地传送（哇
        //   所在格子索引不改变 直接返回
        lastMovedGridIndex = GridIndex;
        yield break;
    }
    
    //呱：保存这个索引上对应的bugOnGrid
    GameObject currentBug = GridManager.Grids[GridIndex].bugOnGrid;
    
    
    //呱：如果索引GridIndex指向的格子上没有敌方虫虫 就返回
    if (currentBug == null)
    {
        Debug.Log($" 是空哒！！");
        yield break;
    }

    #region 【1】检查 有没有 前方没有我方虫虫的 敌方虫虫

    // 呱： 1. 检查敌方虫虫的正前方（同一列，向下 -4） 是否有我方虫子
    int forwardIndex = GridIndex - 4;
    if (forwardIndex < GridManager.Grids.Length)
    {
        GameObject targetBug = GridManager.Grids[forwardIndex].bugOnGrid;
        
        if (targetBug != null)
        {
            //呱：如果敌方虫虫前面有我方虫 我们就核销一只我方虫 
            //    而且此敌方虫虫不用索敌
            myBugCount--;
            lastMovedGridIndex = GridIndex;
            Debug.Log($"FindRival: 找到正前方敌人，格子 {forwardIndex}");
           
            yield break;
        }
    }

    #endregion

    #region 【2】检查 我方虫虫 是不是前面都已经有 敌方虫虫了

    //呱： 2. 没有正前方敌人，遍历我方虫虫前排（索引 4-7）
    int targetGridIndex = -1;
    for (int i = 4; i < 8; i++)
    {
        if (GridManager.Grids[i].bugOnGrid == null) continue;
        int frontIndex = i + 4;
        if (frontIndex < GridManager.Grids.Length && GridManager.Grids[frontIndex].bugOnGrid == null)
        {
            Debug.Log("成功索敌");
            myBugCount--;
            lastMovedGridIndex = frontIndex;
            targetGridIndex = frontIndex;
            break;
        }
    }

    #endregion

    #region 如果 敌方虫虫数量>我方虫虫

    //呱：这里是我方虫虫都有对应的敌方虫虫了 所以这个敌方虫虫就不用索敌了
    //   所以我们的处理依旧是原地tp
    if (targetGridIndex == -1)
    {
        targetGridIndex = GridIndex;
        lastMovedGridIndex = GridIndex;
        
        //呱：直接赋0 让这个敌方虫虫后面的敌方虫虫都不用索敌了
        myBugCount = 0;
        yield break;
    }

    #endregion
  
    //————————————————————后面就是需要索敌进行的一些动作——————————————————

    // 3. 平滑移动
    Vector3 startPos = currentBug.transform.position;
    Vector3 targetPos = GridManager.Grids[targetGridIndex].matchedPos + offset;
    float moveDuration = 0.5f;
    float elapsed = 0f;
    while (elapsed < moveDuration)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / moveDuration);
        float curveValue = moveCurve.Evaluate(t);
        currentBug.transform.position = Vector3.Lerp(startPos, targetPos, curveValue);
        yield return null;
    }
    currentBug.transform.position = targetPos;


    // 4. 更新格子占用
    GridManager.Grids[GridIndex].isOccupied = false;
    GridManager.Grids[GridIndex].bugOnGrid = null;
    GridManager.Grids[targetGridIndex].isOccupied = true;
    GridManager.Grids[targetGridIndex].bugOnGrid = currentBug;



    Debug.Log($"FindRival: 虫子从格子 {GridIndex} 移动到格子 {targetGridIndex}");
    yield return null;
}    
    IEnumerator MoveAndDisable(Vector3 endPos, GameObject fightBug,AnimationCurve curve,GameObject targetBug)
    {
   
        yield return Move(endPos, fightBug,curve);
        
        if (needCompound)
        {
            
            AudioMgr.Instance.PlaySFX(levelUpSound);
            levelUpParticles.Play();
            Destroy(fightBug);
           
            Camera.main.GetComponent<CamaraShake>().ShakeStart(0.5f,0.3f);
            Color color = new Color();
            color = Color.red;
            StartCoroutine(ColorTransition.FadeToColor(targetBug, color));
            yield return new WaitForSeconds(levelUpSound.length+0.5f);
            needCompound = false;
            levelUpParticles.Stop();
            
        }
        
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
    private bool NoFrontBug(Grid nowGrid , out int index)
    {
        Grid frontGrid = GridManager.Grids[gridIndex - 4];
        
        //呱：前面格子有虫就不动 前面格子没虫就往前移(或者合成)
        index = frontGrid.isOccupied ?gridIndex : gridIndex-4;

        //呱：返回判断值
        return !frontGrid.isOccupied;
    }
    
    private void NeedCompound(GameObject nowBug,GameObject frontBug)
    {
        if (nowBug == null) return;
        InsectData nowBugData = nowBug.GetComponent<InsectData>();
        InsectData frontBugData = frontBug.GetComponent<InsectData>();

        if (frontBugData.insectLevel == nowBugData.insectLevel && frontBugData.bugType == nowBugData.bugType)
        {
            needCompound = true;
            //呱： 直接在这里面处理升级数据处理的问题
            frontBugData.insectLevel += 1;
            frontBugData.insectAtk += nowBugData.insectAtk;
            frontBugData.insectHP += nowBugData.insectHP;
            frontBugData.isCompound = true;
            finishComposed = true;
            
            UpdateBugUI(frontBugData);
            return ;
        }

        needCompound = false;
        return ;
    }
    
    public IEnumerator Shake(float duration, float strength,int bugIndex)
    {
        float timeCount = 0.0f;
      Vector3 originalPos = WaitingBugs[bugIndex].transform.position;
        while (timeCount < duration)
        {

            
            //呱：古法抖动 还得是随机数
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;
            WaitingBugs[bugIndex].transform.localPosition = originalPos + new Vector3(x, y, 0);
            timeCount += Time.deltaTime;
            yield return null;
        }
  
        WaitingBugs[bugIndex].transform.localPosition = originalPos;
       
       
    }
     IEnumerator Shake(float duration, float strength,GameObject bug)
    {
        float timeCount = 0.0f;
        Vector3 originalPos = bug.transform.position;
        while (timeCount < duration)
        {

            
            //呱：古法抖动 还得是随机数
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;
          bug.transform.localPosition = originalPos + new Vector3(x, y, 0);
            timeCount += Time.deltaTime;
            yield return null;
        }
  
        bug.transform.localPosition = originalPos;
       
       
    }
    private void UpdateBugUI(InsectData bug)
    {
        if (bug == null) return;
        TextMeshProUGUI tmp = bug.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = $"{bug.insectHP}\n\n\n{bug.insectAtk}";
    }

    IEnumerator UpBugAnime(GameObject obj, Vector3 endPos, float duration)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
       

        // 初始透明度设为 0
        Color startColor = sr.color;
        startColor.a = 0f;
        sr.color = startColor;

        float newY = endPos.y;
        newY += 1;
        
        
        Vector3 startPos = new Vector3(endPos.x+0.2f, newY, endPos.z);
        
        // 设置起始位置
        obj.transform.position = startPos;

        float elapsed = 0f;

        
        if (!FightFlowManager.OnGame1)
        {
            ParticleSystem ps = obj.transform.GetChild(1).GetComponent<ParticleSystem>();
            ps.gameObject.SetActive(true);
            ps.Play(); 
        }
       
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // 透明度插值
            Color newColor = sr.color;
            newColor.a = Mathf.Lerp(0f, 1f, t);
            sr.color = newColor;

            // 位置插值
            obj.transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        if (!FightFlowManager.OnGame1)
        {
            ParticleSystem ps = obj.transform.GetChild(1).GetComponent<ParticleSystem>();
            Destroy(ps.gameObject);
        }
        
        // 确保最终完全可见且位置精确
        Color finalColor = sr.color;
        finalColor.a = 1f;
        sr.color = finalColor;
        obj.transform.position = endPos;
    }
}

public static class ColorTransition
{

    public static IEnumerator FadeToColor(GameObject obj, Color targetColor, float duration = 0.3f)
    {
        if (obj == null) yield break;

        // 获取当前颜色
        Color currentColor = Color.white;
        bool hasRenderer = false;

        // 1. 尝试 UI Image
        
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                currentColor = sr.color;
                hasRenderer = true;
            }
            // 3. 尝试普通 Renderer（3D物体，需材质支持透明度）
            else
            {
                Renderer rend = obj.GetComponent<Renderer>();
                if (rend != null && rend.material != null)
                {
                    currentColor = rend.material.color;
                    hasRenderer = true;
                }
            }
        

        if (!hasRenderer)
        {
            Debug.LogWarning($"物体 {obj.name} 没有可用的 Image/SpriteRenderer/Renderer 组件，无法渐变颜色");
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Color newColor = Color.Lerp(currentColor, targetColor, t);

          
            if (obj.GetComponent<SpriteRenderer>() != null) obj.GetComponent<SpriteRenderer>().color = newColor;
            else if (obj.GetComponent<Renderer>() != null) obj.GetComponent<Renderer>().material.color = newColor;

            yield return null;
        }

        // 确保最终颜色精确
        if (obj.GetComponent<SpriteRenderer>() != null) obj.GetComponent<SpriteRenderer>().color = targetColor;
        
    }
}
