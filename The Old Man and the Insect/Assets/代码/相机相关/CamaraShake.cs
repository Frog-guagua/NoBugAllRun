using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//呱：写在最前面 ——> 挂载在 Main Camara上 
public class CamaraShake : MonoBehaviour
{
   
    //呱：记住这个美味函数的名字吧！
    //呱：妙妙工具嗷嗷嗷啊啊啊啊
    public void ShakeStart(float duration, float strength)
    {
        //呱：考虑到 在老头家里的时候 是相机是没有挂载 延迟跟随的 所以我们搞个 空值检查
        //呱：这样就很通用了 hh!
        if (camaraDelayFollow != null)
        {
            StopAllCoroutines();
            StartCoroutine(Shake(duration, strength));
        }
    }
    public static void ShakeCameraInDialogue(float duration,float strength)
    {
        //我默认将这个代码挂在主相机上
        //这是失败产出的rubbish
        //但好像也能用。
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        CamaraShake shake = mainCam.GetComponent<CamaraShake>();
        if (shake != null)
        {
            shake.ShakeStart(duration, strength);
        }
    }

    #region 实现逻辑

    private Vector3 originalPos;
    private CamaraDelayFollow camaraDelayFollow;
    
    
    void Start()
    { 
        camaraDelayFollow =GetComponent<CamaraDelayFollow>();
        originalPos = transform.localPosition;
    }


    void Update()
    {
        
    }

   

    IEnumerator Shake(float duration, float strength)
    {
       
        //呱：有没有挂载美味跟随脚本？！ ！？
        if (camaraDelayFollow != null)
        {
            camaraDelayFollow.enabled = false;
        }
        else
        {
            yield return null;
        }
        
        float timeCount = 0.0f;
        
        while (timeCount < duration)
        {

            //呱：古法抖动 还得是随机数
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;
            transform.localPosition = originalPos + new Vector3(x, y, 0);
            timeCount += Time.deltaTime;
            yield return null;
        }
  
        transform.localPosition = originalPos;
    }

    #endregion
    
    
}
