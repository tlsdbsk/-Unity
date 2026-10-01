using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class RandomPillarGenerator : MonoBehaviour
{
    public GameObject pillarPrefab;   // 생성할 기둥 프리팹
    public Vector2 floorSize = new Vector2(100, 100); // 플로어 크기 (X, Z)
    public float pillarSpacing = 2f; // 기둥 간격
    public float minHeight = 1f;     // 기둥 최소 높이
    public float maxHeight = 5f;     // 기둥 최대 높이
    public float minScale = 3f;      // 기둥의 최소 너비/깊이
    public float maxScale = 5f;      // 기둥의 최대 너비/깊이
    public float animationSpeed = 2f; // 올라갔다 내려가는 속도
    public int maxPillars = 10;      // 한 번에 생성 가능한 기둥의 최대 개수

    private Queue<GameObject> activePillars = new Queue<GameObject>(); // 현재 활성화된 기둥들

    void Start()
    {
        StartCoroutine(GeneratePillars());
    }

    System.Collections.IEnumerator GeneratePillars()
    {
        while (true)
        {
            if (activePillars.Count < maxPillars)
            {
                SpawnPillar();
            }
            else
            {
                RemoveOldestPillar();
            }

            yield return new WaitForSeconds(0.5f); // 기둥 생성 간격
        }
    }

    void SpawnPillar()
    {
        // 무작위 위치와 높이 설정
        float randomX = Random.Range(-floorSize.x / 2, floorSize.x / 2);
        float randomZ = Random.Range(-floorSize.y / 2, floorSize.y / 2);
        float randomHeight = Random.Range(minHeight, maxHeight);

        // 기둥의 정사각형 크기 설정
        float randomScale = Random.Range(minScale, maxScale);

        // 기둥 생성
        GameObject pillar = Instantiate(pillarPrefab, new Vector3(randomX, randomHeight / 2, randomZ), Quaternion.identity);
        pillar.transform.localScale = new Vector3(randomScale, randomHeight, randomScale);

        // 애니메이션 추가
        StartCoroutine(AnimatePillar(pillar));

        // 기둥을 큐에 추가
        activePillars.Enqueue(pillar);
    }

    void RemoveOldestPillar()
    {
        if (activePillars.Count > 0)
        {
            GameObject oldestPillar = activePillars.Dequeue();
            Destroy(oldestPillar); // 오래된 기둥 제거
        }
    }

    // 기둥 애니메이션
    System.Collections.IEnumerator AnimatePillar(GameObject pillar)
    {
        Vector3 originalPosition = pillar.transform.position;
        float elapsedTime = 0f;

        while (pillar != null) // 기둥이 존재하는 동안 애니메이션 반복
        {
            elapsedTime += Time.deltaTime;
            float offsetY = Mathf.Sin(elapsedTime * animationSpeed) * 0.5f; // Y축 이동량 계산
            pillar.transform.position = new Vector3(
                originalPosition.x,
                originalPosition.y + offsetY,
                originalPosition.z
            );

            yield return null;
        }
    }
}
