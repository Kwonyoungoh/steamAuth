using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using steamAuth.Data;
using steamAuth.Models;

namespace steamAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly DBContext _context;
        private const string SteamAuthUrl = "https://partner.steam-api.com/ISteamUserAuth/AuthenticateUserTicket/v1/";

        public UserController(IConfiguration configuration, HttpClient httpClient, DBContext context)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _context = context;

        }

        [HttpGet("getUserinfo/{steamId}")]
        public async Task<IActionResult> GetUserInfo(ulong steamId)
        {
            string apiKey = _configuration["SteamApiKey"];
            string url = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={apiKey}&steamids={steamId}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                ApiResponse jsonResponse = JsonConvert.DeserializeObject<ApiResponse>(content);
                Player player = jsonResponse.Response.Players.FirstOrDefault();

                if (player != null)
                {
                    return Ok(new { isValid = true, steamId = player.SteamId, name = player.PersonaName });
                }
            }

            return BadRequest(new { isValid = false });
        }

        [HttpPost("ValidateUserTicket")]
        public async Task<IActionResult> ValidateUserTicket([FromBody] ValidateTicketRequest request)
        {
            using (var httpClient = new HttpClient())
            {
                string queryString = $"key={_configuration["SteamApiKey"]}&appid={_configuration["SteamAppID"]}&ticket={request.AuthTicket}&identity={request.Identity}";
                string url = $"{SteamAuthUrl}?{queryString}";

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var authResponse = JsonConvert.DeserializeObject<AuthResponse>(json);

                    if (authResponse?.Response?.Result == 1 &&
                        authResponse.Response.SteamID == request.SteamId)
                    {
                        return Ok(new { success = true });
                    }
                }
            }

            return BadRequest(new { success = false });
        }

        [HttpPost("checkUser")]
        public async Task<IActionResult> CheckUserIsNewAsync([FromBody] CheckUserRequest request)
        {
            string steamId = request.SteamId;
            var user = await _context.UserInfo.FirstOrDefaultAsync(u => u._steamId == steamId);

            if (user == null)
            {
                // 신규 유저
                var newUser = new UserInfoTable
                {
                    _steamId = steamId,
                    _nickname = "",
                    _server = ""
                };

                _context.UserInfo.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok(new { IsNew = true });
            }
            else
            {
                // 기존 유저
                return Ok(new { IsNew = false, Nickname = user._nickname, Server = user._server });
            }
        }


        [HttpPost("saveUser")]
        public async Task<IActionResult> SaveUser([FromBody] UserInfoTable userInfo)
        {
            // 입력 데이터 검증
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            // 해당 steamId의 레코드에 server 정보 업데이트
            var user = await _context.UserInfo.FirstOrDefaultAsync(u => u._steamId == userInfo._steamId);
            if (user != null)
            {
                user._server = userInfo._server;
                user._nickname = userInfo._nickname;
                await _context.SaveChangesAsync();
                return Ok(new { success = true });
            }
            else
            {
                return BadRequest(new { success = false, message = "해당 사용자 정보가 존재하지 않습니다." });
            }
        }

    }
}
