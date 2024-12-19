public abstract class Token
{
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