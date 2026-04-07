using Compiler.BddTests.Support;
using TechTalk.SpecFlow;

namespace Compiler.BddTests.Hooks;

[Binding]
public class TestHooks
{
    private readonly CompilerTestContext _context;

    public TestHooks(CompilerTestContext context)
    {
        _context = context;
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        _context.Reset();
    }

    [AfterScenario]
    public void AfterScenario()
    {
    }
}