using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI; // Slider 사용을 위한 네임스페이스 추가

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;       // 최대 체력
    private float currentHealth;         // 현재 체력

    public Slider healthSlider;          // 체력 표시 Slider UI
    public float lowHealthThreshold = 0.3f; // 체력이 30% 이하일 때 경고 표시

    private Image sliderFillImage;       // Slider의 Fill Area 이미지 (체력 색상 변경)
    private bool isWarningActive = false; // 경고 효과가 활성화된 상태인지 여부

    void Start()
    {
        currentHealth = maxHealth;      // 초기 체력을 최대 체력으로 설정
        sliderFillImage = healthSlider.fillRect.GetComponent<Image>(); // 슬라이더의 색상을 변경할 이미지

        UpdateHealthUI();                // 초기 체력 UI 업데이트
    }

    // 기둥과 충돌 시 호출되는 함수 (isTrigger 사용 안함)
    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 객체가 기둥이라면
        if (collision.gameObject.CompareTag("Pillar"))  // 기둥의 태그가 "Pillar"라면
        {
            Debug.Log("Pillar hit! Taking damage.");  // 디버깅 메시지

            TakeDamage(10f);  // 기둥과 충돌 시 10의 피해를 받음
        }
    }

    // 기둥이 플레이어에게 피해를 입힐 때 호출되는 함수
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;        // 체력 감소
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 체력을 0~최대값으로 제한

        Debug.Log("Current Health: " + currentHealth); // 체력 변화 확인 디버깅 메시지

        UpdateHealthUI();               // 체력 UI 업데이트

        if (currentHealth <= 0)
        {
            Die();  // 체력이 0 이하일 경우 게임 오버 처리
        }
    }

    // 체력 UI 업데이트
    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            // 슬라이더의 값은 currentHealth / maxHealth로 설정 (슬라이더는 0~1 범위로 값이 설정됨)
            healthSlider.value = currentHealth / maxHealth; // 체력 비율로 Slider 업데이트
        }

        // 체력이 일정 이하일 때 경고 효과 처리
        if (currentHealth / maxHealth <= lowHealthThreshold)
        {
            ActivateLowHealthWarning(); // 경고 효과 활성화
        }
        else
        {
            DeactivateLowHealthWarning(); // 경고 효과 비활성화
        }
    }

    // 경고 효과 활성화 (슬라이더 색상 빨간색으로 변경, 깜빡이기)
    private void ActivateLowHealthWarning()
    {
        if (!isWarningActive)
        {
            isWarningActive = true;

            // 슬라이더 색상 빨간색으로 변경
            if (sliderFillImage != null)
            {
                sliderFillImage.color = Color.red;
            }

            // 깜빡이는 효과 시작
            StartCoroutine(BlinkWarningEffect());
        }
    }

    // 경고 효과 비활성화 (슬라이더 색상 기본 색으로 변경)
    private void DeactivateLowHealthWarning()
    {
        if (isWarningActive)
        {
            isWarningActive = false;

            // 슬라이더 색상을 기본 색상으로 복원
            if (sliderFillImage != null)
            {
                sliderFillImage.color = Color.green;
            }

            // 깜빡이는 효과 중지
            StopCoroutine(BlinkWarningEffect());
        }
    }

    // 깜빡이는 효과 (Coroutine 사용)
    private IEnumerator BlinkWarningEffect()
    {
        while (isWarningActive)
        {
            sliderFillImage.enabled = !sliderFillImage.enabled; // 이미지 깜빡임 (Visibility)
            yield return new WaitForSeconds(0.5f); // 0.5초마다 깜빡이기
        }
    }

    // 게임 오버 처리
    private void Die()
    {
        Debug.Log("Player has died!");
        SceneManager.LoadScene("Over");
        // 게임 오버 화면을 표시하거나 씬을 재시작하는 코드 추가 가능
    }
}
