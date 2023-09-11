using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class LivesBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject lifePrefab = null;
    private List<GameObject> lifeList = null;
    
    public void SetupLives(int maxLives)
    {
        lifeList = new List<GameObject>();

        for (int i = 0; i < maxLives; i++)
        {
            lifeList.Add(Instantiate(lifePrefab,new Vector3(0f,0f,0f),quaternion.identity,transform));
            lifeList[i].GetComponent<RectTransform>().localPosition = new Vector3(0, i * 30);
        }
    }

    public void RemoveOneLifeFromUI(int currentLifeAmount)
    {
        Destroy(lifeList[currentLifeAmount]);
        lifeList.RemoveAt(currentLifeAmount);
    }
}
