using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int _currentHealth;

    public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
}
