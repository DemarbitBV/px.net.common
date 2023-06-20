using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Px.Net.Common.Exceptions;

namespace Px.Net.Common.Repositories
{
    /// <summary>
    /// Base interface for building UnitOfWork classes.
    /// </summary>
    /// <typeparam name="TContext">Implementation of <see cref="DbContext"/></typeparam>
	public interface IUnitOfWork<TContext>
        where TContext : DbContext
	{
        /// <summary>
        /// Get a generic repository for a TEntity class.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IDbRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        /// <summary>
        /// Starts a new transaction on the database
        /// <exception cref="InvalidOperationException"></exception>
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Starts a new transaction on the database
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits the current transaction
        /// <exception cref="InvalidTransactionException" />
        /// </summary>
        void Commit();

        /// <summary>
        /// Commits the current transaction
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <exception cref="InvalidTransactionException" />
        /// <returns></returns>
        Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolls back the current transaction       
        /// <exception cref="InvalidTransactionException" />
        /// </summary>
        void Rollback();

        /// <summary>
        /// Rolls back the current transaction
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <exception cref="InvalidTransactionException" />
        /// <returns></returns>
        Task RollbackAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Save current changes to the database
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Save current changes to the database
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
	}

	public abstract class UnitOfWork<TContext> : IUnitOfWork<TContext>, IDisposable
        where TContext : DbContext
	{
        private bool disposedValue;
        private IDbContextTransaction? _currentTransaction;

        protected readonly TContext _context;
        protected readonly ILogger _logger;

        protected readonly string _databaseName;

        public UnitOfWork(TContext context, ILogger logger)
		{
            _context = context;
            _logger = logger;

            _databaseName = context.GetType().Name;
		}

        public IDbRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class
        {
            return new DbRepository<TEntity>(_context, _logger);
        }

        public void BeginTransaction()
        {
            if (_currentTransaction != null)
            {
                throw new InvalidOperationException($"A transaction is already in progress for database {_databaseName}");
            }

            _logger.LogDebug("Starting a new transaction on database {Name}", _databaseName);

            _currentTransaction = _context.Database.BeginTransaction();
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
            {
                throw new InvalidOperationException($"A transaction is already in progress for database {_databaseName}");
            }

            _logger.LogDebug("Starting a new transaction on database {Name}", _databaseName);

            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public void Commit()
        {
            if (_currentTransaction == null)
            {
                throw new InvalidTransactionException($"No active transaction for database {_databaseName}");
            }

            try
            {
                _logger.LogDebug("Committing current transaction for database {Name}", _databaseName);

                _context.SaveChanges();
                _currentTransaction.Commit();
            }
            finally
            {
                _currentTransaction.Dispose();
            }
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null)
            {
                throw new InvalidTransactionException($"No active transaction for database {_databaseName}");
            }

            try
            {
                _logger.LogDebug("Committing current transaction for database {Name}", _databaseName);

                await _context.SaveChangesAsync(cancellationToken);
                await _currentTransaction.CommitAsync(cancellationToken);
            }
            finally
            {
                _currentTransaction.Dispose();
            }
        }

        public void Rollback()
        {
            if (_currentTransaction == null)
            {
                throw new InvalidTransactionException($"No active transaction for database {_databaseName}");
            }

            _logger.LogDebug("Rolling back current transaction for database {Name}", _databaseName);

            _currentTransaction.Rollback();
            _currentTransaction.Dispose();
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null)
            {
                throw new InvalidTransactionException($"No active transaction for database {_databaseName}");
            }

            _logger.LogDebug("Rolling back current transaction for database {Name}", _databaseName);

            await _currentTransaction.RollbackAsync(cancellationToken);
            _currentTransaction.Dispose();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        #region IDisposable implementation
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _currentTransaction?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

