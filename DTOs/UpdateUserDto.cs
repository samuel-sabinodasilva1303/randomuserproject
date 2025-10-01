using System.ComponentModel.DataAnnotations;

namespace RandomUserProject.DTOs
{
    /// <summary>
    /// Autor: Samuel Sabino - 30/09/2025
    /// Descrição: Class responsavel por atualizar os dados de um usuário.
    /// </summary>

    public class UpdateUserDto
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O sobrenome é obrigatório")]
        [StringLength(100, ErrorMessage = "O sobrenome deve ter no máximo 100 caracteres")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(200, ErrorMessage = "O email deve ter no máximo 200 caracteres")]
        public string Email { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres")]
        public string Street { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres")]
        public string City { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "O estado deve ter no máximo 100 caracteres")]
        public string State { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "O CEP deve ter no máximo 20 caracteres")]
        public string PostalCode { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "O país deve ter no máximo 100 caracteres")]
        public string Country { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        [StringLength(10, ErrorMessage = "O gênero deve ter no máximo 10 caracteres")]
        public string Gender { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "A URL da foto deve ter no máximo 500 caracteres")]
        public string PictureUrl { get; set; } = string.Empty;
    }
}
