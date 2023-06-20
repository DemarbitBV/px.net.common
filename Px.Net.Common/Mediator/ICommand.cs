using System;
using MediatR;
using Px.Net.Common.Models;

namespace Px.Net.Common.Mediator
{
	/// <summary>
	/// Marker interface to represent a Command with a standard <see cref="MediatorResult"/> response.
	/// </summary>
	public interface ICommand : IRequest<MediatorResult> { }

	/// <summary>
	/// Handler definition for the <see cref="ICommand"/> interface.
	/// </summary>
	/// <typeparam name="TCommand"></typeparam>
	public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, MediatorResult>
		where TCommand : ICommand
	{

	}
}

