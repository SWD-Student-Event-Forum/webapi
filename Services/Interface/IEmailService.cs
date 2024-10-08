﻿using EventZone.Domain.DTOs.EmailModels;

namespace EventZone.Services.Interface
{
    public interface IEmailService
    {
        Task SendEmail(Message message);

        Task SendHTMLEmail(Message message);
    }
}