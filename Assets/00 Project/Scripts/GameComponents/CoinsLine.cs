using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CoinsLine : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField, Range(0f, 2f)] private float height;
    [SerializeField, Range(0f, 2f)] private float spaceBetweenCoins;
    [SerializeField, Range(3, 8)] private int coinsCount;
    [SerializeField] private Ease coinSpawnAnim;
    List<Item> coins = new List<Item>();
    float lenght = 0f;
    bool isShowed = false;

    private void Awake() 
    {
        lenght = (coinsCount - 1) * spaceBetweenCoins;
        for(int i = 0; i < coinsCount; i++)
        {
            Vector3 spawnPos = transform.position + Vector3.forward * (lenght * ((float)i / (float)(coinsCount - 1)));
            spawnPos.y = height + 5;
            GameObject spawnedCoin = Instantiate(coinPrefab, spawnPos, Quaternion.Euler(0f, 0f, 0f));
            spawnedCoin.SetActive(false);
            spawnedCoin.transform.parent = transform;
            coins.Add(spawnedCoin.GetComponent<Item>());
        }
    }

    public void Show()
    {
        if (isShowed)
            return;

        isShowed = true;
        StartCoroutine(ShowCoins());
    }

    IEnumerator ShowCoins()
    {
        for(int i = 0; i < coinsCount; i++)
        {
            coins[i].transform.localScale = Vector3.zero;
            coins[i].Setup(i * 0.2f);
            coins[i].gameObject.SetActive(true);
            coins[i].transform.DOScale(Vector3.one, 1f).SetEase(coinSpawnAnim);
            coins[i].transform.DOLocalMoveY(height, 1f).SetEase(coinSpawnAnim);
            yield return new WaitForSeconds(0.2f);
        }
    }

#if UNITY_EDITOR

    float pathLenght => coinPrefab.GetComponent<SineMoveAnimation>().pathLenght();
    float speed => coinPrefab.GetComponent<SineMoveAnimation>().moveSpeed();

    float lastT = 0f;

    private void OnDrawGizmos() 
    {
        lenght = (coinsCount - 1) * spaceBetweenCoins;

        Vector3 center = transform.position + Vector3.forward * lenght * 0.5f;
        center.y = height;
        Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
        Gizmos.DrawCube(center, new Vector3(1f, 1f, lenght));
        Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
        Gizmos.DrawWireCube(center, new Vector3(1f, 1f, lenght));


        //Mesh coinMesh = GameObject.CreatePrimitive(PrimitiveType.Cylinder).GetComponent<Mesh>();

        float t = (float)UnityEditor.EditorApplication.timeSinceStartup;

        for(int i = 0; i < coinsCount; i++)
        {
            center = transform.position + (lenght * ((float)i / (float)(coinsCount - 1))) * Vector3.forward;
            center.y = height + Mathf.Sin((t + 0.2f * i) * speed) * pathLenght;
            
            Gizmos.color = new Color(1f, 1f, 0f, 0.6f);
            Gizmos.DrawSphere(center, 0.3f);
        }

        UnityEditor.SceneView.RepaintAll();
        lastT = t;
    }

#endif
}
