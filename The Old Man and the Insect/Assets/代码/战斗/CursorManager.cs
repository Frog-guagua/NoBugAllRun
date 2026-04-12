using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
   
    [Header("光标图")]
    [Tooltip("这里拖入手的张开的图")][SerializeField] Texture2D handNormal;
    [Tooltip("这里拖入手的捏紧的图")][SerializeField] Texture2D handSeize;

    private Vector2 ClickPoint;
  
    
    void Start()
    {
        ClickPoint =new Vector2((float)handNormal.width / 2, (float)handNormal.height / 2);
        Cursor.SetCursor(handNormal,ClickPoint, CursorMode.Auto);
    }

   
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            SwitchToSeizeHnad();
        }
        else
        {
            SwitchToNormalHnad();
        }
    
       
    }
    
    void SwitchToSeizeHnad()
    {
       
            ClickPoint =new Vector2((float)handSeize.width / 2, (float)handSeize.height / 2);
            Cursor.SetCursor(handSeize,ClickPoint, CursorMode.Auto);
        
    }

    void SwitchToNormalHnad()
    {
        ClickPoint =new Vector2((float)handNormal.width / 2, (float)handNormal.height / 2);
        Cursor.SetCursor(handNormal,ClickPoint, CursorMode.Auto);
    }
    
}
