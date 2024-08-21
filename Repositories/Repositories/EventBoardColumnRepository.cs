﻿using EventZone.Domain.Entities;
using EventZone.Repositories.Interfaces;

namespace EventZone.Repositories.Repositories
{
    public class EventBoardColumnRepository : GenericRepository<EventBoardColumn>, IEventBoardColumnRepository
    {
        private readonly StudentEventForumDbContext _context;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public EventBoardColumnRepository(StudentEventForumDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
            _context = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
    }
}
