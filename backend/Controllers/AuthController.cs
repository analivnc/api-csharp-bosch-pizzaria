using BoschPizza.Models; 
using BoschPizza.Services; // importa o TokenService (JWT)
using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Authorization; 
using BoschPizza.Data; // acesso ao banco (DbContext)
using System.Security.Cryptography; // usado para gerar hash
using System.Text; 

namespace BoschPizza.Controllers;

[ApiController]

// rota base: tudo começa com /auth
[Route("auth")]
public class AuthController : ControllerBase
{
    // serviço que gera token JWT
    private readonly TokenService _tokenService;

    // conexão com o banco
    private readonly AppDbContext _context;

    // pega configs do appsettings.json
    private readonly IConfiguration _configuration;

    // construtor (injeção de dependência)
    public AuthController(
        TokenService tokenService,
        AppDbContext context,
        IConfiguration configuration)
    {
        _tokenService = tokenService;
        _context = context;
        _configuration = configuration;
    }

    //  REGISTER (CADASTRAR USUÁRIO)
    [HttpPost("register")]
    public IActionResult Register([FromBody] UserLogin user)
    {
        // valida se veio vazio
        if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            return BadRequest(new { message = "Usuário e senha são obrigatórios" });

        // valida tamanho da senha
        if (user.Password.Length < 4)
            return BadRequest(new { message = "Senha deve ter no mínimo 4 caracteres" });

        // verifica se já existe no banco
        if (_context.UserLogins.Any(u => u.Username == user.Username))
            return BadRequest(new { message = "Usuário já existe" });

        //  transforma a senha em HASH (não salva senha real!)
        user.Password = HashPassword(user.Password);

        // salva no banco
        _context.UserLogins.Add(user);
        _context.SaveChanges();

        return Ok(new { message = "Usuário criado com sucesso" });
    }

    //  LOGIN
    [HttpPost("login")]

    // permite acessar sem estar logado
    [AllowAnonymous]
    public IActionResult Login([FromBody] UserLogin login)
    {
        // procura usuário no banco
        var user = _context.UserLogins
            .FirstOrDefault(u => u.Username == login.Username);

        // se não existir
        if (user == null)
            return NotFound(new { message = "Usuário não registrado" });

        // gera hash da senha digitada
        var hash = HashPassword(login.Password);

        // compara com o que está no banco
        if (user.Password != hash)
            return Unauthorized(new { message = "Senha incorreta" });

        // gera token JWT (login deu certo)
        var token = _tokenService.GenerateToken(
            user.Username,
            _configuration["Jwt:Key"],       // chave secreta
            _configuration["Jwt:Issuer"],    // quem emitiu
            _configuration["Jwt:Audience"]   // quem usa
        );

        // retorna token pro frontend
        return Ok(new { token });
    }

    //  FUNÇÃO DE HASH
    private string HashPassword(string password)
    {
        // cria algoritmo SHA256
        using var sha256 = SHA256.Create();

        // transforma a senha em bytes
        var bytes = Encoding.UTF8.GetBytes(password);

        // gera o hash (embaralha a senha)
        var hash = sha256.ComputeHash(bytes);

        // transforma em string pra salvar no banco
        return Convert.ToBase64String(hash);
    }
}