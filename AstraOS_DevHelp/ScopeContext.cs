public struct VariableInfo
{
	public ClassType type;
}

public class ScopeContext
{
	public int LastOffset => offsetByVariableName.Count == 0 ? 0 : offsetByVariableName[variableNameOrdered.Last()];

	public Dictionary<string, int> offsetByVariableName = new();
	public Dictionary<string, int> sizeInBytesByVariableName = new();
	public Dictionary<string, string> typeNameByVariableName = new();
	public List<string> variableNameOrdered = new();

	public int parentRelatedOffset;
	public ScopeContext parent;

	public string prefix = null;

	public Dictionary<string, FunctionInfo> functionTypeByName = new();

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
		return FormatRSP(GetIntRSPIndex(variableName));
    }
    public int GetIntRSPIndex(string variableName)
    {
        if (offsetByVariableName.TryGetValue(variableName, out int offset))
		{
			return offset + parentRelatedOffset;
		}
		else if (parent != null)
		{
			return parent.GetIntRSPIndex(variableName);
		}
		else
		{
			throw new Exception($"Failed to get RSP index. Variable '{variableName}' is out of scope.");
		}
    }
	public string FormatRSP(int offset)
	{
        return $"[rbp" + (offset < 0 ? offset : " + " + offset) + "]";
    }
    public int GetSizeInBytes(string variableName)
	{
		return sizeInBytesByVariableName[variableName];
    }
    public int GetOffset(string variableName)
    {
        return offsetByVariableName[variableName];
    }

	public VariableInfo GetVariable(string name)
	{
		if (typeNameByVariableName.TryGetValue(name, out string typeName))
		{
			return new VariableInfo()
			{
				type = space.classes[typeName]
			};
		}
		else if (parent != null)
		{
			return parent.GetVariable(name);
		}
		else
		{
			throw new Exception($"There is no variable with name '{name}' inside this scope and any parent. Seems, variable is out of scope.");
		}
    }

	public ScopeContext CreateSubContext(string functionName, string prefix = null)
	{
		ScopeContext ctx = new()
		{
			parent = this,
			space = space,
			prefix = prefix,
            parentRelatedOffset = LastOffset,
        };
		return ctx;
    }

	public string NextUniquePostfix()
	{
		return space.NextUniquePostfix();
    }
}

public class Namespace
{
	public string name;
	public Dictionary<string, ClassType> classes = new();

	private int uniquePostfixNumber;

    public Namespace(string name)
    {
		this.name = name;
    }

	public string NextUniquePostfix()
	{
		uniquePostfixNumber++;
		return "_" + uniquePostfixNumber;
    }
}
public class ClassType
{
	public string name;
	public int sizeInBytes;

	public Dictionary<string, FunctionInfo> functions = new();
	public Dictionary<string, FieldInfo> fieldInfoByName = new();

	public List<FieldInfo> fields = new();

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
public class FieldInfo
{
	public ClassType parent;
	public string name;
	public ClassType type;
	
	public int GetOffset()
	{
		int offset = 0;
		for (int i = 0; i < parent.fields.Count; i++)
		{
			if (parent.fields[i] == this) return offset;
			else offset += 4;
		}

		throw new Exception("Failed to calculate offset for FieldInfo");
	}
}