namespace Ast.Declarations;

/// <summary>
/// Абстрактный класс с информацией о формальном параметре функции — как встроенной, так и пользовательской.
/// </summary>
public abstract class AbstractParameterDeclaration : Declaration // Со второй итерации будет наследовать VariableDeclaration
{
    protected AbstractParameterDeclaration()
    {
    }
}