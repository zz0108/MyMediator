# MyMediator 練習專案

這是一個模仿 MediatR 功能的練習專案，主要用於學習中介者模式（Mediator Pattern）的實作方式。

## 專案結構

- MyMediator.Abstractions - 介面定義
- MyMediator.Core - 核心實作
- MyMediator.DependencyInjection - DI 容器整合
- MyMediator.Tests - 單元測試

## 功能

- 基本的 Request/Response 模式
- 依賴注入整合
- 自動註冊 Handlers

## 使用方式

1. 註冊服務：

```csharp
services.AddMediator(Assembly.GetExecutingAssembly());
```

2. 定義 Request 和 Response：

```csharp
public class TestRequest : IRequest<TestResponse> { }
public class TestResponse { }
```

3. 實作 Handler：

```csharp
public class TestHandler : IRequestHandler<TestRequest, TestResponse>
{
    public Task<TestResponse> Handle(TestRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TestResponse());
    }
}
```

## 學習目標

- 了解中介者模式的實作方式
- 練習依賴注入的使用
- 熟悉 .NET 專案架構