using System.Collections.Generic;
using UnityEngine;

public class ActionPoint : MonoBehaviour
{
    [Tooltip("行动点")]
    public List<GameObject> pointObjects;

    void Start()
    {
        UpdatePoints(FightDataManager.ActionPoints);
    }

   

    public void UpdatePoints(int currentPoints)
    {
        if (pointObjects == null) return;
        for (int i = 0; i < pointObjects.Count; i++)
        {
            bool shouldActive = i < currentPoints;
            if (pointObjects[i] != null)
                pointObjects[i].SetActive(shouldActive);
        }
    }
}