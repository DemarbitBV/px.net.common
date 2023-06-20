using System;
using MediatR;
using Px.Net.Common.Models;

namespace Px.Net.Common.Mediator
{
    /// <summary>
    /// Marker interface to represent a Query with a standard <see cref="MediatorResult"/> response.
    /// </summary>
    public interface IQuery : IRequest<MediatorResult> { }

    /// <summary>
	/// Handler definition for the <see cref="IQuery"/> interface.
	/// </summary>
	/// <typeparam name="TQuery"></typeparam>
	public interface IQueryHandler<TQuery> : IRequestHandler<TQuery, MediatorResult>
		where TQuery : IQuery
	{

	}
}

