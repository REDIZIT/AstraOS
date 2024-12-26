public static class Lexer
{
    public static void RawCodeToTokens(string text, ScopeContext ctx, List<Token> listToFill)
    {
        int commentDepth = 0;

        string[] lines = text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            Token token = TokenizeLine(lines, i, commentDepth > 0, ctx);
            if (token != null)
            {
                if (token is Token_Comment comment)
                {
                    commentDepth += comment.isClosing ? -1 : 1;
                }
                else if (commentDepth <= 0)
                {
                    if (token is Token_FunctionDeclaration functionDeclaration)
                    {
                        ctx = ctx.CreateSubContext(functionDeclaration.functionName, ctx.prefix + "__fn_" + functionDeclaration.functionName);
                        functionDeclaration.ctx = ctx;
                    }
                    else if (token is Token_BlockEnd)
                    {
                        ctx = ctx.parent;
                    }
                    else if (token is Token_Struct structDeclaration)
                    {
                        ctx = ctx.CreateSubContext(structDeclaration.name, ctx.prefix + "__type_" + structDeclaration.name);
                    }

                    token.ctx = ctx;
                    listToFill.Add(token);
                }
            }
        }
    }

    private static Token TokenizeLine(string[] lines, int lineIndex, bool isCommentWaiting, ScopeContext ctx)
    {
        string line = lines[lineIndex].Trim();
        string[] words = line.Split(' ');

        string nextLine = "";
        string[] nextWords = new string[0];

        if (lineIndex + 1 < lines.Length)
        {
            nextLine = lines[lineIndex + 1];
            nextWords = nextLine.Split(' ');
        }

        if (string.IsNullOrWhiteSpace(line))
        {
            return null;
        }


        if (line == "--[" || line == "--]")
        {
            return new Token_Comment()
            {
                isClosing = line == "--]"
            };
        }

        if (isCommentWaiting) return null;

        if (line.Trim().StartsWith("--")) return null;

        // Function
        // declaration or call with no return
        if (words[0].Contains("(") && line.Contains(")"))
        {
            Token_Function tokenFunc;
            string functionName = line.Split('(')[0];

            // Function declaration
            if (nextLine.Contains("{"))
            {
                tokenFunc = new Token_FunctionDeclaration(ctx, line);
            }
            // Function call
            else
            {
                tokenFunc = new Token_FunctionCall(ctx, functionName);
            }

            tokenFunc.HandleArguments(line);

            return tokenFunc;
        }

        // Print
        if (words[0] == "print")
        {
            string offset = ctx.GetRSPIndex(words[0]);
            return new Token_Print()
            {
                pointerToString = offset
            };
        }
        if (words[0] == "writeline")
        {
            string offset = ctx.GetRSPIndex(words[1]);
            return new Token_WriteConsole()
            {
                pointerToString = offset
            };
        }
        if (words[0] == "printbuffer")
        {
            return new Token_WriteConsole_Buffer();
        }


        if (words[0] == "return")
        {
            return new Token_Return()
            {
                ctx = ctx,
                rawCode = string.Join(" ", words[1..])
            };
        }

        if (words[0] == "exit")
        {
            return new Token_Exit();
        }

        if (words[0] == "if")
        {
            return new Token_If()
            {
                expression = TokenizeVarOrExpression(words[1..])
            };
        }

        if (words[0] == "int" || words[0] == "string" || words[0] == "ptr")
        {
            string defaultValue = "0";
            if (words.Length > 3)
            {
                defaultValue = string.Join(" ", words.Skip(3));
            }

            return new Token_VariableDeclaration(words[0], words[1], defaultValue, ctx);
        }

        if (words.Length >= 3 && words[1] == "=")
        {
            string value = string.Join(" ", words.Skip(2));
            if (MathExpressions.IsExpression(value))
            {
                return new Token_MathExpression()
                {
                    expression = value,
                    variableToAssign = words[0]
                };
            }
            else
            {
                return new Token_VariableAssign()
                {
                    ctx = ctx,
                    variableName = words[0],
                    rawValue = value
                };
            }
        }

        if (words[0] == "int_to_string")
        {
            return new Token_IntToString()
            {
                valueOrRsp_int = words[1],
                rsp_outString = words[2]
            };
        }

        // While and for
        if (words[0] == "while")
        {
            return new Token_While()
            {
                expression = string.Join(" ", words.Skip(1))
            };
        }
        if (words[0] == "for")
        {
            string[] range = words[4].Replace("..", ".").TrimEnd(',').Split('.');

            bool isReversed = false;
            if (words.Length > 5 && int.TryParse(words[5], out int step))
            {
                isReversed = step < 0;
            }

            return new Token_For()
            {
                ctx = ctx,
                iteratorName = words[2],
                range_a = range[0][1..],
                include_a = range[0][0] == '[',
                range_b = range[1][..^1],
                include_b = range[1][^1] == ']',
                isReversed = isReversed
            };
        }
        if (words[0] == "struct")
        {
            return new Token_Struct(ctx, words[1]);
        }
        if (words[0].StartsWith("``"))
        {
            return new Token_AsmBlock()
            {

            };
        }

        if (line == "{") return new Token_BlockBegin();
        else if (line == "}") return new Token_BlockEnd();

        throw new Exception("Failed to tokenize line: '" + line + "'");
    }

    private static Token TokenizeVarOrExpression(string[] words)
    {
        string value = string.Join(" ", words);
        if (MathExpressions.IsExpression(value))
        {
            return new Token_MathExpression()
            {
                expression = value,
                variableToAssign = null
            };
        }
        else
        {
            return new Token_VariableAssign()
            {
                variableName = words[0],
                rawValue = value
            };
        }
    }
}
