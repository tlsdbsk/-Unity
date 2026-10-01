using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResettingMovingObstacle : MonoBehaviour
{
    public float moveSpeed = 5f;      // 방해물 이동 속도
    public float damage = 20f;        // 플레이어에게 입힐 피해
    public Vector3 spawnPosition;     // 방해물의 초기 생성 위치

    private Vector3 startPosition;    // 방해물의 시작 위치
    private bool isMoving = false;    // 방해물의 이동 상태

    private void Start()
    {
        startPosition = transform.position;  // 시작 지점 설정
        spawnPosition = new Vector3(Random.Range(-50f, 50f), 0.5f, Random.Range(-50f, 50f)); // 무작위 생성 위치
        StartCoroutine(SpawnAndResetRoutine()); // 생성 및 리셋을 위한 코루틴 시작
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveObstacle();
        }
    }

    private void MoveObstacle()
    {
        transform.position = Vector3.MoveTowards(transform.position, spawnPosition, moveSpeed * Time.deltaTime);

        // 목표 위치에 도달하면 이동을 멈추고 다시 시작 위치로 돌아갑니다.
        if (transform.position == spawnPosition)
        {
            isMoving = false;
            spawnPosition = startPosition;  // 목표 지점 도달 후 시작 지점으로 리셋
        }
    }

    // 방해물 위치 초기화 및 일정 간격으로 무작위 위치에서 생성
    private IEnumerator SpawnAndResetRoutine()
    {
        while (true)
        {
            // 10초마다 방해물 리셋
            yield return new WaitForSeconds(10f);
            spawnPosition = new Vector3(Random.Range(-50f, 50f), 0.5f, Random.Range(-50f, 50f)); // 무작위 위치 지정
            isMoving = true;  // 이동 시작
        }
    }

    // 충돌 시 플레이어에게 피해를 주고, 시작 지점으로 돌아가도록 설정
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))  // 플레이어와 충돌
        {
            // 플레이어 체력 감소
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // 방해물 시작 지점으로 리셋
            transform.position = startPosition;
            spawnPosition = startPosition; // 시작 지점으로 리셋
            isMoving = false;  // 이동을 멈추고
        }
    }
}

