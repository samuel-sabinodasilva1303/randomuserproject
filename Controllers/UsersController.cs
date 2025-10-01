using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RandomUserProject.DTOs;
using RandomUserProject.Models;
using RandomUserProject.Repositories;
using RandomUserProject.Services;

namespace RandomUserProject.Controllers
{
    /// <summary>
    /// Autor: Samuel Sabino - 30/09/205
    /// Descriçao: Class responsavel por receber e controllar as requisiçoes HTTP.
    /// </summary>
        
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRandomUserService _randomUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserRepository userRepository,
            IRandomUserService randomUserService,
            IMapper mapper,
            ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _randomUserService = randomUserService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<UserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResultDto<UserDto>>> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var (users, totalCount) = await _userRepository.GetPagedAsync(page, pageSize, searchTerm);
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            var result = new PagedResultDto<UserDto>
            {
                Data = userDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (await _userRepository.EmailExistsAsync(createUserDto.Email))
            {
                return BadRequest(new { message = "Email já cadastrado" });
            }

            var user = _mapper.Map<User>(createUserDto);
            var createdUser = await _userRepository.AddAsync(user);

            var userDto = _mapper.Map<UserDto>(createdUser);

            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, userDto);
        }


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            if (await _userRepository.EmailExistsAsync(updateUserDto.Email, id))
            {
                return BadRequest(new { message = "Email já cadastrado para outro usuário" });
            }

            _mapper.Map(updateUserDto, existingUser);
            await _userRepository.UpdateAsync(existingUser);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            await _userRepository.DeleteAsync(user);
            return NoContent();
        }

        [HttpPost("delete-multiple")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteMultipleUsers([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest(new { message = "Nenhum usuário selecionado para exclusão" });
            }

            if (ids.Count > 100)
            {
                return BadRequest(new { message = "Não é possível excluir mais de 100 usuários por vez" });
            }

            var deletedCount = 0;
            var notFoundIds = new List<int>();

            try
            {
                foreach (var id in ids)
                {
                    var user = await _userRepository.GetByIdAsync(id);
                    if (user != null)
                    {
                        await _userRepository.DeleteAsync(user);
                        deletedCount++;
                    }
                    else
                    {
                        notFoundIds.Add(id);
                    }
                }

                _logger.LogInformation("Excluídos {Count} usuários", deletedCount);

                return Ok(new
                {
                    success = true,
                    message = $"{deletedCount} usuários excluídos com sucesso",
                    deletedCount = deletedCount,
                    notFoundCount = notFoundIds.Count,
                    notFoundIds = notFoundIds
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplos usuários");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro ao excluir usuários",
                    error = ex.Message,
                    deletedCount = deletedCount
                });
            }
        }

        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddUsers([FromQuery] int count = 10)
        {
            if (count < 1 || count > 100)
            {
                return BadRequest(new { message = "A quantidade deve estar entre 1 e 100" });
            }

            var initialCount = await _userRepository.CountAsync();
            //A api estava travando - gargalo necessario
            var batchSize = 20;
            var totalGenerated = 0;
            var batches = new List<int>();

            try
            {
                while (totalGenerated < count)
                {
                    var currentBatch = Math.Min(batchSize, count - totalGenerated);
                    var newUsers = await _randomUserService.GetRandomUsersAsync(currentBatch);

                    await _userRepository.AddRangeAsync(newUsers);

                    totalGenerated += newUsers.Count;
                    batches.Add(newUsers.Count);

                    _logger.LogInformation("Adicionado lote de {Count} usuários. Total: {Total}/{Target}",
                        newUsers.Count, totalGenerated, count);

                    if (totalGenerated < count)
                    {
                        //A api estava travando - gargalo necessario
                        await Task.Delay(500);
                    }
                }

                var finalCount = await _userRepository.CountAsync();

                _logger.LogInformation("Operação concluída. {Total} usuários adicionados", totalGenerated);

                return Ok(new
                {
                    success = true,
                    message = $"{totalGenerated} usuários adicionados com sucesso!",
                    added = totalGenerated,
                    previousTotal = initialCount,
                    currentTotal = finalCount,
                    batches = batches.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar usuário(s)");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro ao adicionar usuário(s)",
                    error = ex.Message,
                    added = totalGenerated
                });
            }
        }

        /// <summary>
        /// Obtém estatísticas do banco de dados
        /// </summary>
        /// <returns>Estatísticas</returns>
        [HttpGet("stats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetStats()
        {
            var totalUsers = await _userRepository.CountAsync();

            return Ok(new
            {
                //Validar**
                totalUsers = totalUsers,
                isEmpty = totalUsers == 0
            });
        }
    }
}
