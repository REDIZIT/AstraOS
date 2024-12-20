public class Token_VariableAssign : Token
{
    public string variableName;
    public string value;

    public override string Generate()
    {
        string variableRspIndex = ctx.GetRSPIndex(variableName);
        return $"mov qword {variableRspIndex}, {value}\n";
    }

    public static string Generate_Value_to_RSP(string value, string destRspIndex)
    {
        return $"mov qword {destRspIndex}, {value}\n";
    }
    public static string Generate_RSP_to_RSP(string sourceRspIndex, string destRspIndex)
    {
        return $"mov qword rbx, {sourceRspIndex}\nmov {destRspIndex}, rbx\n";
    }
    public static string Generate_RSP_to_Register(string sourceRspIndex, string destRegName)
    {
        return $"mov qword {destRegName}, {sourceRspIndex}\n";
    }
    public static string Generate_ValueOrRSP_to_RSP(string valueOrRsp, string destRspIndex)
    {
        if (IsRspIndex(valueOrRsp))
        {
            return Generate_RSP_to_RSP(valueOrRsp, destRspIndex);
        }
        else
        {
            return Generate_Value_to_RSP(valueOrRsp, destRspIndex);
        }
    }
    public static string Convert_ValueOrRSP_to_RSP(string valueOrRsp, CompilationContext ctx)
    {
        if (IsRspIndex(valueOrRsp))
        {
            return valueOrRsp;
        }
        else
        {
            return ctx.GetRSPIndex(valueOrRsp);
        }
    }

    public static bool IsRspIndex(string value)
    {
        return value.StartsWith("[") && value.EndsWith("]");
    }
}
