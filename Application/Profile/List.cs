using Application.DTOs;
using Application.Extensions;
using Application.Interfaces;
using Infrastructure;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Profile
{
    public class List
    {
        public class Query : IRequest<List<ProfileDTO>> 
        {
            public string UserName { get; set; }
            public string Prdicate { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<ProfileDTO>>
        {
            private readonly DataContext _dataContext;
            private readonly IUserAccesor _userAccesor;
            public Handler(DataContext dataContext, IUserAccesor userAccesor)
            {
                _dataContext = dataContext;
                _userAccesor = userAccesor;
            }
            public async Task<List<ProfileDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                var result = new List<ProfileDTO>();
                switch(request.Prdicate)
                {
                    case "followers":  //the list of profiles that follow a given userName
                    {
                        var followers = await Task.Run(()=> _dataContext.Followings.Where(f => f.Target.UserName == request.UserName));
                        foreach (var item in followers)
                            result.Add(item.Observer.GetProfileDTOFor(_userAccesor.GetUserName()));
                        return result;
                    }                            
                    default: //the list of profiles that the given user follow
                    {
                        var followings = _dataContext.Followings.Where(f => f.Observer.UserName == request.UserName);
                        foreach (var item in followings)
                            result.Add(item.Target.GetProfileDTOFor(_userAccesor.GetUserName()));
                        return result;
                    }                        
                }
            }
        }
    }
}
