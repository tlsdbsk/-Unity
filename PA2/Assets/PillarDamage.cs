using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarDamage : MonoBehaviour
{
    public float damage = 10f;  // 기둥이 주는 피해량

    // 플레이어와 충돌 시
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // 플레이어에게 피해를 입힘
            }
        }
    }
}

