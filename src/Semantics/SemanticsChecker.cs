namespace Semantics;

using Ast;
using Ast.Declarations;
using Semantics.Passes;
using Semantics.Symbols;

/// <summary>
/// Класс для проверки семантики программы.
/// Реализован как фасад над несколькими проходами (passes), каждый из которых реализует шаблон «Посетитель» (Visitor).
/// </summary>
public class SemanticsChecker
{
    private readonly AbstractPass[] passes;

    public SemanticsChecker(
        IReadOnlyList<NativeFunction> builtinFunctions,
        IReadOnlyList<BuiltinType> builtinTypes
    )
    {
        SymbolsTable globalSymbols = new();
        foreach (NativeFunction function in builtinFunctions)
        {
            globalSymbols.DeclareFunction(function);
        }

        foreach (BuiltinType type in builtinTypes)
        {
            globalSymbols.DeclareType(type);
        }

        passes =
        [
            new ResolveNamesPass(globalSymbols),
            new CheckContextSensitiveRulesPass(),
            new ResolveTypesPass(),
            new CheckTypesPass(),
        ];
    }

    public void Check(EntryPointNode program)
    {
        foreach (AbstractPass pass in passes)
        {
            program.Accept(pass);
        }
    }
}