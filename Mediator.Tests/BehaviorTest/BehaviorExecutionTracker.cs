namespace Mediator.Tests.BehaviorTest;

public static class BehaviorExecutionTracker
{
    public static List<string> ExecutionOrder { get; } = new();
    
    public static void Reset()
    {
        ExecutionOrder.Clear();
    }

}