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
    public void FindRival(int GridIndex)
    {
        int targetRivalIndex = 0;
        Vector2 origin = GridManager.Grids[GridIndex].bugOnGrid.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 5, targetLayer);
        if(hit.collider == null)
        {
            for(int i =4; i<8; i++)
            {
                Vector2 start = GridManager.Grids[i].bugOnGrid.transform.position;
                RaycastHit2D hit2 = Physics2D.Raycast(origin, Vector2.up, 5, targetLayer);
                if (hit2.collider == null)
                {
                    targetRivalIndex = i;
                    break;
                }
            }

            GridManager.Grids[GridIndex].isOccupied = false;
            GridManager.Grids[GridIndex].bugOnGrid.transform.position = GridManager.Grids[targetRivalIndex+4].matchedPos+offset;
            GridManager.Grids[targetRivalIndex+4].isOccupied = true;
        }
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
}
