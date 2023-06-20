using System;
using MediatR;
using Px.Net.Common.Models;

namespace Px.Net.Common.Mediator
{
	public interface IQuery : IRequest<MediatorResult> { }

	public interface IQueryHandler<TQuery> : IRequestHandler<TQuery, MediatorResult>
		where TQuery : IQuery
	{

	}
}

