using System.Diagnostics;

public static class Compiler
{
	public static string source = @"
	
program_ñompiled_main()
{
	--[
	int a
	a = 1

	int b
	b = 2

	int c
	c = 10 - (a + b * 5)
	--]

	--[
	int a = 1
	int b = 1

	int c = a and b
	
	if c
	{
		my_func()
	}
	--]

	my_func()
	
	exit
}

my_func()
{
	string str = My string
	writeline str

	string str2 = My another string
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