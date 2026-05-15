using UnityEngine;

public class CloudSequentialLoop : MonoBehaviour
{
    [Header("云图")]
    public SpriteRenderer cloudA;
    public SpriteRenderer cloudB;

    [Header("移动参数")]
    public float moveSpeed = 1.5f;          // 左移速度
    public float delayDistance = 3f;        // cloudA 移动多远后 cloudB 开始出场
    public float gap = 0f;                  // 两张云之间的间距（0 表示紧贴）

    [Header("形变（可选）")]
    public bool enableDeformation = true;
    public float stretchIntensity = 0.1f;
    public float stretchSpeed = 1.5f;

    private float cloudWidth;               // 单张云的宽度（世界单位）
    private Vector3 originalScaleA, originalScaleB;
    private bool isBActive = false;
    private float aMovedDistance = 0f;

    private float cameraLeftBound;          // 相机左边界
    private float cameraRightBound;         // 相机右边界

    void Start()
    {
        Camera cam = Camera.main;
        if (!cam.orthographic)
        {
            Debug.LogError("需要正交相机");
            return;
        }

        // 计算相机世界边界
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        cameraLeftBound = cam.transform.position.x - camWidth / 2f;
        cameraRightBound = cam.transform.position.x + camWidth / 2f;

        // 获取云图实际宽度（考虑初始缩放）
        cloudWidth = cloudA.sprite.bounds.size.x * cloudA.transform.localScale.x;

        // 设置 cloudA 初始位置：左边缘与相机左边界重合
        // 因为 SpriteRenderer 的中心点就是图片的中心，所以位置 = 左边界 + 半宽
        float startX = cameraLeftBound + cloudWidth / 2f;
        cloudA.transform.position = new Vector3(startX, cloudA.transform.position.y, 0);
        cloudA.gameObject.SetActive(true);

        // cloudB 初始放在右侧远处（相机右边界右侧一个屏幕宽度），暂时禁用
        float initB_X = cameraRightBound + cloudWidth / 2f + cloudWidth + gap;
        cloudB.transform.position = new Vector3(initB_X, cloudB.transform.position.y, 0);
        cloudB.gameObject.SetActive(false);

        originalScaleA = cloudA.transform.localScale;
        originalScaleB = cloudB.transform.localScale;

        isBActive = false;
        aMovedDistance = 0f;
    }

    void Update()
    {
        float step = moveSpeed * Time.deltaTime;

        // 1. cloudA 持续左移
        cloudA.transform.Translate(Vector3.left * step);
        aMovedDistance += step;

        // 2. 当 cloudA 移动了足够距离且 cloudB 尚未激活时，激活 cloudB 并放到 cloudA 的右侧紧贴位置
        if (!isBActive && aMovedDistance >= delayDistance)
        {
            isBActive = true;
            cloudB.gameObject.SetActive(true);
            // 让 cloudB 的左边缘紧贴 cloudA 的右边缘（或根据 gap 留间距）
            cloudB.transform.position = new Vector3(cloudA.transform.position.x + cloudWidth + gap, cloudA.transform.position.y, 0);
        }

        // 3. 如果 cloudB 已激活，则同步左移
        if (isBActive)
        {
            cloudB.transform.Translate(Vector3.left * step);
        }

        // 4. 正弦波形变
        if (enableDeformation)
        {
            float factor = 1f + Mathf.Sin(Time.time * stretchSpeed) * stretchIntensity;
            cloudA.transform.localScale = originalScaleA * factor;
            if (isBActive) cloudB.transform.localScale = originalScaleB * factor;
        }

        // 5. 边界重置：当某张云的右边缘移出相机左边界（即整张云完全不可见）时，将其放到另一张云的右侧
        float leftBoundExit = cameraLeftBound - cloudWidth;  // 完全移出屏幕左侧的阈值
        if (cloudA.transform.position.x + cloudWidth / 2f < leftBoundExit)
        {
            if (isBActive)
            {
                // 放到 cloudB 的右侧
                float newX = cloudB.transform.position.x + cloudWidth + gap;
                cloudA.transform.position = new Vector3(newX, cloudA.transform.position.y, 0);
            }
            else
            {
                // 如果 cloudB 还未激活（一般不会），重置到初始位置
                float resetX = cameraLeftBound + cloudWidth / 2f;
                cloudA.transform.position = new Vector3(resetX, cloudA.transform.position.y, 0);
            }
            // 重置移动距离（避免再次触发延迟）
            aMovedDistance = 0f;
        }

        if (isBActive && cloudB.transform.position.x + cloudWidth / 2f < leftBoundExit)
        {
            float newX = cloudA.transform.position.x + cloudWidth + gap;
            cloudB.transform.position = new Vector3(newX, cloudA.transform.position.y, 0);
        }
    }
}