
using AloPrefeitoP.Models;
using FacilityCareApp.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AloPrefeitoP.Services
{
    public class ApiServices
    {
        private readonly HttpClient _httpClient;
        private static string _baseUrl = "https://apiagente.iaintelligence.com.br/";
      //  private static string _baseUrl = "https://wt8c4018-7117.brs.devtunnels.ms/"; //teste

        private readonly ILogger<ApiServices> _logger;
        JsonSerializerOptions _serializerOptions;

        public ApiServices(HttpClient httpClient, ILogger<ApiServices> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiResponse<bool>> Login(string email, string password)
        {
            try
            {
               // _httpClient.DefaultRequestHeaders.Add("X-CORS-APP-IACARE", "iacare");
                var login = new Usuario() { VusuDsEmail = email, VusuDsSenha = password };

                var json = JsonSerializer.Serialize(login, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await PostRequest("api/Usuarios/Login", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao enviar requisição HTTP :{response.StatusCode}");
                    return new ApiResponse<bool> { ErrorMessage = $"Erro ao enviar requisição HTTP :{response.StatusCode}" };
                }
                //Ler o conteúdo da resposta HTTP como uma string de forma assíncrona.
                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

                //Armazena dados do token
                Preferences.Set("accesstoken", result.Accesstoken ?? "");
                Preferences.Set("usuarioid", result.UsuarioId ?? 0);
                Preferences.Set("usuarionome", result.UsuarioNome ?? "");
                Preferences.Set("usuarioemail", result.UsuarioEmail ?? "");
                Preferences.Set("tokenexpiration", result.Expiration ?? DateTime.MinValue);
                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro no login : {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }




        private async Task<HttpResponseMessage> PostRequest(string uri, HttpContent content)
        {
            var enderecoUrl = _baseUrl + uri;
            try
            {
               // AddAuthorizationHeader();
                var result = await _httpClient.PostAsync(enderecoUrl, content);
                return result;
            }
            catch (Exception ex)
            {
                // Log o erro ou trate conforme necessário
                _logger.LogError($"Erro ao enviar requisição POST para {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }


        public async Task<string> GetRespostaAgentContexto(string mensagem, int id)
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.None)
                return string.Empty;

            var msg = Uri.EscapeDataString(mensagem ?? "");
            var url = $"{_baseUrl}api/SupporteAloPreito/AloPrefeito?Mensagem={msg}&UserID={id}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return string.Empty;

            return await response.Content.ReadAsStringAsync();
        }


        private void AddAuthorizationHeader()
        {
            var token = Preferences.Get("accesstoken", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
               // _httpClient.DefaultRequestHeaders.Add("X-CORS-APP-IACARE", "iacare");
            }
        }



    }
}
