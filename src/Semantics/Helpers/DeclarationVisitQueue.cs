using Ast;
using Ast.Declarations;

namespace Semantics.Helpers;

/// <summary>
/// Выполняет отложенный обход узлов объявлений для реализации взаимной рекурсии объявлений.
/// </summary>
/// <remarks>
/// Для поддержки взаимной рекурсии функций мы выполняем обход дочерних узлов необычным способом:
/// 1. Для подряд идущих объявлений функций лбо типов мы объявляем все их заранее (до посещения дочерних узлов)
/// 2. Как только подряд идущие функции либо типы заканчиваются — запускаем обход дочерних узлов.
/// </remarks>
public class DeclarationVisitQueue
{
    private readonly IAstVisitor visitor;
    private readonly Queue<Declaration> visitQueue;
    private VisitQueueType visitQueueType;

    public DeclarationVisitQueue(IAstVisitor visitor)
    {
        this.visitor = visitor;
        visitQueue = [];
        visitQueueType = VisitQueueType.None;
    }

    private enum VisitQueueType
    {
        None,
        Type,
        Function,
    }

    public void BeforeTypeDeclaration()
    {
        UpdateVisitQueueType(VisitQueueType.Type);
    }

    public void BeforeFunctionDeclaration()
    {
        UpdateVisitQueueType(VisitQueueType.Function);
    }

    public void Flush()
    {
        UpdateVisitQueueType(VisitQueueType.None);
    }

    public void Enqueue(Declaration declaration)
    {
        if (visitQueueType != VisitQueueType.None)
        {
            visitQueue.Enqueue(declaration);
        }
        else
        {
            declaration.Accept(visitor);
        }
    }

    private void UpdateVisitQueueType(VisitQueueType type)
    {
        if (visitQueueType != type)
        {
            ProcessVisitQueue();
            visitQueueType = type;
        }
    }

    private void ProcessVisitQueue()
    {
        while (visitQueue.TryDequeue(out Declaration? declaration))
        {
            declaration.Accept(visitor);
        }
    }
}