using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Clear : MonoBehaviour
{
    public float targetX = 48f; // 목표 x 좌표
    public float targetZ = 48f; // 목표 z 좌표


    void Update()
    {
        // 플레이어의 x, z 위치가 각각 목표 값 이상일 때
        if (transform.position.x >= targetX && transform.position.z >= targetZ)
        {
            SceneManager.LoadScene("Clear"); 
            Debug.Log("Player reached the target position! Loading the clear scene..."); // 목표 위치에 도달하면 씬 전환
        }
    }

}
