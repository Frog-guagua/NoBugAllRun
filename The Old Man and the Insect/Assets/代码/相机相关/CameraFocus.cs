using System.Collections;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    //呱： 请使用这个美味的 相机聚焦 函数！
    public void LetCameraFocus()
    {
        StopAllCoroutines();
        StartCoroutine(MoveRoutine());
    }

    #region 实现

    [SerializeField] Vector2 goalPos;         
    [SerializeField] float goalSize = 5f;      
    [SerializeField] float moveDuration = 1f;    
    [SerializeField] float stayDuration = 1f;    
    [SerializeField] Camera camara;
    private Vector3 originalPos;
    private float originalSize;
    

    void Start()
    {
        camara = camara.GetComponent<Camera>();
        originalPos = camara.transform.position;
        originalSize = camara.orthographicSize;
        
        
        //呱：懒得写个调试代码了 注释掉就行了
        LetCameraFocus();
    }

   

    IEnumerator MoveRoutine()
    {
        
        float t = 0;
        
        //呱：相机靠近物体 并且开始放大
        Vector3 startPos = camara.transform.position;
        float startSize = camara.orthographicSize;
        Vector3 target = new Vector3(goalPos.x, goalPos.y, originalPos.z);

        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float p = t / moveDuration;
            camara.transform.position = Vector3.Lerp(startPos, target, p);
            camara.orthographicSize = Mathf.Lerp(startSize, goalSize, p);
            yield return null;
        }
        camara.transform.position = target;
        camara.orthographicSize = goalSize;

        
        yield return new WaitForSeconds(stayDuration);

        //呱：相机远离物体 并且开始缩回原先的尺寸
        t = 0;
        startPos = camara.transform.position;
        startSize = camara.orthographicSize;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float p = t / moveDuration;
            camara.transform.position = Vector3.Lerp(startPos, originalPos, p);
            camara.orthographicSize = Mathf.Lerp(startSize, originalSize, p);
            yield return null;
        }
        camara.transform.position = originalPos;
        camara.orthographicSize = originalSize;
    }

    #endregion
    
}