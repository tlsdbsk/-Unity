using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickupSpawner : MonoBehaviour
{
    public GameObject healthPickupPrefab;    // 회복 아이템 프리팹
    public float spawnInterval = 5f;         // 아이템이 생성되는 간격 (초)
    public Vector2 spawnRangeX = new Vector2(-50f, 50f); // X축 생성 범위
    public Vector2 spawnRangeZ = new Vector2(-50f, 50f); // Z축 생성 범위

    void Start()
    {
        // 주기적으로 아이템 생성
        InvokeRepeating("SpawnHealthPickup", 0f, spawnInterval);
    }

    // 회복 아이템을 무작위 위치에 생성
    void SpawnHealthPickup()
    {
        // 무작위 위치 계산
        float randomX = Random.Range(spawnRangeX.x, spawnRangeX.y);
        float randomZ = Random.Range(spawnRangeZ.x, spawnRangeZ.y);
        Vector3 spawnPosition = new Vector3(randomX, 0.5f, randomZ); // Y값은 0.5f로 설정 (지면 위에 놓이도록)

        // 회복 아이템 생성
        Instantiate(healthPickupPrefab, spawnPosition, Quaternion.identity);
    }
}
