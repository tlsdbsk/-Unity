using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObstacleSpawner : MonoBehaviour
{
    public GameObject rotatingObstaclePrefab;  // 회전 방해물 프리팹
    public float spawnInterval = 3f;           // 방해물이 생성되는 간격 (초)
    public float spawnRangeX = 50f;            // 방해물이 생성될 수평 범위 (X축)
    public float spawnRangeZ = 50f;            // 방해물이 생성될 수평 범위 (Z축)
    public float spawnHeight = 1f;             // 방해물이 생성될 Y축 높이

    private void Start()
    {
        // 방해물이 일정 시간 간격으로 생성되도록 타이머 시작
        InvokeRepeating("SpawnObstacle", 0f, spawnInterval);
    }

    // 방해물 생성 함수
    private void SpawnObstacle()
    {
        // 방해물이 생성될 위치 계산 (랜덤하게 X와 Z 값을 설정)
        float spawnPosX = Random.Range(-spawnRangeX, spawnRangeX);
        float spawnPosZ = Random.Range(-spawnRangeZ, spawnRangeZ);
        Vector3 spawnPosition = new Vector3(spawnPosX, spawnHeight, spawnPosZ);  // Z값도 랜덤으로 설정

        // 방해물 생성
        Instantiate(rotatingObstaclePrefab, spawnPosition, Quaternion.identity);
    }
}
