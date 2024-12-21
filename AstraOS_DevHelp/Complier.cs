using Newtonsoft.Json;
using System.Diagnostics;

public static class Compiler
{
	public static string source = @"
	
program_ñompiled_main()
{
	int a = 0
	int b = 10

	for int i = [a..b], -1
	{
		int_to_string i buffer
		printbuffer
	}

	my_func2()

	return
}

my_func1()
{
	string str = body
	writeline str
	return
}
my_func2()
{
	string str = !!! exit !!!
	writeline str
	return
}

";
	
	public static void Main()
	{
        Stopwatch w = Stopwatch.StartNew();

		string asm = Compile(source);

		w.Stop();
		Console.WriteLine("\nCompiled in: " + w.ElapsedMilliseconds + " ms");

		Export(asm);
	}
	
	public static string Compile(string source)
	{
		CompilationContext ctx = new();

		List<Token> tokens = Tokenizer.Tokenize(source, ctx);

		int i = 0;
		while (i < tokens.Count)
		{
			tokens[i].Simplify(tokens, i);
			i++;
		}

        string asm = Generator.Generate(tokens, ctx);
		
		return Generator.FormatAsm(asm);
	}

	private static void Export(string asm)
	{
		string filepath = "../../../appsettings.json";

		if (File.Exists(filepath) == false)
		{
			Console.WriteLine("Export: no appsettings.json found");
            return;
        }

		AppSettings settings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(filepath));
		if (string.IsNullOrWhiteSpace(settings.compilationOutputFile))
		{
            Console.WriteLine("Export: compilationOutputFile is empty");
            return;
        }


        string header = @"section .data
	msg db ""String in .data section"", 0

section .bss
	buffer resb 32

section .text

extern console_writeline
extern int_to_string

global program_ñompiled_main
";

        File.WriteAllText(settings.compilationOutputFile, header + asm);
        Console.WriteLine("Export: success");
    }
}
public class AppSettings
{
	public string compilationOutputFile;
}