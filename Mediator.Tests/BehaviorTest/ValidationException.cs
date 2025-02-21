namespace Mediator.Tests.BehaviorTest;

/// <summary>
/// 用來測試驗證行為的例外狀況
/// </summary>
public class ValidationException(string message) : Exception(message);