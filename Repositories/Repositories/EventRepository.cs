﻿using Domain.Entities;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Repositories.Helper;
using Repositories.Interfaces;

namespace Repositories.Repositories
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        private readonly StudentEventForumDbContext _context;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;

        public EventRepository(StudentEventForumDbContext studentEventForumDbContext, ICurrentTime timeService, IClaimsService claims) : base(studentEventForumDbContext, timeService, claims)

        {
            _context = studentEventForumDbContext;
            _timeService = timeService;
            _claimsService = claims;
        }

        public IQueryable<Event> FilterAllField(EventParams eventParams)
        {
            var query = _context.Events
                .Include(x => x.User)
                .Include(x => x.EventCategory)
                //.Include(x => x.EventCampaigns)
                .Search(eventParams.SearchTerm)
                .Filter(eventParams.EventCategoryId)
                .FilterByUserId(eventParams.UserId)
                .FilterByEventDate(eventParams.EventStartDate, eventParams.EventEndDate)
                .FilterByStatus(eventParams.Status.ToString());

            return query;
        }
    }
}