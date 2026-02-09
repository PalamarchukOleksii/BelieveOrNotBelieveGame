using GameCore.Enums;

namespace GameCore.Common;

public class Error
{
    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }

    public Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    public override string ToString() => $"{Code} ({Type}): {Message}";
}