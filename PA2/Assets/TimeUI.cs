using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimeUI : MonoBehaviour
{
    public float timer = 120f; // 초기 타이머 시간 (초)
    float curTime = 0f; // 현재 타이머 시간
    string timerText;

    // GUI 스타일 설정
    GUIStyle guiStyle = new GUIStyle();

    // Start is called before the first frame update
    void Start()
    {
        curTime = timer; // 시작할 때 초기 타이머 값으로 설정
    }

    // Update is called once per frame
    void Update()
    {
        // 타이머가 0보다 큰 경우만 감소
        if (curTime > 0)
        {
            curTime -= Time.deltaTime; // 실시간 측정
        }
        else
        {
            curTime = 0; // 타이머가 0 이하로 내려가지 않도록 설정
            GameOver(); // 게임 오버 처리
        }
    }

    void OnGUI()
    {
        // GUI 스타일 설정
        guiStyle.fontSize = 40;
        guiStyle.normal.textColor = Color.red;

        // 현재 타이머 값을 문자열로 변환
        timerText = "Timer : " + Mathf.FloorToInt(curTime); // 소수점을 버림
        Rect textPos = new Rect(100, 100, 200, 40); // UI 출력을 위해 Rect 설정
        GUI.Label(textPos, timerText, guiStyle); // 타이머 UI 출력
    }

    // 타이머를 감소시키는 함수
    public void ReduceTime(float amount)
    {
        curTime -= amount;
        curTime = Mathf.Max(curTime, 0); // 타이머가 음수가 되지 않도록 설정
        Debug.Log("Time reduced! Current timer: " + curTime);
    }

    // 게임 오버 처리 함수
    private void GameOver()
    {
        Debug.Log("Game Over!");
        SceneManager.LoadScene("Over");
        // 추가적인 게임 오버 로직 (예: UI 업데이트, 게임 종료 등)
        // 예: Time.timeScale = 0; // 게임 정지
    }
}
