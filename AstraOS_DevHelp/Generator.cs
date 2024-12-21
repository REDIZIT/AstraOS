using System.Text;

public static class Generator
{
	public static string Generate(List<Token> tokens, CompilationContext mainCtx)
	{
		StringBuilder b = new();
		CompilationContext ctx = mainCtx;
		
		for (int i = 0; i < tokens.Count; i++)
		{
			Token token = tokens[i];

			if (token is Token_FunctionDeclaration functionDeclaration)
			{
                ctx = functionDeclaration.ctx.parent.childrenContextByFunctionName[functionDeclaration.functionName];
            }
			else if (token is Token_Return)
			{
                ctx = ctx.parent;
			}

			if (token is Token_If tokenIf)
			{
				int blockBeginIndex = i + 1;
				int blockEndIndex = FirstIndexOf(tokens, t => t is Token_BlockEnd, blockBeginIndex);

                Token blockBegin = tokens[blockBeginIndex];
				Token blockEnd = tokens[blockEndIndex];

				// Valid tokens
				if (blockBegin is Token_BlockBegin && blockEnd is Token_BlockEnd)
				{
					/*
					
					cmp rbx, 1
					jge .if_true
					jmp .if_false
					.if_true:
					-- body
					jmp .if_end
					.if_false:
					-- body
					.if_end

					*/

					if (tokenIf.expression is Token_MathExpression mathExpression)
					{
						string asm = mathExpression.Generate(ctx);
						b.AppendLine(asm);
                        b.AppendLine("cmp rbx, 1");
                    }
					else if (tokenIf.expression is Token_VariableAssign variable)
					{
                        b.AppendLine($"cmp qword {ctx.GetRSPIndex(variable.variableName)}, 1");
                    }
					else
					{
						throw new Exception($"Unkown expression '{tokenIf.expression}' in TokenIf");
					}

					//b.AppendLine("cmp rbx, 1");
					b.AppendLine("jge .if_true");
					b.AppendLine("jmp .if_end");
					b.AppendLine(".if_true:");

					var bodyTokens = tokens.Slice(blockBeginIndex + 1, blockEndIndex - blockBeginIndex - 1);
					b.Append(Generate(bodyTokens, ctx));

					// generated body
					b.AppendLine("jmp .if_end");
					//b.AppendLine(".if_false:");
					//// generated body
					b.AppendLine(".if_end:");

					i = blockEndIndex;
				}
				else
				{
					throw new Exception("Invalid if statement tokens");
				}
			}
			else if (token is Token_While tokenWhile)
			{
				string asm = tokenWhile.Generate(ctx);

                int blockBeginIndex = i + 1;
                int blockEndIndex = FirstIndexOf(tokens, t => t is Token_BlockEnd, blockBeginIndex);

                Token blockBegin = tokens[blockBeginIndex];
                Token blockEnd = tokens[blockEndIndex];

                var bodyTokens = tokens.Slice(blockBeginIndex + 1, blockEndIndex - blockBeginIndex - 1);
				string bodyAsm = Generate(bodyTokens, ctx);

                asm = asm.Replace("%while_body%", bodyAsm);
				b.AppendLine(asm);

                i = blockEndIndex;
            }
			else
			{
                b.Append(tokens[i].Generate(ctx));
            }
		}

		return b.ToString();
	}

	public static string FormatAsm(string asm)
	{
		string[] lines = asm.Split('\n');

		for (int i = 0; i < lines.Length; i++)
		{
			string line = lines[i];

			if (line.Contains(":") == false)
			{
                // Tabs for lines inside function
                lines[i] = "\t" + line;
			}
			else
			{
				// Spacing for function naming
				if (line.Contains(".") == false)
				{
					lines[i] = "\n" + line;
				}
			}
		}

		return string.Join("\n", lines);
	}

	private static int FirstIndexOf(List<Token> tokens, Func<Token, bool> predicate, int startIndex)
	{
		for (int i = startIndex; i < tokens.Count; i++)
		{
			Token token = tokens[i];
			if (predicate(token)) return i;
		}
		return -1;
	}
}
