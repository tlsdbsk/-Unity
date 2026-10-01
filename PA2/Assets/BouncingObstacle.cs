using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingObstacle : MonoBehaviour
{
    public float moveSpeed = 5f;              // 방해물의 이동 속도
    public float bounceForce = 10f;           // 플레이어가 충돌했을 때 튕겨나는 힘
    public float minX = -50f;                 // 방해물의 최소 x 위치 (플로어 범위)
    public float maxX = 50f;                  // 방해물의 최대 x 위치 (플로어 범위)
    public float minZ = -50f;                 // 방해물의 최소 z 위치 (플로어 범위)
    public float maxZ = 50f;                  // 방해물의 최대 z 위치 (플로어 범위)

    private Rigidbody rb;                     // 방해물의 Rigidbody 컴포넌트
    private Vector3 moveDirection;            // 이동 방향

    void Start()
    {
        // 방해물의 시작 위치를 무작위로 설정
        transform.position = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minZ, maxZ));

        // Rigidbody 컴포넌트 가져오기
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;  // 물리 엔진의 영향을 받지 않도록 설정 (충돌 처리용)
        }

        // 방해물의 이동 방향을 무작위로 설정 (좌우)
        moveDirection = Vector3.right * (Random.Range(0, 2) == 0 ? -1f : 1f); // 좌측 또는 우측으로 이동
    }

    void Update()
    {
        // 방해물 좌우로 이동
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // x 범위 벗어나면 이동 방향 반전
        if (transform.position.x <= minX || transform.position.x >= maxX)
        {
            moveDirection = -moveDirection; // 이동 방향 반전
        }
    }

    // 플레이어와 충돌했을 때 호출되는 함수
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // 플레이어와 충돌하면
        {
            // 플레이어의 Rigidbody를 가져와 튕겨나가게 함
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 bounceDirection = collision.transform.position - transform.position; // 반대 방향
                playerRb.AddForce(bounceDirection.normalized * bounceForce, ForceMode.Impulse); // 튕겨나가는 힘을 줌
            }
        }
    }
}

