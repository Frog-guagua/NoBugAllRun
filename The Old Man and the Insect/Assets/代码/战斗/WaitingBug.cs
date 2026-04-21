using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float moveDuration = 1f;
    private int gridIndex;
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
                    GridManager.Grids[GridIndex].bugOnGrid = WaitingBugs[BugIndex];
                }
                //呱：如果前方有虫 
                else
                {
                    //呱 ：需要合成
                    if (NeedCompound(WaitingBugs[BugIndex], GridManager.Grids[GridIndex-4].bugOnGrid))
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
                        
                 
                        StartCoroutine(MoveAndDisable(targetPos, WaitingBugs[BugIndex], moveCurveForLevelUp));
                        //if (!needCompound) return;

                    }
                    //呱：不需要合成
                    else
                    {
                        WaitingBugs[BugIndex].transform.position = GridManager.Grids[realIndex].matchedPos+offset;
                        GridManager.Grids[realIndex].isOccupied = true;
                        GridManager.Grids[realIndex].bugOnGrid = WaitingBugs[realIndex];
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
        WaitingBugs[BugIndex].GetComponent<SpriteRenderer>().enabled = true;
        WaitingBugs[BugIndex].transform.GetChild(0).gameObject.SetActive(true);

        
    }

    //呱 ：这是敌方索敌
    //呱 ：这是敌方索敌（寻找可攻击的目标，并移动到对应位置）
//呱 ：这是敌方索敌（动态移动）
public IEnumerator FindRival(int GridIndex)
{
    // 1. 边界检查
    if (GridIndex < 0 || GridIndex >= GridManager.Grids.Length)
    {
        Debug.LogError($"FindRival: 无效的 GridIndex {GridIndex}");
        yield break;
    }

    // 2. 获取当前虫子
    GameObject currentBug = GridManager.Grids[GridIndex].bugOnGrid;
    if (currentBug == null)
    {
        Debug.LogError($"FindRival: GridIndex {GridIndex} 的 bugOnGrid 为空");
        yield break;
    }

    // 3. 临时禁用 FollowCage（如果有），避免干扰移动
    FollowCage follow = currentBug.GetComponent<FollowCage>();
    if (follow != null) follow.enabled = false;

    // 4. 确定当前虫子所在的行和列（4列 x 4行）
    int column = GridIndex % 4;
    int row = GridIndex / 4;

    // 5. 优先攻击正前方（同一列）
    int targetRow = row - 1;
    if (targetRow >= 0)
    {
        int targetIndex = targetRow * 4 + column;
        if (targetIndex >= 0 && targetIndex < GridManager.Grids.Length)
        {
            GameObject targetBug = GridManager.Grids[targetIndex].bugOnGrid;
            if (targetBug != null)
            {
                // 正前方有敌人，直接攻击（这里先恢复 FollowCage 并退出）
                if (follow != null) follow.enabled = true;
                Debug.Log($"FindRival: 找到正前方敌人，格子 {targetIndex}");
                yield break;
            }
        }
    }

    // 6. 没有正前方敌人，寻找可移动的空位（我方前排）
    int targetGridIndex = -1;
    for (int i = 0; i < GridManager.Grids.Length; i++)
    {
        int r = i / 4;
        if (r != 2 && r != 3) continue; // 假设我方前排行索引 2,3
        if (GridManager.Grids[i].isOccupied) continue;
        targetGridIndex = i;
        break;
    }

    if (targetGridIndex == -1)
    {
        Debug.LogWarning("FindRival: 未找到可移动的空位");
        if (follow != null) follow.enabled = true;
        yield break;
    }

    // 7. 执行平滑移动
    Vector3 startPos = currentBug.transform.position;
    Vector3 targetPos = GridManager.Grids[targetGridIndex].matchedPos + offset;
    float moveDuration = 0.5f; // 移动耗时，可调
    float elapsed = 0f;
    while (elapsed < moveDuration)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / moveDuration);
        // 使用 AnimationCurve 使移动更生动（你已有 moveCurve 变量，可在此传入）
        float curveValue = moveCurve.Evaluate(t); // 假设你有一个 AnimationCurve 类型的曲线
        currentBug.transform.position = Vector3.Lerp(startPos, targetPos, curveValue);
        yield return null;
    }
    currentBug.transform.position = targetPos;

    // 8. 更新格子占用信息
    GridManager.Grids[GridIndex].isOccupied = false;
    GridManager.Grids[GridIndex].bugOnGrid = null;

    GridManager.Grids[targetGridIndex].isOccupied = true;
    GridManager.Grids[targetGridIndex].bugOnGrid = currentBug;

    // 9. 恢复 FollowCage
    if (follow != null) follow.enabled = true;

    Debug.Log($"FindRival: 虫子从格子 {GridIndex} 平滑移动到格子 {targetGridIndex}");
    yield return null;
}
    
    IEnumerator MoveAndDisable(Vector3 endPos, GameObject fightBug,AnimationCurve curve)
    {
   
        yield return Move(endPos, fightBug,curve);
        
        if (needCompound)
        {
            
            AudioMgr.Instance.PlaySFX(levelUpSound);
            levelUpParticles.Play();
            Destroy(fightBug);
           
            Camera.main.GetComponent<CamaraShake>().ShakeStart(0.5f,0.3f);
            
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

}
