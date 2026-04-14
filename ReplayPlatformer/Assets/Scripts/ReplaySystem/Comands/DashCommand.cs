using UnityEngine;
using ReplaySystem.Core;

public class DashCommand : ICommand
{
    public int Tick { get; private set; }
    public CommandType Type => CommandType.Dash;
    
    [System.Serializable]
    public class DashPayload
    {
        public Vector2 Direction;
    }
    
    private DashPayload _payload;
    
    public DashCommand(int tick, DashPayload payload)
    {
        Tick = tick;
        _payload = payload;
    }
    
    public object GetPayload() => _payload;
    
    public void Execute(object target)
    {
        var controller = (target as GameObject)?.GetComponent<PlayerController>();
        if (controller != null)
            controller.Dash(_payload.Direction);
    }
}