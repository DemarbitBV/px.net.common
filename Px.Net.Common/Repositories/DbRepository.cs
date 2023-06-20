using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Px.Net.Common.Exceptions;
using Px.Net.Common.Extensions;

namespace Px.Net.Common.Repositories
{
	/// <summary>
	/// Generic database repository
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public interface IDbRepository<TEntity>
		where TEntity : class
	{
		#region Create methods
		/// <summary>
		/// Tracks a new record by settings it's state to <see cref="EntityState.Added"/>
		/// </summary>
		/// <param name="entity"></param>
		void Insert(TEntity entity);
		#endregion

		#region Read methods
		/// <summary>
		/// Check if any records exists for the current Entity Type
		/// </summary>
		/// <param name="filter">Optional filter expression</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Count the number of records in the set
		/// </summary>
		/// <param name="filter">Optional filter expression</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Returns a list of records in the set given a specific filter
		/// </summary>
		/// <param name="filter">Optional filter expression</param>
		/// <param name="includes">Optional include list where includes should be separated by a comma</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? filter = null, string? includes = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a list of records in the set given a specific filter
        /// </summary>
		/// <param name="selector">Selector expression used in the Select statement of the query.
        /// <param name="filter">Optional filter expression</param>
        /// <param name="includes">Optional include list where includes should be separated by a comma</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TResult>> ListAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? filter = null, string? includes = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get a single record based on it's Id
		/// </summary>
		/// <param name="id"></param>
		/// <param name="includes"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<TEntity?> GetAsync(string id, string? includes = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get's a single record based on it's Id
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="id"></param>
		/// <param name="selector"></param>
		/// <param name="includes"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<TResult?> GetAsync<TResult>(string id, Expression<Func<TEntity, TResult>> selector, string? includes = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the first available record given the filter
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="includes"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>>? filter = null, string? includes = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the first available record given the filter
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <param name="filter"></param>
		/// <param name="includes"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<TResult?> GetAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? filter = null, string? includes = null, CancellationToken cancellationToken = default);
		#endregion

		#region Update methods
		/// <summary>
		/// Tracks a record by setting it's state to <see cref="EntityState.Modified"/>
		/// </summary>
		/// <param name="entity"></param>
		void Update(TEntity entity);

		/// <summary>
		/// Fetch the record based on it's Id and update that record's values from the <paramref name="entity"/> object.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="entity"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task UpdateAsync(string id, TEntity entity, CancellationToken cancellationToken = default);

		/// <summary>
		/// Fetch the record based on it's Id and update that record's values from the <paramref name="values"/> dictionary.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="values"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task UpdateAsync(string id, Dictionary<string, object?> values, CancellationToken cancellationToken = default);
		#endregion

		#region Delete methods
		/// <summary>
		/// Marks entity as deleted by setting it's state to <see cref="EntityState.Deleted"/>
		/// </summary>
		/// <param name="entity"></param>
		void Delete(TEntity entity);

		/// <summary>
		/// Search the record by it's id and mark it as deleted
		/// </summary>
		/// <param name="id"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task DeleteAsync(string id, CancellationToken cancellationToken = default);
		#endregion

		#region Save methods
		/// <summary>
		/// Save all changes
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task SaveChangesAsync(CancellationToken cancellationToken = default);
		#endregion
	}

	/// <summary>
	/// Generic database repository
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
    public class DbRepository<TEntity> : IDbRepository<TEntity>
		where TEntity : class
	{
		private readonly DbContext _context;
		private readonly DbSet<TEntity> _set;

		private readonly ILogger _logger;

		private readonly string _entityName;

		public DbRepository(DbContext context, ILogger logger)
		{
			_context = context;
			_set = _context.Set<TEntity>();

			_logger = logger;

			_entityName = typeof(TEntity).Name;
		}

		#region Create methods
		public void Insert(TEntity entity)
		{
			_logger.LogTrace("Creating new {Entity} record {@Record}", _entityName, entity);

			_set.Add(entity);
		}
		#endregion

		#region Read methods
		public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
		{
			_logger.LogTrace("Checking existence of {Entity} records with filter: {Filter}",
				_entityName,
				filter.Readable());

			var query = BuildQuery(filter: filter);

			var recordsExist = await query.AnyAsync(cancellationToken);

			if (recordsExist)
				_logger.LogTrace("{Entity} records found", _entityName);
			else
				_logger.LogTrace("No {Entity} records found", _entityName);

			return recordsExist;
		}

		public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
		{
			_logger.LogTrace("Checking number of {Entity} records with filter: {Filter}",
				_entityName,
				filter.Readable());

			var query = BuildQuery(filter: filter);

			var numberOfRecords = await query.CountAsync(cancellationToken);

			_logger.LogTrace("{Count} {Entity} records found", numberOfRecords, _entityName);

			return numberOfRecords;
		}

        public async Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? filter = null, string? includes = null, CancellationToken cancellationToken = default)
		{
			_logger.LogTrace(
				"Fetching {Name} records with filter: {Filter}",
				_entityName,
				filter.Readable());

			var query = BuildQuery(filter: filter, includes: includes);

			var records = await query.ToListAsync(cancellationToken);

			_logger.LogTrace(
				"Fetching {Count} {Name} records",
				records.Count,
				_entityName);

			return records;
		}

