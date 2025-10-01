using System.Text.Json;
using RandomUserProject.Models;

namespace RandomUserProject.Services
{
    /// <summary>
    /// Autor: Samuel Sabino - 29/09/2025
    /// Descrição: Serviço responsável por consumir a API Random User, 
    /// obter dados de usuários aleatórios e convertê-los para a entidade User.
    /// </summary>
    public class RandomUserService : IRandomUserService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RandomUserService> _logger;

        public RandomUserService(HttpClient httpClient, ILogger<RandomUserService> logger)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://randomuser.me/api/");
            _logger = logger;
        }

        public async Task<List<User>> GetRandomUsersAsync(int count = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync($"?results={count}&nat=us,br,gb");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var randomUserData = JsonSerializer.Deserialize<RandomUserResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return randomUserData?.Results?.Select(r => new User
                {
                    FirstName = r.Name.First,
                    LastName = r.Name.Last,
                    Email = r.Email,
                    Phone = r.Phone,
                    Street = $"{r.Location.Street.Number} {r.Location.Street.Name}",
                    City = r.Location.City,
                    State = r.Location.State,
                    PostalCode = r.Location.Postcode.ToString(),
                    Country = r.Location.Country,
                    DateOfBirth = DateTime.Parse(r.Dob.Date).ToUniversalTime(),
                    Gender = r.Gender,
                    PictureUrl = r.Picture.Large
                }).ToList() ?? new List<User>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuários da API Random User");
                throw;
            }
        }

        private class RandomUserResponse
        {
            public List<RandomUser> Results { get; set; } = new();
        }

        private class RandomUser
        {
            public Name Name { get; set; } = new();
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public Location Location { get; set; } = new();
            public Dob Dob { get; set; } = new();
            public string Gender { get; set; } = string.Empty;
            public Picture Picture { get; set; } = new();
        }

        private class Name
        {
            public string First { get; set; } = string.Empty;
            public string Last { get; set; } = string.Empty;
        }

        private class Location
        {
            public Street Street { get; set; } = new();
            public string City { get; set; } = string.Empty;
            public string State { get; set; } = string.Empty;
            public object Postcode { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
        }

        private class Street
        {
            public int Number { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private class Dob
        {
            public string Date { get; set; } = string.Empty;
        }

        private class Picture
        {
            public string Large { get; set; } = string.Empty;
        }
    }
}