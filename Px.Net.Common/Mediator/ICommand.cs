using System;
using MediatR;
using Px.Net.Common.Models;

namespace Px.Net.Common.Mediator
{
	public interface ICommand : IRequest<MediatorResult> { }

	public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, MediatorResult>
		where TCommand : ICommand
	{

	}
}

