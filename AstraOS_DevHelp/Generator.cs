using System.Text;

public static class Generator
{
	public static string Generate(List<Token> tokens, CompilationContext ctx)
	{
		StringBuilder b = new();
		
		for (int i = 0; i < tokens.Count; i++)
		{
			Token token = tokens[i];

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
					
					cmp rax, 1
					jge .if_true
					jmp .if_false
					.if_true:
					-- body
					jmp .if_end
					.if_false:
					-- body
					.if_end

					*/

					b.AppendLine("mov rax, 0"); // DEBUG LINE ('if true')

					b.AppendLine("cmp rax, 1");
					b.AppendLine("jge .if_true");
					b.AppendLine("jmp .if_end");
					b.AppendLine(".if_true:");

					var bodyTokens = tokens.Slice(blockBeginIndex + 1, blockEndIndex - blockBeginIndex - 1);
					Console.WriteLine(string.Join(", ", bodyTokens.Select(t => t.ToString())));
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
                lines[i] = "    " + line;
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
		for (int i = startIndex; i < tokens.Count - startIndex; i++)
		{
			Token token = tokens[i];
			if (predicate(token)) return i;
		}
		return -1;
	}
}

public class CompilationContext
{
	public Dictionary<string, int> offsetByVariableName = new();
	public List<string> variableNameOrdered = new();

	public int LastOffset => offsetByVariableName.Count == 0 ? 0 : offsetByVariableName[variableNameOrdered.Last()];

	public int AllocVariable(string name)
	{
		variableNameOrdered.Add(name);
		offsetByVariableName.Add(name, LastOffset + 4);

        return LastOffset;
	}
	public void FreeVariable(string name)
	{
		if (variableNameOrdered.Last() != name)
		{
			throw new Exception($"Failed to register variable's free due to passed name '{name}' is not on top of the stack. Now, top variable is '{variableNameOrdered.Last()}'");
		}

		variableNameOrdered.Remove(name);
		offsetByVariableName.Remove(name);
    }
}