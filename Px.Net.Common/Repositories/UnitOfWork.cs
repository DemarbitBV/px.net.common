using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Px.Net.Common.Exceptions;

namespace Px.Net.Common.Repositories
{
	public interface IUnitOfWork<TContext>
        where TContext : DbContext
	{
        IDbRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        void BeginTransaction();
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        void Commit();
        Task CommitAsync(CancellationToken cancellationToken = default);

        void Rollback();
        Task RollbackAsync(CancellationToken cancellationToken = default);

        void SaveChanges();
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

