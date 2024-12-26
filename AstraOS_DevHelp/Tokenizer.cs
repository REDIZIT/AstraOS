public static class Tokenizer
{	
	public static List<Token> Tokenize(string text, ScopeContext ctx)
	{
		List<Token> tokens = new(128);

		//
		// Lexer
		//
		Lexer.RawCodeToTokens(text, ctx, tokens);

		//
		// Scan for data types
		//
		DiscoverTypes(tokens, ctx.space);


        //
        // Split complex tokens for more simple
        //
        SplitTokens(tokens);

        //
        // Resolve refs
        //
        ResolveRefs(tokens);

        return tokens;
	}

    private static void SplitTokens(List<Token> tokens)
	{
        int j = 0;
        while (j < tokens.Count)
        {
            j = tokens[j].Simplify(tokens, j);
            j++;
        }
    }

	private static void DiscoverTypes(List<Token> tokens, Namespace namespaceToFill)
	{
        //
        // Note:
        // Here you're not allowed to cast string type names into ClassType due to not all ClassTypes have been discovered yet.
        // To cast ClassType use ResolveRefs stage.
        //

        for (int i = 0; i < tokens.Count; i++)
        {
            Token token = tokens[i];
            if (token is Token_Struct tokenStruct)
            {
                ClassType type = new ClassType()
                {
                    name = tokenStruct.name,
                    sizeInBytes = 4 // TODO: calc based on fields sizes
                };

                // Go thro struct and register functions and fields

                int depth = 0;
                i++;
                while (i < tokens.Count)
                {
                    Token structSubToken = tokens[i];

                    if (structSubToken is Token_BlockBegin)
                    {
                        depth++;
                    }
                    else if (structSubToken is Token_BlockEnd)
                    {
                        depth--;
                    }

                    if (depth <= 0) break;

                    if (structSubToken is Token_FunctionDeclaration tokenFunc)
                    {
                        FunctionInfo funcInfo = new FunctionInfo()
                        {
                            parent = type,
                            name = tokenFunc.functionName,
                            arguments = null,
                            returns = null,
                        };
                        type.functions.Add(tokenFunc.functionName, funcInfo);

                        tokenFunc.info = funcInfo;


                        // Link with return token
                        Token_Return tokenReturn = (Token_Return)tokens.Skip(i).First(t => t is Token_Return);
                        tokenReturn.functionInfo = funcInfo;
                    }
                    if (structSubToken is Token_VariableDeclaration tokenField && depth == 1)
                    {
                        FieldInfo info = new FieldInfo()
                        {
                            parent = type,
                            name = tokenField.name,
                        };
                        type.fields.Add(info);
                        type.fieldInfoByName.Add(info.name, info);

                        tokenField.info = info;
                    }

                    i++;
                }

                namespaceToFill.classes.Add(type.name, type);
            }
        }
    }

    private static void ResolveRefs(List<Token> tokens)
    {
        foreach (Token token in tokens.Where(t => t is Token_FunctionDeclaration))
        {
            token.ResolveRefs();
        }
        foreach (Token token in tokens.Where(t => t is Token_FunctionCall))
        {
            token.ResolveRefs();
        }
        foreach (Token token in tokens.Where(t => t is Token_VariableDeclaration))
        {
            token.ResolveRefs();
        }
    }
}
