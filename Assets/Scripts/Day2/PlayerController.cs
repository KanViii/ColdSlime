using UnityEngine;

public class PlayerController : BaseUnitController
{
    public override void CustomUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized; //chuan hoa vector ve |1|
        Move(direction);

        if (Input.GetKeyDown(KeyCode.J))
        {
            BaseUnitController enemy = GameManager.Instance.FindEnemy();
            if (enemy != null)
            {
                Attack(enemy);
            }
        }
    }
}