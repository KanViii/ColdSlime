using UnityEngine;

public class BaseUnitController : MonoBehaviour
{
    private BaseUnitConfig _config;
    public BaseUnitConfig Config => _config;

    private int _currentHealth;
    private int _currentDamage;
    private int _currentSpeed;

    Color baseColor;

    public virtual void CustomUpdate() { }

    public void Initialize(BaseUnitConfig config)
    {
        _config = config;

        _currentDamage = _config.BaseDamage;
        _currentHealth = _config.BaseHealth;
        _currentSpeed = _config.MoveSpeed;
        
        baseColor = transform.GetChild(0).GetComponent<Renderer>().material.color;
    }

    public void Move(Vector3 direction)
    {
        _config.Move(transform, _currentSpeed, direction);
    }

    public void Attack(BaseUnitController target)
    {
        if (target == null) return;

        target.TakeDamage(_currentDamage);
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        transform.GetChild(0).GetComponent<Renderer>().material.color = Color.Lerp(Color.white, baseColor, _currentHealth / _config.BaseHealth);

        if (_currentHealth <= 0) Die();
    }

    public void Die()
    {
        UnitPool.Instance.ReleaseUnit(this);
    }

}
