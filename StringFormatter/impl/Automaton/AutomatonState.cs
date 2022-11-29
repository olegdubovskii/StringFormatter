namespace StringFormatter.impl.Automaton;

public enum AutomatonState
{
    Init = 0,
    PreFieldName,
    FieldName,
    EndField,
    PreEscape,
    Escape,
    Exception
}