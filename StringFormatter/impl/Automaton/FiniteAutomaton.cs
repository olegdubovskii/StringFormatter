using System.Text;
using StringFormatter.impl.Exception;

namespace StringFormatter.impl.Automaton;

public class FiniteAutomaton
{
    
    private readonly AutomatonState[,] _transitionMatrix = 
    {
        { AutomatonState.PreFieldName, AutomatonState.Escape, AutomatonState.Exception, AutomatonState.Init, AutomatonState.Exception, AutomatonState.Init, AutomatonState.Init},
        { AutomatonState.PreEscape, AutomatonState.Exception, AutomatonState.EndField, AutomatonState.Init, AutomatonState.Escape, AutomatonState.Init,  AutomatonState.Init},
        { AutomatonState.Init, AutomatonState.FieldName, AutomatonState.FieldName, AutomatonState.Init, AutomatonState.Exception, AutomatonState.Init,  AutomatonState.Init}
    };

    private AutomatonState _currentState;

    public FiniteAutomaton()
    {
        _currentState = AutomatonState.Init;
    }

    public List<Token> ParseString(string str)
    {
        var tokens = new List<Token>();
        var tokenValue = new StringBuilder();
        _currentState = AutomatonState.Init;
        
        foreach (var c in str)
        {
            _currentState = GetNextState(c);
            
            switch (_currentState)
            {
                case AutomatonState.Init:
                    tokenValue.Append(c);
                    break;
                
                case AutomatonState.PreFieldName:
                    tokens.Add(new Token(TokenType.String, tokenValue.ToString()));
                    tokenValue.Clear();
                    break;
                
                case AutomatonState.FieldName:
                    tokenValue.Append(c);
                    break;
                
                case AutomatonState.EndField:
                    tokens.Add(new Token(TokenType.Substitution, tokenValue.ToString()));
                    tokenValue.Clear();
                    _currentState = AutomatonState.Init;
                    break;

                case AutomatonState.PreEscape:
                    tokens.Add(new Token(TokenType.String, tokenValue.ToString()));
                    break;
                
                case AutomatonState.Escape:
                    tokens.Add(new Token(TokenType.String, c.ToString()));
                    tokenValue.Clear();
                    _currentState = AutomatonState.Init;
                    break;
                case AutomatonState.Exception:
                    throw new InvalidStringException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (_currentState != AutomatonState.Init) throw new InvalidStringException();
        
        tokens.Add(new Token(TokenType.String, tokenValue.ToString()));

        return tokens;
    }

    
    private AutomatonState GetNextState(char symbol)
    {
        _currentState = symbol switch
        {
            '{' => _transitionMatrix[0, (int)_currentState],
            '}' => _transitionMatrix[1, (int)_currentState],
            _ => _transitionMatrix[2, (int)_currentState]
        };

        return _currentState;
    }
}