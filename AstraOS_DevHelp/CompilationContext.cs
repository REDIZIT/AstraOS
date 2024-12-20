public class CompilationContext
{
	public Dictionary<string, int> offsetByVariableName = new();
	public Dictionary<string, int> sizeInBytesByVariableName = new();
	public List<string> variableNameOrdered = new();

	public int LastOffset => offsetByVariableName.Count == 0 ? 0 : offsetByVariableName[variableNameOrdered.Last()];

	public int AllocVariable(string name, int sizeInBytes)
	{
		if (sizeInBytes <= 0) throw new Exception($"Invalid argument 'sizeInBytes'. Expected value > 0, but given {sizeInBytes}");

		int lastOffset = LastOffset - sizeInBytes;

        variableNameOrdered.Add(name);
		offsetByVariableName.Add(name, lastOffset);
		sizeInBytesByVariableName.Add(name, sizeInBytes);

        return LastOffset;
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
}