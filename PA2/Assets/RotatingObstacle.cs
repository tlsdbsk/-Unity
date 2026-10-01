using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObstacle : MonoBehaviour
{
    public float rotationSpeed = 90f;  // 회전 속도 (도/초)
    public float lifetime = 10f;       // 방해물이 살아있는 시간 (초)
    public float damage = 20f;         // 방해물이 플레이어에게 입힐 피해

    private void Start()
    {
        // 일정 시간 후 방해물 제거
        Destroy(gameObject, lifetime);  // 10초 후에 방해물 객체를 제거
    }

    private void Update()
    {
        // 회전 처리
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);  // Y축을 기준으로 회전 (수직 회전)
    }

    // 충돌 처리 (플레이어가 방해물과 충돌할 경우)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))  // 플레이어와 충돌했을 경우
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);  // 방해물에 닿으면 피해 입힘
            }
        }
    }
}
