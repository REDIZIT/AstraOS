using System.Net.NetworkInformation;

public abstract class Token
{
    protected CompilationContext ctx;

    public string Generate(CompilationContext ctx)
    {
        this.ctx = ctx;
        return Generate();
    }
	public abstract string Generate();
}


public class Token_FunctionDeclaration : Token
{
    public string functionName;

    public override string Generate()
    {
        return $"{functionName}:\n";
    }
}
public class Token_FunctionCall : Token
{
    public string functionName;

    public override string Generate()
    {
        return $"call {functionName}\n";
    }
}
public class Token_BlockBegin : Token
{
    public override string Generate()
    {
        return "";
    }
}
public class Token_BlockEnd : Token
{
    public override string Generate()
    {
        return "";
    }
}
public class Token_Return : Token
{
    public override string Generate()
    {
        return "ret\n";
    }
}
public class Token_Print : Token
{
    public string message;

    public override string Generate()
    {
        return @"mov rax, 1
mov rdi, 1
mov rsi, msg
mov rdx, 13
syscall
";
    }
}
public class Token_Exit : Token
{
    public string message;

    public override string Generate()
    {
        return @"mov rax, 60
mov rdi, 0
syscall
";
    }
}
public class Token_Comment : Token
{
    public string text;
    public bool isClosing;

    public override string Generate()
    {
        return $"; {text}\n";
    }
}
public class Token_If : Token
{
    public Token expression;

    public override string Generate()
    {
        return $"";
    }
}

public class Token_VariableDeclaration : Token
{
    public string type;
    public string name;
    public int localOffset;

    public Token_VariableDeclaration(string type, string name, CompilationContext ctx)
    {
        this.type = type;
        this.name = name;
        localOffset = ctx.AllocVariable(name);
    }

    public override string Generate()
    {
        string rspIndex = $"[rsp" + (localOffset < 0 ? localOffset : " + " + localOffset) + "]";
        return $"sub rsp, 4\nmov qword {rspIndex}, 0\n";
    }
}
public class Token_VariableAssign : Token
{
    public string variableName;
    public string value;

    public override string Generate()
    {
        int localOffset = ctx.offsetByVariableName[variableName];
        string rspIndex = $"[rsp" + (localOffset < 0 ? localOffset : " + " + localOffset) + "]";
        return $"mov qword {rspIndex}, {value}\n";
    }
}