﻿public static class Tokenizer
{
	public static List<Token> Tokenize(string text)
	{
		List<Token> tokens = new List<Token>();
		
		int commentDepth = 0;
		
		string[] lines = text.Split('\n');
		for (int i = 0; i < lines.Length; i++)
		{
			Token token = TokenizeLine(lines, i, commentDepth > 0);
			if (token != null)
			{
				if (token is Token_Comment comment)
				{
					commentDepth += comment.isClosing ? -1 : 1;
					//Console.WriteLine(commentDepth);
				}
				else if (commentDepth <= 0)
				{
					//Console.WriteLine("Add token: " + token);
					tokens.Add(token);
				}
			}
		}
		
		return tokens;
	}
	public static Token TokenizeLine(string[] lines, int lineIndex, bool isCommentWaiting)
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
			return new Token_Print()
			{
				message = line.Split('\'')[1]
			};
		}
		
		// Return
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
				expression = null
			};
		}
		
		if (line == "{") return new Token_BlockBegin();
		else if (line == "}") return new Token_BlockEnd();
		
		throw new Exception("Failed to tokenize line: '" + line + "'");
	}
}