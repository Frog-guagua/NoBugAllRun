using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InsectWorldSpawner : MonoBehaviour
{
    public List<GameObject> spawnPoints;
    public GameObject insectPrefab;

    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        if (insectPrefab == null)
        {
            Debug.LogError("[Spawner] Prefab 没拖！");
            return;
        }

        List<InsectData> dataList = CageManager.Instance.insectDataList;

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            GameObject bug = Instantiate(insectPrefab, spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
            bug.transform.SetParent(spawnPoints[i].transform, true);

            InsectData cageData = dataList[i];
            InsectData bugData = bug.GetComponent<InsectData>();

            bugData.insectId    = cageData.insectId;
            bugData.insectHP    = cageData.insectHP;
            bugData.insectAtk   = cageData.insectAtk;
            bugData.insectLevel = cageData.insectLevel;
            bugData.bugType     = cageData.bugType;
            bugData.insectImage = cageData.insectImage;
            bugData.isCompound  = cageData.isCompound;

            // 更新图标（按你项目情况二选一）
            // 方式A：2D场景用 SpriteRenderer
            SpriteRenderer sr = bug.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.sprite = cageData.insectImage;

            // 方式B：UI 用 Image（如果你虫虫是 UI 对象，取消下面注释）
            // Image img = bug.GetComponentInChildren<<Image>();
            // if (img != null) img.sprite = cageData.insectImage;

            // id为0 → 隐藏，不触发点击
            if (cageData.insectId == 0)
                bug.SetActive(false);

            // 绑定槽位索引
            bug.GetComponent<InsectWorldEntity>().cageSlotIndex = i;
        }
    }
}