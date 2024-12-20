public static class Tokenizer
{
	public static List<Token> Tokenize(string text, CompilationContext ctx)
	{
		List<Token> tokens = new List<Token>();
		
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
						ctx = ctx.CreateSubContext(functionDeclaration.functionName);
                    }
					else if (token is Token_Return)
					{
						ctx = ctx.parent;
					}

                    tokens.Add(token);
                }
			}
		}
		
		return tokens;
	}
	public static Token TokenizeLine(string[] lines, int lineIndex, bool isCommentWaiting, CompilationContext ctx)
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
		
		// Function
		if (words[0].Contains("(") && words[0].Contains(")"))
		{
			string functionName = words[0].Split('(')[0];
			// Function declaration
			if (nextLine.Contains("{"))
			{
				return new Token_FunctionDeclaration()
				{
					functionName = functionName
				};
			}
			// Function call
			else
			{
				return new Token_FunctionCall()
				{
					functionName = functionName
				};
			}
		}
		
		// Print
		if (words[0].Contains("print"))
		{
			string offset = ctx.GetRSPIndex(words[1]);
			return new Token_Print()
			{
				pointerToString = offset
            };
		}
        if (words[0].Contains("writeline"))
        {
            string offset = ctx.GetRSPIndex(words[1]);
            return new Token_WriteConsole()
            {
                pointerToString = offset
            };
        }


        if (words[0] == "return")
		{
			return new Token_Return();	
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

		if (words[0] == "int" || words[0] == "string")
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
                    variableName = words[0],
                    value = value
                };
            }
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
                variableToAssign = words[0]
            };
        }
        else
        {
            return new Token_VariableAssign()
            {
                variableName = words[0],
                value = value
            };
        }
    }
}
