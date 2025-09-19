using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Example_LINQ : MonoBehaviour
{
    [SerializeField] GameObject _enemyPrefab;

    private void Start()
    {
        CreateEnemies();

        // 모든 게임 오브젝트 중에서 'Enemy' 태그를 가진 오브젝트를 찾습니다.
        // FindGameObjectsWithTag("Enemy")와 유사한 기능
        GameObject[] allGameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        IEnumerable<GameObject> enemies = allGameObjects.Where(go => go.CompareTag("Enemy"));

        Debug.Log($"총 {enemies.Count()}명의 적을 찾았습니다.");

        // 체력이 100 미만인 적들을 필터링합니다.
        var weakEnemies = from enemy in enemies
                          let healthComponent = enemy.GetComponent<Health>()
                          where healthComponent != null && healthComponent.CurrentHealth < 100
                          select enemy;

        Debug.Log($"체력이 100 미만인 적: {weakEnemies.Count()}명");

        // 모든 적을 체력 순으로 정렬합니다.
        var sortedEnemies = enemies
            .Select(enemy => new { Enemy = enemy, Health = enemy.GetComponent<Health>() })
            .Where(x => x.Health != null)
            .OrderBy(x => x.Health.CurrentHealth);

        Debug.Log("체력 순 정렬:");
        foreach (var item in sortedEnemies)
        {
            Debug.Log($"{item.Enemy.name} - 체력: {item.Health.CurrentHealth}");
        }

        // 가장 약한 적 하나를 찾습니다.
        GameObject weakestEnemy = sortedEnemies.FirstOrDefault()?.Enemy;
        if (weakestEnemy != null)
        {
            Debug.Log($"가장 약한 적: {weakestEnemy.name} (체력: {weakestEnemy.GetComponent<Health>().CurrentHealth})");
        }
        else
        {
            Debug.Log("가장 약한 적을 찾을 수 없습니다.");
        }
    }

    void CreateEnemies()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject enemy = Instantiate(_enemyPrefab);
            enemy.tag = "Enemy";
            enemy.name = "Enemy_" + i;
            enemy.GetComponent<Health>().CurrentHealth = Random.Range(50, 150);
            Debug.Log($"{enemy.name}, HP : {enemy.GetComponent<Health>().CurrentHealth}");
        }
    }
}
