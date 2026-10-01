using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;  // 방해물 프리팹
    public float spawnInterval = 5f;   // 방해물 생성 간격
    public float minX = -50f;          // 생성 가능한 최소 x 위치
    public float maxX = 50f;           // 생성 가능한 최대 x 위치
    public float minZ = -50f;          // 생성 가능한 최소 z 위치
    public float maxZ = 50f;           // 생성 가능한 최대 z 위치

    void Start()
    {
        // 첫 번째 방해물을 즉시 생성
        SpawnObstacle();

        // 일정 간격으로 방해물을 생성하는 반복 호출
        InvokeRepeating("SpawnObstacle", spawnInterval, spawnInterval);
    }

    // 방해물 생성 함수
    void SpawnObstacle()
    {
        // 무작위 위치 계산
        float spawnX = Random.Range(minX, maxX);
        float spawnZ = Random.Range(minZ, maxZ);

        // 새로운 방해물 생성
        Vector3 spawnPosition = new Vector3(spawnX, 0f, spawnZ); // y는 0으로 설정 (플로어에 배치)
        Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity); // 방해물 생성
    }
}
