namespace Noizera.BackgroundJobs.Common;

internal sealed class RetryException : Exception
{
    public int DelayInSeconds { get; } = 5;

    public RetryException(int delayInSeconds)
    {
        DelayInSeconds = delayInSeconds;
    }

    public RetryException()
    {
    }

    public RetryException(string message) : base(message)
    {
    }

    public RetryException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
