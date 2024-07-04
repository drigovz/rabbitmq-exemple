namespace Shared.Services;

public abstract class BackgroundService : IHostedService, IDisposable
{
	private Task _task;
	private readonly CancellationTokenSource _cancelationTokenSource = new();

	protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

	public virtual Task StartAsync(CancellationToken cancellationToken)
	{
		_task = ExecuteAsync(_cancelationTokenSource.Token);
		return _task.IsCompleted ? _task : Task.CompletedTask;
	}

	public virtual async Task StopAsync(CancellationToken cancellationToken)
	{
		try
		{
			_cancelationTokenSource.Cancel();
		}
		finally
		{
			await Task.WhenAny(_task, Task.Delay(Timeout.Infinite, cancellationToken));
		}
	}

	public virtual void Dispose() => _cancelationTokenSource.Cancel();
}
