using System.Runtime.CompilerServices;
using System.Text;

public abstract class Token
{
    public CompilationContext ctx;

    public string Generate(CompilationContext ctx)
    {
        this.ctx = ctx;
        return Generate();
    }
    public virtual void Simplify(List<Token> tokens, int thisTokenIndex)
    {
    }
    public virtual string Generate()
    {
        throw new Exception($"Token '{this}' is not generetable");
    }
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
public class Token_WriteConsole_Buffer : Token
{
    public string pointerToString;

    public override string Generate()
    {
        return @$"
push buffer
call console_writeline
add rsp, 4
";
    }
}


public class Token_IntToString : Token
{
    public string valueOrRsp_int;
    public string rsp_outString;

    public override string Generate()
    {
        string value = Token_VariableAssign.Convert_ValueOrRSP_to_RSP(valueOrRsp_int, ctx);
        return @$"
; IntToString {valueOrRsp_int}, {rsp_outString}
mov qword rbx, {value}
push rbx
push {rsp_outString}
call int_to_string
add rsp, 8
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
        if (name.Contains(' ')) throw new Exception("Invalid variable name.");

        this.type = type;
        this.name = name;
        this.defaultValue = defaultValue;
        this.ctx = ctx;

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
                // < expression code, where result inside rax >
                // sub rsp sizeInBytes
                // mov rspIndex, rax

                string asm = MathExpressions.Generate(defaultValue, ctx);
                return $"{asm}\nsub rsp, {sizeInBytes}\nmov qword {rspIndex}, rax\n";
            }
            else
            {
                string asmValue = ctx.offsetByVariableName.ContainsKey(defaultValue) ? ctx.GetRSPIndex(defaultValue) : defaultValue;
                //return $"sub rsp, {sizeInBytes}\nmov qword {rspIndex}, {asmValue}\n";

                return $"sub rsp, {sizeInBytes}\n" + Token_VariableAssign.Generate_ValueOrRSP_to_RSP(asmValue, rspIndex);
            }
        }        
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
    public Token whileBodyEnd;

    public override string Generate()
    {
        string expressionAsm = MathExpressions.Generate(expression, ctx);

        string whileBodyEndAsm = whileBodyEnd != null ? "\n" + whileBodyEnd.Generate(ctx) : "";

        return @$".while_check:
{expressionAsm}
cmp rbx, 0
jle .while_exit
%while_body%{whileBodyEndAsm}
jmp .while_check
.while_exit:
";
    }
}

public class Token_For : Token
{
    public string iteratorName;
    public string range_a, range_b;
    public bool include_a, include_b;
    public bool isReversed;

    public override void Simplify(List<Token> tokens, int thisTokenIndex)
    {
        List<Token> expandedTokens = new();

        Token token_i = new Token_VariableDeclaration("int", iteratorName, isReversed ? range_b /*+ (include_b ? " + 1" : "")*/: range_a, ctx);
        expandedTokens.Add(token_i);

        if (isReversed && include_b == false)
        {
            Token_IncDec token_range_start_dec = new Token_IncDec()
            {
                variableName = iteratorName,
                isDec = true
            };
            expandedTokens.Add(token_range_start_dec);
        }
        else if (isReversed == false && include_a == false)
        {
            Token_IncDec token_range_start_inc = new Token_IncDec()
            {
                variableName = iteratorName,
                isDec = false
            };
            expandedTokens.Add(token_range_start_inc);
        }

        Token token_incdec = new Token_IncDec()
        {
            variableName = iteratorName,
            isDec = isReversed
        };

        Token token_while = new Token_While()
        {
            ctx = ctx,
            whileBodyEnd = token_incdec,

            expression = 
                isReversed ? 
                iteratorName + (include_a ? " >= " : " > ") + range_a :
                iteratorName + (include_b ? " <= " : " < ") + range_b
        };
        expandedTokens.Add(token_while);



        tokens.RemoveAt(thisTokenIndex);
        tokens.InsertRange(thisTokenIndex, expandedTokens);
    }
}
public class Token_IncDec : Token
{
    public string variableName;
    public bool isDec;

    public override string Generate()
    {
        string rspIndex = ctx.GetRSPIndex(variableName);
        if (isDec) return $"dec qword {rspIndex}\n";
        else return $"inc qword {rspIndex}\n";
    }
}