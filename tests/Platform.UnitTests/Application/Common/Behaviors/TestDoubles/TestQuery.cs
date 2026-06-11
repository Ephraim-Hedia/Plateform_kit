using MediatR;
using Platform.Domain.Results;

namespace Platform.UnitTests.Application.Common.Behaviors.TestDoubles;

public sealed record TestQuery(string Name) : IRequest<Result<string>>;
