using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리

public class GameOver : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public float fallThreshold = -5f; // 떨어졌다고 판단할 Y 위치

    void Update()
    {
        // 플레이어가 떨어졌는지 확인
        if (player.position.y < fallThreshold)
        {
            EndGame(); // 게임 종료 함수 호출
        }
    }

    void EndGame()
    {
        // 게임 종료 로직 (씬을 재시작하거나 다른 씬으로 전환)
        SceneManager.LoadScene("Over"); // 현재 씬 재시작
        // 또는 Application.Quit(); // 빌드된 게임에서는 애플리케이션 종료
    }
}
