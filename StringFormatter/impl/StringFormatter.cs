using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Text;
using StringFormatter.impl.Automaton;

namespace StringFormatter.impl;

public class StringFormatter : IStringFormatter
{
    public static readonly StringFormatter Shared = new();
    private readonly FiniteAutomaton _parser = new();
    private static readonly ConcurrentDictionary<Expression<Func<object, string>>, Func<object, string>> Cache = new();

    public string Format(string template, object target)
    {
        var tokens = _parser.ParseString(template);
        return CreateString(tokens, target);
    }

    private string CreateString(List<Token> tokens, object target)
    {
        StringBuilder str = new StringBuilder();
        foreach(Token TemporaryToken in tokens)
        {
            str.Append(ParseToken(TemporaryToken, target));
        }
        return str.ToString();
    }

    private string ParseToken(Token token, object target)
    {
        switch (token.Type)
        {
            case TokenType.String:
                return token.Value;
            case TokenType.Substitution:
                return GetStringFieldValue(token.Value, target);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private string GetStringFieldValue(string fieldName, object target)
    {
        return GetCachedFunc(fieldName, target).Invoke(target);
    }

    private Func<object, string> GetCachedFunc(string fieldName, object target)
    {
        var param = Expression.Parameter(typeof(object));
        var cast = Expression.Convert(param, target.GetType());
        var prop = Expression.PropertyOrField(cast, fieldName);
        var methodCall = Expression.Call(prop, "ToString", null);
        var expr = Expression.Lambda<Func<object, string>>(methodCall, param);
        return Cache.GetOrAdd(expr, k => k.Compile());
    }
}