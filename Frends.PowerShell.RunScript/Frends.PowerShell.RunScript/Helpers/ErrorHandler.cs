using System.Runtime.ExceptionServices;
using Frends.PowerShell.RunScript.Definitions;

namespace Frends.PowerShell.RunScript.Helpers;

internal static class ErrorHandler
{
    internal static Result Handle(this Exception exception, Options options, bool throwCanceled = true)
    {
        ThrowIfCanceled(exception, throwCanceled);
        if (options.ThrowErrorOnFailure) ThrowBaseException(exception, options.ErrorMessageOnFailure);

        return ReturnResult(exception, options.ErrorMessageOnFailure);
    }

    private static void ThrowIfCanceled(Exception exception, bool throwCanceled = true)
    {
        if (throwCanceled && exception is OperationCanceledException) throw exception;
    }

    private static void ThrowBaseException(Exception exception, string customMessage = null)
    {
        if (string.IsNullOrEmpty(customMessage))
            ExceptionDispatchInfo.Capture(exception).Throw();

        throw new Exception(customMessage, exception);
    }

    private static Result ReturnResult(Exception exception, string customMessage = null)
    {
        var errorMessage = string.IsNullOrEmpty(customMessage)
            ? exception.Message
            : $"{customMessage}: {exception.Message}";

        return new Result(
            false,
            null,
            null,
            string.Empty,
            new Error
            {
                Message = errorMessage,
                AdditionalInfo = exception,
            });
    }
}
