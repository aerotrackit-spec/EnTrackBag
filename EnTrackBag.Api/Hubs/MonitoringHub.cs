using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EnTrackBag.Api.Hubs;

[Authorize(Policy = "Dashboard.View:VIEW")]
public class MonitoringHub : Hub
{
}
