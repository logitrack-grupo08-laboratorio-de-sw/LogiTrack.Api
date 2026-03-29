using Back.Application.Services;
using Back.Controllers;
using Back.Domain.Models;
using Back.Domain.Repositories;

namespace Back.UnitTests.Application;

public class AuthServiceTests
{
    [Fact]
    public async Task CP_Back_Login_UsuarioNoEncontrado_LanzaError()
    {
        var repo = new FakeUserRepository();
        var service = new AuthService(repo);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.Login(new LoginRequest { Email = "noexiste@mail.com", Password = "12345678" }));

        Assert.Contains("Usuario no encontrado", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CP_Back_Login_PasswordIncorrecta_LanzaError()
    {
        var repo = new FakeUserRepository
        {
            UserByEmail = new Operador(
                "Ana",
                "Lopez",
                "ana@mail.com",
                PasswordHasher.HashPassword("password-correcta"),
                "12345678")
        };
        var service = new AuthService(repo);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.Login(new LoginRequest { Email = "ana@mail.com", Password = "password-incorrecta" }));

        Assert.Contains("Contraseña incorrecta", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CP_Back_Login_Valido_RetornaTokenYDatosUsuario()
    {
        var repo = new FakeUserRepository
        {
            UserByEmail = new Supervisor(
                "Juan",
                "Perez",
                "juan@mail.com",
                PasswordHasher.HashPassword("12345678"),
                "87654321")
        };
        var service = new AuthService(repo);

        var result = await service.Login(new LoginRequest { Email = "juan@mail.com", Password = "12345678" });

        var resultType = result.GetType();
        var token = resultType.GetProperty("token")?.GetValue(result) as string;
        var user = resultType.GetProperty("user")?.GetValue(result);

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.NotNull(user);

        var userType = user!.GetType();
        var email = userType.GetProperty("email")?.GetValue(user) as string;
        var role = userType.GetProperty("role")?.GetValue(user) as string;

        Assert.Equal("juan@mail.com", email);
        Assert.Equal("supervisor", role);
    }

    [Fact]
    public async Task CP_Back_Registrarse_EmailDuplicado_LanzaError()
    {
        var repo = new FakeUserRepository
        {
            UserByEmail = new Operador("Ana", "Lopez", "ana@mail.com", PasswordHasher.HashPassword("12345678"), "12345678")
        };
        var service = new AuthService(repo);

        var request = new RegisterRequest
        {
            Nombre = "Ana",
            Apellido = "Lopez",
            Email = "ana@mail.com",
            Password = "12345678",
            DNI = "11111111",
            Role = "Operador",
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.Registrarse(request));

        Assert.Contains("correo electrónico ya está registrado", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CP_Back_Registrarse_Operador_CreaUsuarioHasheado()
    {
        var repo = new FakeUserRepository();
        var service = new AuthService(repo);

        var request = new RegisterRequest
        {
            Nombre = "Luis",
            Apellido = "Perez",
            Email = "luis@mail.com",
            Password = "12345678",
            DNI = "11223344",
            Role = "Operador",
        };

        await service.Registrarse(request);

        Assert.NotNull(repo.AddedUser);
        Assert.IsType<Operador>(repo.AddedUser);
        Assert.Equal("luis@mail.com", repo.AddedUser!.Email);
        Assert.NotEqual("12345678", repo.AddedUser.Password);
        Assert.True(PasswordHasher.VerifyPassword("12345678", repo.AddedUser.Password));
    }

    [Fact]
    public async Task CP_Back_Registrarse_RolTransportista_LanzaError()
    {
        var repo = new FakeUserRepository();
        var service = new AuthService(repo);

        var request = new RegisterRequest
        {
            Nombre = "Tra",
            Apellido = "Nsport",
            Email = "transportista@mail.com",
            Password = "12345678",
            DNI = "99887766",
            Role = "Transportista",
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.Registrarse(request));

        Assert.Contains("solo puede ser registrado por un Supervisor", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public Usuario? UserByEmail { get; set; }
        public Usuario? UserByDni { get; set; }
        public Usuario? UserById { get; set; }
        public Usuario? AddedUser { get; private set; }

        public Task Add(Usuario usuario)
        {
            AddedUser = usuario;
            return Task.CompletedTask;
        }

        public Task<List<Usuario>> GetAll() => Task.FromResult(new List<Usuario>());
        public Task<List<Operador>> GetOperadores() => Task.FromResult(new List<Operador>());
        public Task<List<Supervisor>> GetSupervisores() => Task.FromResult(new List<Supervisor>());
        public Task<List<Transportista>> GetTransportistas() => Task.FromResult(new List<Transportista>());
        public Task<Usuario?> GetUsuarioByDni(string dni) => Task.FromResult(UserByDni);
        public Task<Usuario?> GetUsuarioByEmail(string email) => Task.FromResult(UserByEmail);
        public Task<Usuario?> GetUsuarioById(Guid id) => Task.FromResult(UserById);
    }
}
