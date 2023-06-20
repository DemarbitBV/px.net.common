using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Px.Net.Common.Contexts
{
	/// <summary>
	/// Shared interface for defining a database loader that can be run on app start up.
	/// </summary>
	public interface IDatabaseLoader
	{
		Task ExecuteAsync(bool allowMigration = true, CancellationToken cancellationToken = default);
	}

	/// <summary>
	/// Abstract implementation of a Database Loader
	/// </summary>
	/// <typeparam name="TContext">Implementation of <see cref="DbContext"/></typeparam>
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

		/// <summary>
		/// Run administrative tasks on startup such as loading default data into the database
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		protected abstract Task OnExecuteAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Execute the database loader
		/// </summary>
		/// <param name="allowMigration">Specifies whether the database should be migrated (defaults to <c>true</c>)</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
        public async Task ExecuteAsync(bool allowMigration = true, CancellationToken cancellationToken = default)
        {
			if (allowMigration)
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

