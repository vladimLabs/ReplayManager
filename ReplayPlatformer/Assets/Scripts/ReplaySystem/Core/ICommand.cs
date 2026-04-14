using System;

namespace ReplaySystem.Core
{
    public interface ICommand
    {
        int Tick { get; }
        CommandType Type { get; }
        object GetPayload();
        void Execute(object target);
    }
}