using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Compiler
{
	public static string source = @"
	
_start()
{
	--[
	rbp = rsp

	int a = 1
	--[
	sub rsp, 4
	mov [rsp-4], 1
	add rsp, 4
	--]
	--]
	
	if true
	{
		my_func()
	}
	
	exit
}

my_func()
{
	print 'My test string'
	return
}

";
	
	public static void Main()
	{
		string asm = Compile(source);
		Console.WriteLine(asm);
	}
	
	public static string Compile(string source)
	{
		List<Token> tokens = Tokenizer.Tokenize(source);
		
		//Console.WriteLine(string.Join(", ", tokens.Select(t => t.ToString())));
		
		string asm = Generator.Generate(tokens);
		
		return asm;
	}
}

public static class Tokenizer 
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

/*public class Token_VariableDeclaration : Token
{
	public string variableType;
	public string variableName;
	public string variableDefaultValue;

	public override string Generate()
	{
		if (variableType == "int")
		{
			return "
		}
		throw new Exception("Unknown variable type: " + variableType);
	}
}
public class VariablesContext
{
	public Dictionary<string, int> offsetByName = new();
	
	public int Register(string name)
	{
		
		offsetByName.Add(name
	}
}*/



public static class Generator
{
	public static string Generate(List<Token> tokens)
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
					b.Append(Generate(bodyTokens));

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
                b.Append(tokens[i].Generate());
            }
		}
		
		return b.ToString();
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