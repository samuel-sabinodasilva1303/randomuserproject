namespace RandomUserProject.DTOs
{
    /// <summary>
    /// Autor: Samuel Sabino - 30/09/2025
    /// Descrição: class responsavel por representar os dados de um usuário.
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
