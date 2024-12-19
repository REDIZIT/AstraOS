using System.Diagnostics;

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

	int a
	a = 123
	
	if a
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
		Stopwatch w = Stopwatch.StartNew();

		string asm = Compile(source);
		Console.WriteLine(asm);

		w.Stop();
		Console.WriteLine("\nCompiled in: " + w.ElapsedMilliseconds + " ms");
	}
	
	public static string Compile(string source)
	{
		CompilationContext ctx = new();

		List<Token> tokens = Tokenizer.Tokenize(source, ctx);
		//Console.WriteLine(string.Join(", ", tokens.Select(t => t.ToString())));

        string asm = Generator.Generate(tokens, ctx);
		
		return Generator.FormatAsm(asm);
	}
}