using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerController _controller;
    
    void Start()
    {
        _controller = GetComponent<PlayerController>();
    }
    
    void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _controller.RecordMoveInput(moveInput);
        
        if (Input.GetButtonDown("Jump"))
        {
            _controller.RecordJump();
        }
        
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            Vector2 dashDir = GetDashDirection();
            _controller.RecordDash(dashDir);
        }
    }
    
    private Vector2 GetDashDirection()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
        
        if (direction.magnitude < 0.1f)
        {
            direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            if (direction.magnitude < 0.1f) direction = Vector2.right;
        }
        
        return direction;
    }
}