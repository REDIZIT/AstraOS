public class CompilationContext
{
	public int LastOffset => offsetByVariableName.Count == 0 ? 0 : offsetByVariableName[variableNameOrdered.Last()];

	public Dictionary<string, int> offsetByVariableName = new();
	public Dictionary<string, int> sizeInBytesByVariableName = new();
	public Dictionary<string, string> typeNameByVariableName = new();
	public List<string> variableNameOrdered = new();

	public CompilationContext parent;
	public Dictionary<string, CompilationContext> childrenContextByFunctionName = new();

	public string prefix = null;

	public Dictionary<string, FunctionInfo> functionTypeByName = new();
	public ClassType currentClassType;

	public Namespace space;


	public int AllocVariable(string name, int sizeInBytes, string type)
	{
		if (sizeInBytes <= 0) throw new Exception($"Invalid argument 'sizeInBytes'. Expected value > 0, but given {sizeInBytes}");
		if (offsetByVariableName.ContainsKey(name)) throw new Exception($"Variable with name '{name}' already allocated inside context");

		int lastOffset = LastOffset - sizeInBytes;

		variableNameOrdered.Add(name);
		offsetByVariableName.Add(name, lastOffset);
		sizeInBytesByVariableName.Add(name, sizeInBytes);
		typeNameByVariableName.Add(name, type);

		return LastOffset;
	}
	public void AllocVariableAt(string name, ClassType type, int offset)
	{
        variableNameOrdered.Prepend(name);
        offsetByVariableName.Add(name, offset);
        sizeInBytesByVariableName.Add(name, type.sizeInBytes);
        typeNameByVariableName.Add(name, type.name);
    }
	public void FreeVariable(string name)
	{
		if (variableNameOrdered.Last() != name)
		{
			throw new Exception($"Failed to register variable's free due to passed name '{name}' is not on top of the stack. Now, top variable is '{variableNameOrdered.Last()}'");
		}

		variableNameOrdered.Remove(name);
		offsetByVariableName.Remove(name);
        sizeInBytesByVariableName.Remove(name);
        typeNameByVariableName.Remove(name);
    }

	public string GetRSPIndex(string variableName)
	{
        int localOffset = offsetByVariableName[variableName];
        return $"[rbp" + (localOffset < 0 ? localOffset : " + " + localOffset) + "]";
    }
	public int GetSizeInBytes(string variableName)
	{
		return sizeInBytesByVariableName[variableName];
    }
    public int GetOffset(string variableName)
    {
        return offsetByVariableName[variableName];
    }

	public CompilationContext CreateSubContext(string functionName, string prefix = null)
	{
		CompilationContext ctx = new()
		{
			parent = this,
			space = space,
			prefix = prefix,
        };
		childrenContextByFunctionName.Add(functionName, ctx);
		return ctx;
    }
}

public class Namespace
{
	public string name;
	public Dictionary<string, ClassType> classes = new();

    public Namespace(string name)
    {
		this.name = name;
    }
}
public class ClassType
{
	public string name;
	public int sizeInBytes;

	public Dictionary<string, FunctionInfo> functions = new();

    public override string ToString()
    {
		return "ClassType: " + name;
    }
}
public class FunctionInfo
{
	public ClassType parent;
	public string name;
	public List<ClassType> arguments = new();
	public List<ClassType> returns = new();
}