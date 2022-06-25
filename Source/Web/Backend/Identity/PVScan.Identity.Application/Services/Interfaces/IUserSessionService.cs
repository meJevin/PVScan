using PVScan.Identity.Domain.Entities;
using PVScan.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.Application.Services.Interfaces
{
    public interface IUserSessionService
    {
        Task<(UserSession? Session, DomainResult Result)> StartNewSession(StartNewSessionData data);
        Task<DomainResult> TerminateSession(TerminateSessionData data);
        Task<DomainResult> TerminateSession(UserSession session);
    }

    public record StartNewSessionData(User User, RefreshToken RefreshToken, string? FromIp);
    public record TerminateSessionData(User User, RefreshToken RefreshToken);
}
