using Runtime;
using VirtualMachine.Exceptions;

namespace VirtualMachine.Builtins;

public class BuiltinFunctions
{
    private IEnvironment environment;

    public BuiltinFunctions(IEnvironment environment)
    {
        this.environment = environment;
    }

    public Value Itos(Value value)
    {
        return new Value(value.AsLong().ToString());
    }

    public Value Ftos(Value value, Value precision)
    {
        return new Value(value.AsDouble().ToString($"F{precision.AsLong()}"));
    }

    public Value Ftoi(Value value)
    {
        double d = value.AsDouble();
        return new Value((long)d);
    }

    public Value Itof(Value value)
    {
        long l = value.AsLong();
        return new Value((double)l);
    }

    public Value Sconcat(Value s1, Value s2)
    {
        string str1 = s1.AsString();
        string str2 = s2.AsString();
        return new Value(str1 + str2);
    }

    public Value Strlen(Value value)
    {
        string s = value.AsString();
        return new Value(s.Length);
    }

    public Value Substr(Value s, Value from, Value to)
    {
        string str = s.AsString();
        return new Value(str.Substring((int)from.AsLong(), (int)to.AsLong()));
    }

    public Value Stoi(Value value)
    {
        string s = value.AsString();
        long l = long.Parse(s);
        return new Value(l);
    }

    public Value Stof(Value value)
    {
        string s = value.AsString();
        double d = double.Parse(s);
        return new Value(d);
    }

    public Value Input()
    {
        string s = environment.Input();
        return new Value(s);
    }

    public Value Printf(Value value, Value precision)
    {
        environment.Print(value.AsDouble().ToString($"F{precision.AsLong()}"));
        return Value.Unit;
    }

    public Value Print(Value value)
    {
        environment.Print(value.AsString());
        return Value.Unit;
    }

    public Value Printi(Value value)
    {
        environment.Print(value.AsLong().ToString());
        return Value.Unit;
    }

    public Value Abs(Value value)
    {
        long l = Math.Abs(value.AsLong());
        return new Value(l);
    }

    public Value Sqrt(Value value)
    {
        double r = Math.Sqrt(value.AsDouble());
        return new Value(r);
    }

    public Value Pow(Value x, Value y)
    {
        long l = (long)Math.Pow(x.AsLong(), y.AsLong());
        return new Value(l);
    }

    public Value Min(Value x, Value y)
    {
        long l = Math.Min(x.AsLong(), y.AsLong());
        return new Value(l);
    }

    public Value Max(Value x, Value y)
    {
        long l = Math.Max(x.AsLong(), y.AsLong());
        return new Value(l);
    }
}