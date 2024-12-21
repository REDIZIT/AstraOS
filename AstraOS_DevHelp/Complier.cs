using Newtonsoft.Json;
using System.Diagnostics;

public static class Compiler
{
	public static string source = @"

struct program
{
	main()
	{
		int my_number = 42
		int my_second_number = 32

		ptr my_ptr
		my_ptr.set_value(my_ptr, my_number, my_second_number)

		return
	}
}

struct ptr
{
	set_value(ptr self, int a, int b)
	{
		int_to_string b buffer
		printbuffer

		return
	}
}

struct int
{
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
		CompilationContext ctx = new()
		{
			space = new Namespace("kernel")
		};

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

global program__main
";

        File.WriteAllText(settings.compilationOutputFile, header + asm);
        Console.WriteLine("Export: success");
    }
}
public class AppSettings
{
	public string compilationOutputFile;
}