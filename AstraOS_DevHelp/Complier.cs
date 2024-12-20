using System.Diagnostics;

public static class Compiler
{
	public static string source = @"
	
program_ñompiled_main()
{	
	int c = 1

	if c
	{
		my_func()
	}
	return
}

my_func()
{
	string str = My first string
	writeline str

	string str2 = My second string
	writeline str2

	writeline str
	writeline str2

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
        string asm = Generator.Generate(tokens, ctx);
		
		return Generator.FormatAsm(asm);
	}
}