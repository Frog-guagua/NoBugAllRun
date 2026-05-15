using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCage : MonoBehaviour
{
    [SerializeField] Transform Cage;   
    [SerializeField] GameObject Tag;

    // 改为存储局部坐标和局部缩放比例（相对于笼子）
    private Vector3 localPosition;  
    private Vector3 localScaleFactor;

    // 保留原变量以免其他地方报错（可忽略）
    private Vector3 mousePos;
    private Vector3 CageScale = new Vector3(0.1f, 0.1f, 0.1f);
    private Vector3 CagePosition = new Vector3(0f, 0f, 0f);
    private Vector3 CageNowScale = new Vector3(0.1f, 0.1f, 0.1f);
    private Vector3 CageNowPosition = new Vector3(0f, 0f, 0f);
    
    [SerializeField] private float extraScale = 1f;   // 额外放大倍数，可在 Inspector 中调整

    void Start()
    {
        if (Cage == null)
        {
            GameObject cageObj = GameObject.Find("笼子外部");
            if (cageObj != null)
                Cage = cageObj.transform;
            else
                Debug.LogError("FollowCage: 未找到名为 '笼子外部' 的物体，请检查场景中的笼子物体名称。");
        }

        // 记录当前虫子相对于笼子的局部坐标（考虑笼子的缩放、旋转、位置）
        localPosition = Cage.InverseTransformPoint(transform.position);
        
        // 记录缩放因子：虫子初始缩放 / 笼子初始缩放（保证相对比例不变）
        localScaleFactor = new Vector3(
            transform.localScale.x / Cage.localScale.x,
            transform.localScale.y / Cage.localScale.y,
            transform.localScale.z / Cage.localScale.z
        );

        Tag.SetActive(false);
        
        // 下面这些原变量保留，避免报错
        CageScale = Cage.localScale;
        CagePosition = Cage.position;
    }

    void Update()
    {
        if (CageZoom.CageHasZoomed)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f)
            );
            int layerMask = LayerMask.GetMask("Bug");
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, layerMask);
    
            CageNowPosition = Cage.position;
            CageNowScale = Cage.localScale;
            if (hit != null && hit.CompareTag("Bugs"))
            {
                if (hit.transform.childCount > 0)
                {
                    GameObject child = hit.transform.GetChild(0).gameObject;
                    child.SetActive(true);
                }
            }
            else
            {
                Tag.SetActive(false);
            }
        }
    }

    void LateUpdate()
    {
        if (Cage == null) return;

        // 位置跟随：将局部坐标转换到世界坐标，这样笼子缩放时虫子间距也会等比变化
        transform.position = Cage.TransformPoint(localPosition);
        
        // 缩放跟随：虫子缩放 = 笼子缩放 × 初始比例因子
        transform.localScale = new Vector3(
            Cage.localScale.x * localScaleFactor.x*extraScale,
            Cage.localScale.y * localScaleFactor.y*extraScale,
            Cage.localScale.z * localScaleFactor.z*extraScale
        );
    }
}