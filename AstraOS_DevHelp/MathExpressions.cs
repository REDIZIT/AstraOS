using System.Text;
using System.Text.RegularExpressions;

public class MathExpressions
{
    public static Dictionary<string, int> precedence = new()
    {
        { "*", 4 },
        { "/", 4 },
        { "+", 3 },
        { "-", 3 },
        { ">", 2 },
        { "<", 2 },
        { ">=", 2 },
        { "<=", 2 },
        { "not", 1 },
        { "and", 0 },
        { "or", 0 },
    };

    public static bool IsExpression(string str)
    {
        return precedence.Any(kv => str.Contains(kv.Key));
    }

    public static string Generate(string expression, CompilationContext ctx)
    {
        List<string> tokens = Tokenize(expression);
        List<string> tokens_rpn = ToRPN(tokens);

        string asm = "\n; " + expression + "\n" + GenerateFromRPN(tokens_rpn, ctx);
        return asm;
    }

    public static List<string> Tokenize(string expression)
    {
        var regex = new Regex(@"\d+|[a-zA-Z]|>=|<=|[+\-*\/()><]");
        var matches = regex.Matches(expression);

        var tokens = new List<string>();
        foreach (Match match in matches)
        {
            tokens.Add(match.Value);
        }

        return tokens;
    }

    public static List<string> ToRPN(List<string> tokens)
    {
        var output = new List<string>();
        var operators = new Stack<string>();

        foreach (var token in tokens)
        {
            // If constant or variable
            if (precedence.ContainsKey(token) == false)
            {
                output.Add(token);
            }
            else if (token == "(")
            {
                operators.Push(token);
            }
            else if (token == ")")
            {
                while (operators.Peek() != "(")
                    output.Add(operators.Pop());
                operators.Pop();
            }
            else
            {
                while (operators.Count > 0 && precedence.ContainsKey(operators.Peek()) &&
                       precedence[operators.Peek()] >= precedence[token])
                {
                    output.Add(operators.Pop());
                }
                operators.Push(token);
            }
        }

        while (operators.Count > 0)
            output.Add(operators.Pop());

        return output;
    }
    public static string GenerateFromRPN(List<string> rpn, CompilationContext ctx)
    {
        StringBuilder b = new();

        foreach (string token in rpn)
        {
            // If constant or variable
            if(token == "(" || token == ")")
            {

            }
            else if (precedence.ContainsKey(token) == false)
            {
                string value;

                if (int.TryParse(token, out int constant))
                {
                    value = constant.ToString();
                }
                else
                {
                    value = ctx.GetRSPIndex(token);
                }

                // Push to stack
                b.AppendLine("push qword " + value);
            }
            else
            {
                // Pop in reverse order
                b.AppendLine("pop rcx");
                if (token != "not") b.AppendLine("pop rbx");


                if (token == "+") b.AppendLine("add rbx, rcx");
                else if (token == "-") b.AppendLine("sub rbx, rcx");
                else if (token == "*") b.AppendLine("imul rbx, rcx");
                else if (token == "/") b.AppendLine("idiv rbx, rcx");
                else if (token == "and") b.AppendLine("and rbx, rcx");
                else if (token == "or") b.AppendLine("or rbx, rcx");
                else if (token == "not") b.AppendLine("test rbx, rbx\nsete al");
                else if (token == ">") b.AppendLine("cmp rbx, rcx\nsetg dl\nmovzx rdx, dl\nmov rbx, rdx");
                else if (token == "<") b.AppendLine("cmp rbx, rcx\nsetl dl\nmovzx rdx, dl\nmov rbx, rdx");
                else if (token == ">=") b.AppendLine("cmp rbx, rcx\nsetge dl\nmovzx rdx, dl\nmov rbx, rdx");
                else if (token == "<=") b.AppendLine("cmp rbx, rcx\nsetle dl\nmovzx rdx, dl\nmov rbx, rdx");
                else throw new Exception($"Unknown operator token '{token}'");

                b.AppendLine("push qword rbx");
            }
        }

        b.AppendLine("pop rbx");

        return b.ToString();
    }

    public abstract class MToken
    {
    }
    public class MToken_Constant : MToken
    {
        public string value;
    }
    public class MToken_Variable : MToken
    {
        public string name;
    }
    public class MToken_Bracket : MToken
    {
        public bool isClosing;
    }
}