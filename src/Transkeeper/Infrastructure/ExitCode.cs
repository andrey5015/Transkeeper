namespace Transkeeper.Infrastructure;

public enum ExitCode
{
    Success = 0,
    SystemError = 1,
    DuplicateId = 101,
    NotFound = 102,
    InvalidInput = 103
}