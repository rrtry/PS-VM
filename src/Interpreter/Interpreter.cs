namespace Interpreter;

using Execution;
using Parser;
using Semantics;

public class Interpreter
{
    private readonly Context context;
    private readonly IEnvironment environment;
    private readonly Builtins builtins;

    public Interpreter()
    {
        environment = new ConsoleEnvironment();
        context = new Context(environment);
        builtins = new Builtins(environment);
    }

    public Interpreter(Context cxt, IEnvironment env)
    {
        context = cxt;
        environment = env;
        builtins = new Builtins(environment);
    }

    /// <summary>
    /// Выполнение программы.
    /// </summary>
    /// <param name="sourceCode">Исходный код программы.</param>
    public void Execute(string sourceCode)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            throw new ArgumentException("Source code cannot be null or empty", nameof(sourceCode));
        }

        Parser parser = new(context, environment, sourceCode);
        BlockStatement program = parser.Parse();

        SemanticsChecker semanticsChecker = new(builtins.Functions, builtins.Types);
        semanticsChecker.Check(program);

        AstEvaluator evaluator = new AstEvaluator(context);
        evaluator.Evaluate(program);
    }
}