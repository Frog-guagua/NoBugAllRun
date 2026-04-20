using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCage : MonoBehaviour
{
    [SerializeField] Transform Cage;   
    [SerializeField] GameObject Tag;

    private Vector3 localPositionOffset;  
    private Vector3 localScaleOffset;     

    void Start()
    {
        if (Cage == null)
        {
            GameObject cageObj = GameObject.Find("笼子外部");
            if (cageObj != null)
                Cage = cageObj.transform;
            else
                Debug.LogError("FollowCage: 未找到名为 'Cage' 的物体，请检查场景中的笼子物体名称。");
        }
            Tag.SetActive(false);
            localPositionOffset = transform.position - Cage.position;
            
            localScaleOffset = new Vector3(
                transform.localScale.x / Cage.localScale.x,
                transform.localScale.y / Cage.localScale.y,
                transform.localScale.z / Cage.localScale.z
            );
        
    }

    void LateUpdate()
    {
       
        if (CageZoom.CageHasZoomed)
        {
            Tag.SetActive(true);
        }
        else
        {
            Tag.SetActive(false);
        }
        
        transform.position = Cage.position + localPositionOffset;
        
        transform.localScale = new Vector3(
            Cage.localScale.x * localScaleOffset.x,
            Cage.localScale.y * localScaleOffset.y,
            Cage.localScale.z * localScaleOffset.z
        );
        
        
    }
}

