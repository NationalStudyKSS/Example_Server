using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Example_LINQ : MonoBehaviour
{
    [SerializeField] GameObject _enemyPrefab;

    private void Start()
    {
        CreateEnemies();

        // ��� ���� ������Ʈ �߿��� 'Enemy' �±׸� ���� ������Ʈ�� ã���ϴ�.
        // FindGameObjectsWithTag("Enemy")�� ������ ���
        GameObject[] allGameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        IEnumerable<GameObject> enemies = allGameObjects.Where(go => go.CompareTag("Enemy"));

        Debug.Log($"�� {enemies.Count()}���� ���� ã�ҽ��ϴ�.");

        // ü���� 100 �̸��� ������ ���͸��մϴ�.
        var weakEnemies = from enemy in enemies
                          let healthComponent = enemy.GetComponent<Health>()
                          where healthComponent != null && healthComponent.CurrentHealth < 100
                          select enemy;

        Debug.Log($"ü���� 100 �̸��� ��: {weakEnemies.Count()}��");

        // ��� ���� ü�� ������ �����մϴ�.
        var sortedEnemies = enemies
            .Select(enemy => new { Enemy = enemy, Health = enemy.GetComponent<Health>() })
            .Where(x => x.Health != null)
            .OrderBy(x => x.Health.CurrentHealth);

        Debug.Log("ü�� �� ����:");
        foreach (var item in sortedEnemies)
        {
            Debug.Log($"{item.Enemy.name} - ü��: {item.Health.CurrentHealth}");
        }

        // ���� ���� �� �ϳ��� ã���ϴ�.
        GameObject weakestEnemy = sortedEnemies.FirstOrDefault()?.Enemy;
        if (weakestEnemy != null)
        {
            Debug.Log($"���� ���� ��: {weakestEnemy.name} (ü��: {weakestEnemy.GetComponent<Health>().CurrentHealth})");
        }
        else
        {
            Debug.Log("���� ���� ���� ã�� �� �����ϴ�.");
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
