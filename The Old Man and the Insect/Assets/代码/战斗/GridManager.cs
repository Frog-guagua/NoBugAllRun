using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//呱： 写在最前面 这个脚本挂在 空物体“格子管理器”上面
public class GridManager : MonoBehaviour
{
    public GameObject[] grids = new GameObject[16];
   
    private bool[] isOnGrid = new bool[16];
    [SerializeField]GameObject fightManager;
    public static int nowGridIndex;
    
    void Start()
    {
     
    
    }

    void Update()
    {
        
    }

   
    //呱：用来判断 放置虫子时  虫子在哪个格子上空
    public int OnWhichGrid()
    {

        #region 射线检测前置

        Vector3 mousePos =
            Camera.main.ScreenToWorldPoint
                (new Vector3(Input.mousePosition.x, Input.mousePosition.y, grids[0].transform.position.z));
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        #endregion

        #region Debug ：已解决 我受不了了 死射线你到底打中了什么 ……

           //呱：Debug[已解决 ：我受不了了 死射线你到底打中了什么 ……]
           //Debug.Log("击中物体：" + (hit.collider ? hit.collider.name : "无"));
           
        #endregion
        
        for (int i = 0; i < grids.Length; i++)
        { 
            if (hit.collider == grids[i].GetComponent<Collider2D>())
            {
                isOnGrid[i] = true;
                nowGridIndex = i;
                return nowGridIndex;
            }
        }

        CageZoom.CageHasZoomed = false;
        return -1;
    }

   
    
}
