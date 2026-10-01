using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacleSpawner : MonoBehaviour
{
    public GameObject movingObstaclePrefab; // 방해물 프리팹
    public float spawnInterval = 10f;       // 방해물 생성 간격

    private void Start()
    {
        // 10초마다 방해물 생성
        InvokeRepeating("SpawnMovingObstacle", 0f, spawnInterval);
    }

    private void SpawnMovingObstacle()
    {
        // 무작위 위치에 방해물 생성
        float randomX = Random.Range(-50f, 50f);
        float randomZ = Random.Range(-50f, 50f);
        Vector3 spawnPosition = new Vector3(randomX, 0.5f, randomZ); // Y축 0.5로 설정 (바닥 위에 생성)

        // 방해물 생성
        GameObject obstacle = Instantiate(movingObstaclePrefab, spawnPosition, Quaternion.identity);
        ResettingMovingObstacle resettingObstacle = obstacle.GetComponent<ResettingMovingObstacle>();
        resettingObstacle.spawnPosition = spawnPosition;  // 생성된 방해물의 시작 위치 설정
    }
}
