using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleResover : MonoBehaviour
{
    [Header("格子管理器引用")]
    public GridManager gridManager;

    [Header("动画参数")]
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float moveDuration = 0.5f;

    private Vector3 offset = Vector3.zero; // 如果你的格子位置有偏移，在这里设置

    /// <summary>
    /// 战斗结算主协程（攻击 + 死亡 + 胜负 + 后排前移）
    /// </summary>
    public IEnumerator BattleResolve()
    {
        // 1. 获取双方前排虫子（索引 4-7 为我方前排，8-11 为敌方前排）
        List<InsectData> myFrontBugs = new List<InsectData>();
        List<InsectData> enemyFrontBugs = new List<InsectData>();

        for (int i = 4; i <= 7; i++)
        {
            if (GridManager.Grids[i].bugOnGrid != null)
                myFrontBugs.Add(GridManager.Grids[i].bugOnGrid.GetComponent<InsectData>());
        }
        for (int i = 8; i <= 11; i++)
        {
            if (GridManager.Grids[i].bugOnGrid != null)
                enemyFrontBugs.Add(GridManager.Grids[i].bugOnGrid.GetComponent<InsectData>());
        }

        // 2. 计算伤害（只考虑前排对前排，按索引对应）
        Dictionary<InsectData, int> damageMap = new Dictionary<InsectData, int>();
        int count = Mathf.Min(myFrontBugs.Count, enemyFrontBugs.Count);
        for (int i = 0; i < count; i++)
        {
            InsectData myBug = myFrontBugs[i];
            InsectData enemyBug = enemyFrontBugs[i];
            if (myBug != null && enemyBug != null)
            {
                // 我方攻击敌方
                if (!damageMap.ContainsKey(enemyBug)) damageMap[enemyBug] = 0;
                damageMap[enemyBug] += myBug.insectAtk;
                // 敌方攻击我方
                if (!damageMap.ContainsKey(myBug)) damageMap[myBug] = 0;
                damageMap[myBug] += enemyBug.insectAtk;
            }
        }

        // 3. 统一扣血
        foreach (var kvp in damageMap)
        {
            kvp.Key.insectHP -= kvp.Value;
            Debug.Log($"{kvp.Key.name} 受到 {kvp.Value} 伤害，剩余 HP：{kvp.Key.insectHP}");
        }

        // 4. 刷新所有虫子头顶的 UI
        foreach (var bug in myFrontBugs) UpdateBugUI(bug);
        foreach (var bug in enemyFrontBugs) UpdateBugUI(bug);

        // 5. 处理死亡（摇晃消失）
        List<Coroutine> deathRoutines = new List<Coroutine>();
        for (int i = 4; i <= 7; i++)
        {
            GameObject bugObj = GridManager.Grids[i].bugOnGrid;
            if (bugObj != null)
            {
                InsectData bug = bugObj.GetComponent<InsectData>();
                if (bug != null && bug.insectHP <= 0)
                    deathRoutines.Add(StartCoroutine(ShakeAndFade(bugObj)));
            }
        }
        for (int i = 8; i <= 11; i++)
        {
            GameObject bugObj = GridManager.Grids[i].bugOnGrid;
            if (bugObj != null)
            {
                InsectData bug = bugObj.GetComponent<InsectData>();
                if (bug != null && bug.insectHP <= 0)
                    deathRoutines.Add(StartCoroutine(ShakeAndFade(bugObj)));
            }
        }
        foreach (var routine in deathRoutines) yield return routine;

        // 6. 胜负判定（检查所有格子 0-7 和 8-15）
        bool hasMyBug = false;
        bool hasEnemyBug = false;
        for (int i = 0; i <= 7; i++)
        {
            if (GridManager.Grids[i].bugOnGrid != null) { hasMyBug = true; break; }
        }
        for (int i = 8; i <= 15; i++)
        {
            if (GridManager.Grids[i].bugOnGrid != null) { hasEnemyBug = true; break; }
        }

        if (!hasMyBug)
        {
            Debug.Log("玩家失败！");
            // 这里可以触发失败 UI、场景切换等
            yield break;
        }
        if (!hasEnemyBug)
        {
            Debug.Log("玩家胜利！");
            yield break;
        }

        // 7. 没有结束 → 后排前移（我方后排虫子向前排移动）
        for (int i = 0; i <= 3; i++)  // 后排索引 0-3
        {
            GameObject backBug = GridManager.Grids[i].bugOnGrid;
            if (backBug == null) continue;

            int frontIndex = i + 4;
            if (frontIndex < GridManager.Grids.Length && GridManager.Grids[frontIndex].bugOnGrid == null)
            {
                // 移动虫子到前排
                yield return StartCoroutine(MoveBug(backBug, GridManager.Grids[frontIndex].matchedPos + offset));
                // 更新格子占用
                GridManager.Grids[i].bugOnGrid = null;
                GridManager.Grids[i].isOccupied = false;
                GridManager.Grids[frontIndex].bugOnGrid = backBug;
                GridManager.Grids[frontIndex].isOccupied = true;
            }
        }
    }

    // 移动虫子的协程（平滑移动）
    private IEnumerator MoveBug(GameObject bug, Vector3 targetPos)
    {
        // 临时禁用 FollowCage 避免干扰
        FollowCage follow = bug.GetComponent<FollowCage>();
        if (follow != null) follow.enabled = false;

        Vector3 startPos = bug.transform.position;
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = moveCurve.Evaluate(elapsed / moveDuration);
            bug.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        bug.transform.position = targetPos;

        if (follow != null) follow.enabled = true;
    }

    // 摇晃消失（复用你的逻辑）
    private IEnumerator ShakeAndFade(GameObject obj)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Vector3 originalPos = obj.transform.position;
        float elapsed = 0f;
        float shakeDur = 0.3f;
        while (elapsed < shakeDur)
        {
            float offsetX = Random.Range(-0.3f, 0.3f);
            float offsetY = Random.Range(-0.3f, 0.3f);
            obj.transform.position = originalPos + new Vector3(offsetX, offsetY, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = originalPos;

        Color color = sr.color;
        elapsed = 0f;
        float fadeDur = 0.5f;
        while (elapsed < fadeDur)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDur);
            color.a = alpha;
            sr.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.SetActive(false);
    }

    // 更新虫子头顶文字（复用你的方法）
    private void UpdateBugUI(InsectData bug)
    {
        if (bug == null) return;
        TextMeshProUGUI tmp = bug.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = $"{bug.insectHP}\n\n\n{bug.insectAtk}";
    }
}