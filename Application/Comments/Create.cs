using Application.DTOs;
using Application.Errors;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Comments
{
    public class Create
    {
        public class Command : IRequest<CommentDTO>
        {
            public string Body { get; set; }
            public string ActivityId { get; set; }
            public string UserName { get; set; }
        }
        public class Handler : IRequestHandler<Command, CommentDTO>
        {
            private DataContext _dataContext;
            private IMapper _mapper;
            public Handler(DataContext dataContext, IMapper mapper)
            {
                _dataContext = dataContext;
                _mapper = mapper;
            }
            public async Task<CommentDTO> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _dataContext.Activities.FindAsync(request.ActivityId);
                if (activity == null)
                    throw new RestException(System.Net.HttpStatusCode.NotFound, new { error = "Not found activity" });

                var user = await _dataContext.Users.FirstOrDefaultAsync(usr => usr.UserName == request.UserName);
                var comment = new Comment
                {
                    ActivityId = activity.Id,
                    AuthorId = user?.Id,
                    Body = request.Body,
                    CreatedAt = DateTime.Now,
                    Id= Guid.NewGuid().ToString()
                };

                _dataContext.Comments.Add(comment);

                if (await _dataContext.SaveChangesAsync() > 0)
                    return _mapper.Map<CommentDTO>(comment);

                throw new Exception("Problem saving changes");
            }
        }
    }
}
