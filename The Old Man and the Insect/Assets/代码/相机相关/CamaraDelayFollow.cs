using System.Collections;
using UnityEngine;

public class CamaraDelayFollow : MonoBehaviour
{
    public Transform playerTransform;      // 玩家位置

    public GameObject map;                 // 地图物体（用于自动获取边界）

    private float startX;
    private float startY;
    private float startZ;

    public float delayTime = 1f;
    public float smoothTime;               // 未使用，保留
    private bool startFollow;

    // X轴边界
    private float cameraLeftBound = -28.84f;
    private float cameraRightBound = 39.31f;
    // Y轴边界
    private float cameraBottomBound = -10f;
    private float cameraTopBound = 10f;

    public float cameraMoveSpeed = 0.1f;

    void Start()
    {
        // 自动获取地图边界（如果地图物体存在）
        if (map != null)
        {
            SpriteRenderer sr = map.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                cameraLeftBound = sr.bounds.min.x;
                cameraRightBound = sr.bounds.max.x;
                cameraBottomBound = sr.bounds.min.y;
                cameraTopBound = sr.bounds.max.y;
            }
            else
            {
                Collider2D col = map.GetComponent<Collider2D>();
                if (col != null)
                {
                    cameraLeftBound = col.bounds.min.x;
                    cameraRightBound = col.bounds.max.x;
                    cameraBottomBound = col.bounds.min.y;
                    cameraTopBound = col.bounds.max.y;
                }
            }
        }

        startX = playerTransform.position.x;
        startY = transform.position.y;
        startZ = transform.position.z;
        transform.position = new Vector3(startX, startY, startZ);

        lastPlayerX = playerTransform.position.x;
        lastPlayerY = playerTransform.position.y;

        StartCoroutine(InitialDelay());
    }

    IEnumerator InitialDelay()
    {
        yield return new WaitForSeconds(delayTime);
        startFollow = true;
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(delayTime);
        startFollow = true;
    }

    private float playerMoveDeltaX;
    private float lastPlayerX;
    private float lastPlayerY;
    private Vector3 targetCameraPos;

    void Update()
    {
        // 计算目标位置（先取玩家位置，后续边界会修正）
        targetCameraPos = new Vector3(playerTransform.position.x, playerTransform.position.y, startZ);

        // X轴边界处理
        if (playerTransform.position.x < cameraLeftBound)
        {
            targetCameraPos.x = cameraLeftBound;
            transform.position = Vector3.MoveTowards(transform.position, targetCameraPos, cameraMoveSpeed * Time.deltaTime);
        }
        else if (playerTransform.position.x > cameraRightBound)
        {
            targetCameraPos.x = cameraRightBound;
            transform.position = Vector3.MoveTowards(transform.position, targetCameraPos, cameraMoveSpeed * Time.deltaTime);
        }

        // Y轴边界处理（仿照X）
        if (playerTransform.position.y < cameraBottomBound)
        {
            targetCameraPos.y = cameraBottomBound;
            transform.position = Vector3.MoveTowards(transform.position, targetCameraPos, cameraMoveSpeed * Time.deltaTime);
        }
        else if (playerTransform.position.y > cameraTopBound)
        {
            targetCameraPos.y = cameraTopBound;
            transform.position = Vector3.MoveTowards(transform.position, targetCameraPos, cameraMoveSpeed * Time.deltaTime);
        }

        // 记录玩家上一帧位置（用于判断移动，原脚本中虽然定义但未使用，保留）
        playerMoveDeltaX = Mathf.Abs(playerTransform.position.x) - Mathf.Abs(lastPlayerX);
    }

    void LateUpdate()
    {
        lastPlayerX = playerTransform.position.x;
        lastPlayerY = playerTransform.position.y;
    }

    private void FixedUpdate()
    {
        if (startFollow)
        {
            transform.position = Vector3.Lerp(transform.position, targetCameraPos, cameraMoveSpeed);
        }
    }
}