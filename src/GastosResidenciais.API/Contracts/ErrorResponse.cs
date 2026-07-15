namespace GastosResidenciais.API.Contracts;

public sealed record ErrorResponse(
    string Message,
    IReadOnlyCollection<string> Errors,
    int Status);

