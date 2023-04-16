using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace steamAuth.Models
{
    public class Player
    {
        public string SteamId { get; set; }
        public string PersonaName { get; set; }
    }

    public class Response
    {
        public List<Player> Players { get; set; }
    }

    public class ApiResponse
    {
        public Response Response { get; set; }
    }

    public class ValidateTicketRequest
    {
        [JsonProperty("steamId")]
        public string SteamId { get; set; }

        [JsonProperty("authTicket")]
        public string AuthTicket { get; set; }

        [JsonProperty("identity")]
        public string Identity { get; set; }
    }

    public class CheckUserRequest
    {
        public string SteamId { get; set; }
    }


    public class AuthResponse
    {
        public AuthResponseData Response { get; set; }
    }

    public class AuthResponseData
    {
        public int Result { get; set; }
        public string SteamID { get; set; }
    }

    // 실제 테이블과 명칭 맞춰야함
    [Table("user_info")]
    public class UserInfoTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int _id { get; set; }

        [Required]
        [MaxLength(255)]
        public string _steamId { get; set; }

        [MaxLength(255)]
        public string _nickname { get; set; }

        [MaxLength(255)]
        public string _server { get; set; }
    }

}