        public async Task<List<TResult>> ListAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? filter = null, string? includes = null, CancellationToken cancellationToken = default)
		{
			_logger.LogTrace(
				"Fetching {Name} records with filter: {Filter} and selector: {Selector}",
				_entityName,
				filter.Readable(),
				selector.Readable());

			var query = BuildQuery(filter: filter, includes: includes);

			var records = await query.Select(selector).ToListAsync(cancellationToken);

			_logger.LogTrace(
				"Fetched {Count} {Name} records",
				records.Count,
				typeof(TResult).Name);

			return records;
		}

        public async Task<TEntity?> GetAsync(string id, string? includes = null, CancellationToken cancellationToken = default)
		{
			_logger.LogTrace(
				"Fetching {Name} record {Id}",
				_entityName,
				id);

			var query = BuildQuery(id, includes: includes);

			var record = await query.FirstOrDefaultAsync(cancellationToken);

			if (record == null)
				_logger.LogError("{Name} record {id} not found", _entityName, id);

			return record;			
		}

		public async Task<TResult?> GetAsync<TResult>(string id, Expression<Func<TEntity, TResult>> selector, string? includes = null, CancellationToken cancellationToken = default)
		{
            _logger.LogTrace(
                "Fetching {Name} record {Id}",
                _entityName,
                id);

            var query = BuildQuery(id, includes: includes);

            var record = await query.Select(selector).FirstOrDefaultAsync(cancellationToken);

            if (record == null)
                _logger.LogError("{Name} record {id} not found", _entityName, id);

            return record;
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>>? filter = null, string? includes = null, CancellationToken cancellationToken = default)
		{
            _logger.LogTrace(
                "Fetching first {Name} record with filter: {Filter}",
                _entityName,
                filter.Readable());

            var query = BuildQuery(filter: filter, includes: includes);

            var record = await query.FirstOrDefaultAsync(cancellationToken);

            if (record == null)
                _logger.LogError("No {Name} record found", _entityName);

            return record;
        }

        public async Task<TResult?> GetAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? filter = null, string? includes = null, CancellationToken cancellationToken = default)
		{
            _logger.LogTrace(
               "Fetching first {Name} record with filter: {Filter}",
               _entityName,
               filter.Readable());

            var query = BuildQuery(filter: filter, includes: includes);

            var record = await query.Select(selector).FirstOrDefaultAsync(cancellationToken);

            if (record == null)
                _logger.LogError("No {Name} record found", _entityName);

            return record;
        }
        #endregion

        #region Update methods
        public void Update(TEntity entity)
		{
			_set.Update(entity);
		}

		public async Task UpdateAsync(string id, TEntity entity, CancellationToken cancellationToken = default)
		{
			var record = await _set.FindAsync(new[] { id } , cancellationToken: cancellationToken);

			if (record == null)
			{
				throw new RecordNotFoundException($"{_entityName} record {id} not found");
			}

			record.Merge(entity);

			_set.Update(record);
		}

		public async Task UpdateAsync(string id, Dictionary<string, object?> values, CancellationToken cancellationToken = default)
		{
			var record = await _set.FindAsync(new[] { id }, cancellationToken: cancellationToken);

			if (record == null)
			{
				throw new RecordNotFoundException($"{_entityName} record {id} not found");
			}

			record.Merge(values);

			_set.Update(record);
		}
		#endregion

		#region Delete methods
		public void Delete(TEntity entity)
		{
			_set.Remove(entity);
		}

		public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
		{
			var record = await _set.FindAsync(new[] { id }, cancellationToken: cancellationToken);

			if (record == null)
			{
				throw new RecordNotFoundException($"{_entityName} record {id} not found");
			}

			_set.Remove(record);
		}
		#endregion

		#region Save methods
		public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			await _context.SaveChangesAsync(cancellationToken);
		}
		#endregion

		#region Base methods
		protected virtual IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, Expression<Func<TEntity, bool>>? filter = null)
		{
			if (filter != null)
				query = query.Where(filter);

			return query;
		}

		protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query, string[]? includes = null)
		{
			if (includes == null)
				return query;

			foreach (var include in includes)
			{
				var nested = include.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

				query = query.Include(nested[0]);
			}

			return query;
		}
		#endregion

		#region Helper methods
		private IQueryable<TEntity> BuildQuery(string id, string? includes = null)
		{
			var query = _set.AsQueryable();

			query = query.Where(BuildIdFilter(id));

			query = ApplyIncludes(query, GetIncludes(includes));

			return query;
		}

		private IQueryable<TEntity> BuildQuery(Expression<Func<TEntity, bool>>? filter = null, string? includes = null)
		{
			var query = _set.AsQueryable();

			query = ApplyFilter(query, filter);
			query = ApplyIncludes(query, GetIncludes(includes));

			return query;
		}

		private Expression<Func<TEntity, bool>> BuildIdFilter(string id)
		{
			var parameter = Expression.Parameter(typeof(TEntity), "x");
			var property = Expression.Property(parameter, "Id");
			var constant = Expression.Constant(id);
			return Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(property, constant), parameter);
		}

		private string[]? GetIncludes(string? includes = null)
		{
			return includes?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		}
		#endregion
	}
}

