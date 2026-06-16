using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatsSO", menuName = "Scriptable Objects/UnitStatsSO")]
public class UnitStatsSO : ScriptableObject
{
    [SerializeField] private string _unitName;
    public string unitName => _unitName;

    [SerializeField] private float _maxHealth;
    public float maxHealth => _maxHealth;

    [SerializeField] private float _moveSpeed;
    public float moveSpeed => _moveSpeed;

    [SerializeField] private float _attackDamage;
    public float attackDamage => _attackDamage;


    public GameObject modelPrefab;
}
