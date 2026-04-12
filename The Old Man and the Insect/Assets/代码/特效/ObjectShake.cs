using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//呱：写在最前面 ——> 记得拖想要抖动的物体进来
public class ObjectShake : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] float strength;
    //呱：请用 妙妙函数方法吧！！！！！
    public void ShakeStart(float duration, float strength)
    {
       
            StopAllCoroutines();
            StartCoroutine(Shake(duration, strength));
            
        
    }

    //呱：专门 给 敲门！ 写的！ 美味！ 方法！
    //吼吼吼吼吼吼吼吼吼吼吼吼吼吼吼吼
    public void DoorShake(float duration = 0.5f, float strength = 0.05f)
    {
       
            StopAllCoroutines();
            StartCoroutine(Wait());

        
    }
    
    #region 实现

    [SerializeField]GameObject shakeObject;
    private Vector3 originalPos;
   
    void Start()
    {
        originalPos = shakeObject.transform.localPosition;
        
        //呱：实验用 
        //ShakeStart(3, 0.05f);
        //DoorShake();
    }

   
    void Update()
    {
        
    }
    
    IEnumerator Shake(float duration, float strength)
    {
        float timeCount = 0.0f;
      
        while (timeCount < duration)
        {

            //呱：古法抖动 还得是随机数
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;
            shakeObject.transform.localPosition = originalPos + new Vector3(x, y, 0);
            timeCount += Time.deltaTime;
            yield return null;
        }
  
        shakeObject.transform.localPosition = originalPos;
       
       
    }

    IEnumerator Wait()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return StartCoroutine(Shake(duration, strength));
            yield return new WaitForSeconds(0.3f);
        }
    }


        #endregion
        
}
