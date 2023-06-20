using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Px.Net.Common.Contexts
{
	public interface IDatabaseLoader
	{
		Task ExecuteAsync(CancellationToken cancellationToken = default);
	}

	public abstract class DatabaseLoader<TContext> : IDatabaseLoader
		 where TContext : DbContext
	{
		protected readonly TContext _context;
		protected readonly ILogger _logger;

		protected readonly string _databaseName;

		public DatabaseLoader(TContext context, ILogger logger)			
		{
			_context = context;
			_logger = logger;

			_databaseName = typeof(TContext).Name;
		}

		protected abstract Task OnExecuteAsync(CancellationToken cancellationToken = default);

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
			await HandleMigrationsAsync(cancellationToken);
			await OnExecuteAsync(cancellationToken);
        }

		private async Task HandleMigrationsAsync(CancellationToken cancellationToken = default)
		{
			_logger.LogInformation("Checking migration status of database {Name}", _databaseName);

			var pendingMigrations = await _context.Database.GetPendingMigrationsAsync(cancellationToken);

			if (!pendingMigrations.Any())
			{
				_logger.LogInformation("There are no pending migrations for database {Name}", _databaseName);
			} else
			{
				_logger.LogInformation("Discovered {Count} pending migrations for database {Name}", _databaseName);
				await _context.Database.MigrateAsync(cancellationToken);
				_logger.LogInformation("All migrations have been applied to database {Name}", _databaseName);
			}
		}
    }
}

