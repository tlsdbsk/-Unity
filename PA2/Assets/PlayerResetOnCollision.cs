using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResetOnCollision : MonoBehaviour
{
    public Vector3 startPosition;  // 플레이어의 시작 위치
    public float damage = 20f;     // 플레이어가 방해물에 충돌할 때 입을 피해

    private void Start()
    {
        // 시작 위치 설정 (게임 시작 시 현재 위치를 시작 위치로 지정)
        startPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cylinder"))  // 방해물과 충돌한 경우
        {
            // 플레이어 체력 감소
            PlayerHealth playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // 방해물에 의한 피해
            }

            // 플레이어 위치를 시작 지점으로 리셋
            transform.position = startPosition;

            // 필요한 경우 애니메이션이나 효과 추가 가능
        }
    }
}

