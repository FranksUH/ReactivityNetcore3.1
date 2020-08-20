using Application.Errors;
using Infrastructure;
using MediatR;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Activities
{
    public class Delete
    {
        public class Control : IRequest
        {
            public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Control>
        {
            private readonly DataContext _dataContext;

            public Handler(DataContext dataContext)
            {
                this._dataContext = dataContext;
            }

            public async Task<Unit> Handle(Control request, CancellationToken cancellationToken)
            {
                var toDelete = await _dataContext.Activities.FindAsync(request.Id);
                if (toDelete == null)
                    throw new RestException(HttpStatusCode.NotFound, new { activity = "Not Found" });

                await Task.Run(() => _dataContext.Activities.Remove(toDelete));

                if (await _dataContext.SaveChangesAsync() > 0)
                    return Unit.Value;

                throw new Exception("Error saving Activity");
            }
        }
    }
}
