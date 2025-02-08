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
		ptr my_ptr


		for int y = [0..7)
		{
			for int x = [0..3)
			{
				int temp = 753664 + x * 2 + y * 2 * 80

				my_ptr.address = temp
				my_ptr.set_value(my_ptr, 48)
			}
		}
		

		return
	}
}

struct ptr
{
	int address

	set_value(ptr self, int value)
	{
		```
		mov rbx, [self]
		mov rcx, [value]
		mov qword [rbx], rcx
		mov qword [rbx+1], 0x02
		```
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
		ScopeContext ctx = new()
		{
			space = new Namespace("kernel")
		};

		List<Token> tokens = Tokenizer.Tokenize(source, ctx);

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