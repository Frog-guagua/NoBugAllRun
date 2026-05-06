using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManyHints : MonoBehaviour
{
    [SerializeField] List<GameObject> hints;
    
    
    void Start()
    {
        foreach (var hint in hints)
        {
            //呱：一开始先把提示透明化 然后失活 防止误触
            SpriteRenderer spriteRenderer = hint.GetComponent<SpriteRenderer>();
            Color color = spriteRenderer.color;
            color.a = 0;
            spriteRenderer.color = color;
            hint.SetActive(false);
        }
    }

  
    void Update()
    {
        
    }

    /*public IEnumerator ShowHint()
    {
        int count = 0;
        for (int i = 0; i < hints.Count; i++)
        {
            while (count < 3)
            {
                if(Input.GetKeyDown(KeyCode.Mouse0)) count++;
                
            }
        }
    }

    private IEnumerator ShowHintEffect(GameObject hint)
    {
        
    }*/
}
