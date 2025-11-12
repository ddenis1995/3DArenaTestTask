using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _playerSpeed = 5.0f;
    [SerializeField] private float _jumpHeight = 1.5f;
    [SerializeField] private float _gravityValue = -9.81f;

    private CharacterController _controller;
    private Vector3 _playerVelocity;
    private bool _groundedPlayer;

    [Header("Input Actions")] 
    public InputActionReference MoveAction; // expects Vector2
    public InputActionReference JumpAction; // expects Button

    private void Awake()
    {
        _controller = gameObject.GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        MoveAction.action.Enable();
        JumpAction.action.Enable();
    }

    private void OnDisable()
    {
        MoveAction.action.Disable();
        JumpAction.action.Disable();
    }

    void Update()
    {
        _groundedPlayer = _controller.isGrounded;
        if (_groundedPlayer && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }

        var move = ReadInput();
         
        if (move != Vector3.zero)
        {
            transform.forward = move;
        }

        Jump();

        ApplyGravity();

        CombineMovement(move);
    }

    private void CombineMovement(Vector3 move)
    {
        Vector3 finalMove = (move * _playerSpeed) + (_playerVelocity.y * Vector3.up);
        _controller.Move(finalMove * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        _playerVelocity.y += _gravityValue * Time.deltaTime;
    }

    private void Jump()
    {
        if (JumpAction.action.triggered && _groundedPlayer)
        {
            _playerVelocity.y = Mathf.Sqrt(_jumpHeight * -2.0f * _gravityValue);
        }
    }

    private Vector3 ReadInput()
    {
        Vector2 input = MoveAction.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = Vector3.ClampMagnitude(move, 1f);
       
        return move;
    }
}