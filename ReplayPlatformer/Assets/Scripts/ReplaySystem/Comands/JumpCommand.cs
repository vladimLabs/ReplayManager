using UnityEngine;
using ReplaySystem.Core;

public class JumpCommand : ICommand
{
    public int Tick { get; private set; }
    public CommandType Type => CommandType.Jump;
    
    [System.Serializable]
    public class JumpPayload
    {
        public bool IsJumping;
    }
    
    private JumpPayload _payload;
    
    public JumpCommand(int tick, JumpPayload payload)
    {
        Tick = tick;
        _payload = payload;
    }
    
    public object GetPayload() => _payload;
    
    public void Execute(object target)
    {
        var controller = (target as GameObject)?.GetComponent<PlayerController>();
        if (controller != null && _payload.IsJumping)
            controller.Jump();
    }
}