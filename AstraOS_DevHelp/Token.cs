using System.Text;

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
        return $"{functionName}:\nmov rbp, rsp\n";
    }
}
public class Token_FunctionCall : Token
{
    public string functionName;

    public override string Generate()
    {
        return $"push rbp\ncall {functionName}\npop rbp\n";
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
        return "\nmov rsp, rbp\nret\n";
    }
}
public class Token_Print : Token
{
    public string pointerToString;

    public override string Generate()
    {
        return @$"mov rax, 1
mov rdi, 1
lea rsi, {pointerToString}
mov rdx, 13
syscall
";
    }
}
public class Token_WriteConsole : Token
{
    public string pointerToString;

    public override string Generate()
    {
        return @$"lea rbx, {pointerToString}
push rbx
call console_writeline
add rsp, 4
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
    public string defaultValue;
    public int localOffset;

    public Token_VariableDeclaration(string type, string name, string defaultValue, CompilationContext ctx)
    {
        this.type = type;
        this.name = name;
        this.defaultValue = defaultValue;

        int sizeInBytes;

        if (type == "string")
        {
            sizeInBytes = defaultValue.Length + 1;
        }
        else if (type == "int")
        {
            sizeInBytes = 4;
        }
        else
        {
            throw new Exception($"Unknown variable type '{type}'");
        }

        localOffset = ctx.AllocVariable(name, sizeInBytes);
    }

    public override string Generate()
    {
        string rspIndex = ctx.GetRSPIndex(name);

        if (type == "string")
        {
            int sizeInBytes = ctx.GetSizeInBytes(name);
            int divider = 1;
            int qwordsCount = (int)Math.Floor((sizeInBytes - 1) / (float)divider);
            int offset = ctx.GetOffset(name);

            StringBuilder b = new();

            b.AppendLine($"\n; string {name} = '{defaultValue}'");
            b.AppendLine("mov rbx, rbp");
            b.AppendLine($"sub rbx, {-offset}");

            for (int i = 0; i < qwordsCount; i++)
            {
                int startIndex = i * divider;
                int endIndex = Math.Clamp(startIndex + divider, 0, defaultValue.Length);
                string substring = defaultValue.Substring(i * divider, endIndex - startIndex);
                b.AppendLine($"mov byte [rbx+{i * divider}], \"{substring}\"");
            }

            b.AppendLine($"mov byte [rbx+{sizeInBytes - 1}], 0");
            b.AppendLine($"sub rsp, {sizeInBytes}");
            b.AppendLine();

            return b.ToString();
        }
        else
        {
            int sizeInBytes = 4;
            if (MathExpressions.IsExpression(defaultValue))
            {
                string asm = MathExpressions.Generate(defaultValue, ctx);
                return $"{asm}\nsub rsp, {sizeInBytes}\nmov qword {rspIndex}, rax\n";
            }
            else
            {
                return $"sub rsp, {sizeInBytes}\nmov qword {rspIndex}, {defaultValue}\n";
            }
        }        
    }
}
public class Token_VariableAssign : Token
{
    public string variableName;
    public string value;

    public override string Generate()
    {
        string rspIndex = ctx.GetRSPIndex(variableName);
        return $"mov qword {rspIndex}, {value}\n";
    }
}

public class Token_MathExpression : Token
{
    public string expression;
    public string variableToAssign;

    public override string Generate()
    {
        string asm = MathExpressions.Generate(expression, ctx);

        if (variableToAssign == null)
        {
            // Put (don't touch) result of calculation into rax
            return $"{asm}\n";
        }
        else
        {
            // Put result of calculation into variable
            return $"{asm}mov {ctx.GetRSPIndex(variableToAssign)}, rbx\n; MathExpressions.Generate\n";
        }
    }
}

public class Token_While : Token
{
    public string expression;

    public override string Generate()
    {
        string expressionAsm = MathExpressions.Generate(expression, ctx);

        return @$".while_check:
{expressionAsm}
cmp rbx, 0
jle .while_exit
%while_body%
jmp .while_check
.while_exit:
";
    }
}