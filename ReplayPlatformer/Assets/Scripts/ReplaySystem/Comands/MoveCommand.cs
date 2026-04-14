using UnityEngine;
using ReplaySystem.Core;

[System.Serializable]
public class MoveCommand : ICommand
{
    public int Tick { get; private set; }
    public CommandType Type => CommandType.Move;
    
    [System.Serializable]
    public class MovePayload
    {
        public float Horizontal;
        public float Vertical;
    }
    
    private MovePayload _payload;
    
    public MoveCommand(int tick, MovePayload payload)
    {
        Tick = tick;
        _payload = payload;
    }
    
    public object GetPayload() => _payload;
    
    public void Execute(object target)
    {
        var controller = (target as GameObject)?.GetComponent<PlayerController>();
        if (controller != null)
            controller.SetMoveInput(new Vector2(_payload.Horizontal, _payload.Vertical));
    }
}
