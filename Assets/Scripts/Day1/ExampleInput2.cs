using UnityEngine;

public class ExampleInput2 : MonoBehaviour
{
    private BasicInput _inputs;

    [SerializeField] private float movementSpeed = 5f;
    private Vector2 _inputsMove;
    private bool _isGrounded;

    private void Awake()
    {
        _inputs = new BasicInput();
    }

    private void OnEnable()
    {
        _inputs.Enable();
    }

    private void OnDisable()
    {
        _inputs.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputs.Moverment.Jump.performed += ctx => Jump();
        _inputs.Moverment.Attack.performed += ctx => Attack();
        _inputs.Moverment.Rotation.performed += ctx => Rotation();

    }

    // Update is called once per frame
    void Update()
    {
        _inputsMove = _inputs.Moverment.Move.ReadValue<Vector2>();

        // Debug.Log($"move {_inputsMove}");
        float scale = Time.deltaTime * movementSpeed;
        transform.Translate(_inputsMove.x * scale, 0, _inputsMove.y * scale);

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.75f, 1 <<LayerMask.NameToLayer("Default")))
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }

    public void Jump()
    {
        if (_isGrounded)
        {
        transform.position += new Vector3(0, 2, 0);   
        }
        
    }

    public void Attack()
    {
        Debug.Log("Attack");
    }

    public void Rotation()
    {
        transform.Rotate(0, 90, 0);
    }
}
