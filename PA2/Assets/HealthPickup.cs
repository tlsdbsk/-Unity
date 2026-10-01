using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healAmount = 20f;  // 회복되는 체력 양

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // 플레이어가 충돌 시 체력 회복
            PlayerHealth playerHealth = collision.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                //playerHealth.Heal(healAmount);  // 체력 회복
                Destroy(gameObject);  // 회복 아이템을 제거
            }
        }
    }
}
