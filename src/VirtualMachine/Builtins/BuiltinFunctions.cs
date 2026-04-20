using System.Globalization;

using Runtime;

namespace VirtualMachine.Builtins;

public class BuiltinFunctions
{
    private readonly IEnvironment environment;

    public BuiltinFunctions(IEnvironment environment)
    {
        this.environment = environment;
    }

    public Value Itos(Value value)
    {
        return new Value(value.AsLong().ToString(CultureInfo.InvariantCulture));
    }

    public Value Ftos(Value value, Value precision)
    {
        return new Value(value.AsDouble().ToString($"F{precision.AsLong()}", CultureInfo.InvariantCulture));
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
        return new Value(str.Substring(
            (int)from.AsLong(), (int)to.AsLong()
        ));
    }

    public Value Stoi(Value value)
    {
        string s = value.AsString();
        long l = long.Parse(s, CultureInfo.InvariantCulture);
        return new Value(l);
    }

    public Value Stof(Value value)
    {
        string s = value.AsString();
        double d = double.Parse(s, CultureInfo.InvariantCulture);
        return new Value(d);
    }

    public Value Input()
    {
        string s = environment.Input();
        return new Value(s);
    }

    public Value Printf(Value value, Value precision)
    {
        environment.Print(value.AsDouble().ToString($"F{precision.AsLong()}", CultureInfo.InvariantCulture));
        return Value.Unit;
    }

    public Value Print(Value value)
    {
        environment.Print(value.AsString());
        return Value.Unit;
    }

    public Value Printi(Value value)
    {
        environment.Print(value.AsLong().ToString(CultureInfo.InvariantCulture));
        return Value.Unit;
    }

    public Value Printb(Value value)
    {
        environment.Print(value.AsBool().ToString(CultureInfo.InvariantCulture));
        return Value.Unit;
    }
}