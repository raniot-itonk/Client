using System.Threading.Tasks;
using Client.Clients;
using Client.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Client.Controllers
{
    public class HistoryController : Controller
    {
        private readonly ILogger<HistoryController> _logger;
        private readonly IHistoryClient _historyClient;

        public HistoryController(ILogger<HistoryController> logger, IHistoryClient historyClient)
        {
            _logger = logger;
            _historyClient = historyClient;
        }

        [Authorize("client.UserActions")]
        public async Task<IActionResult> Index()
        {
            var (jwtToken, id) = JwtHelper.GetJwtAndIdFromJwt(Request);
            var history = await _historyClient.GetHistory(id, jwtToken);
            return View(history);
        }
    }
}