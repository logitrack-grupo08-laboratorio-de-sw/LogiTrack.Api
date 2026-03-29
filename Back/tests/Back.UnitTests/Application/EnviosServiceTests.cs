using Back.Application.Services;
using Back.Controllers;
using Back.Domain.Models;
using Back.Domain.Repositories;

namespace Back.UnitTests.Application;

public class EnviosServiceTests
{
    [Fact]
    public async Task CP_Back_RegistrarPaquete_AgregaEnRepositorio()
    {
        var enviosRepo = new FakeEnviosRepository();
        var rutasRepo = new FakeRutasRepository();
        var usersRepo = new FakeUserRepository();
        var service = new EnviosService(enviosRepo, usersRepo, rutasRepo);

        var request = new RegistrarPaqueteRequest
        {
            Peso = 12,
            Comentarios = "Test",
            Remitente = new RegistrarClienteRequest
            {
                Nombre = "Ana",
                Apellido = "Lopez",
                Direccion = "Calle 1",
                Localidad = "CABA",
                CP = "1000"
            },
            Destinatario = new RegistrarClienteRequest
            {
                Nombre = "Luis",
                Apellido = "Perez",
                Direccion = "Calle 2",
                Localidad = "La Plata",
                CP = "1900"
            }
        };

        await service.RegistrarPaquete(request);

        Assert.Single(enviosRepo.AddedPaquetes);
        Assert.Equal("Ana", enviosRepo.AddedPaquetes[0].Remitente.Nombre);
        Assert.Equal("Luis", enviosRepo.AddedPaquetes[0].Destinatario.Nombre);
    }

    [Fact]
    public async Task CP_Back_ReasignarRuta_SinRuta_LanzaError()
    {
        var enviosRepo = new FakeEnviosRepository();
        var rutasRepo = new FakeRutasRepository { RutaToReturn = null };
        var usersRepo = new FakeUserRepository();
        var service = new EnviosService(enviosRepo, usersRepo, rutasRepo);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ReasignarRuta(Guid.NewGuid(), Guid.NewGuid()));

        Assert.Contains("Ruta no encontrada", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CP_Back_ReasignarRuta_TransportistaSuspendido_LanzaError()
    {
        var enviosRepo = new FakeEnviosRepository();
        var rutasRepo = new FakeRutasRepository
        {
            RutaToReturn = new Ruta(
                new Transportista("Ana", "Lopez", "ana@mail.com", "pwd", "12345678", "LIC-1"),
                new Vehiculo("AA123BB", "Iveco", 1000))
        };

        var suspendido = new Transportista("Luis", "Perez", "luis@mail.com", "pwd", "87654321", "LIC-2");
        suspendido.CambiarEstado(Transportista.EstadoTransportista.Suspendido);

        var usersRepo = new FakeUserRepository { UserById = suspendido };
        var service = new EnviosService(enviosRepo, usersRepo, rutasRepo);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ReasignarRuta(Guid.NewGuid(), suspendido.Id));

        Assert.Contains("suspendido", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CP_Back_ReasignarRuta_TransportistaActivo_ReasignaYGuarda()
    {
        var enviosRepo = new FakeEnviosRepository();
        var ruta = new Ruta(
            new Transportista("Ana", "Lopez", "ana@mail.com", "pwd", "12345678", "LIC-1"),
            new Vehiculo("AA123BB", "Iveco", 1000));
        var rutasRepo = new FakeRutasRepository { RutaToReturn = ruta };

        var activo = new Transportista("Luis", "Perez", "luis@mail.com", "pwd", "87654321", "LIC-2");
        var usersRepo = new FakeUserRepository { UserById = activo };

        var service = new EnviosService(enviosRepo, usersRepo, rutasRepo);

        await service.ReasignarRuta(Guid.NewGuid(), activo.Id);

        Assert.Equal(activo.Id, ruta.Transportista.Id);
        Assert.True(rutasRepo.AddCalled);
    }

    private sealed class FakeEnviosRepository : IEnviosRepository
    {
        public List<Paquete> AddedPaquetes { get; } = [];

        public Task Add(Paquete envio)
        {
            AddedPaquetes.Add(envio);
            return Task.CompletedTask;
        }

        public Task Add(Sucursal sucursal) => Task.CompletedTask;
        public Task<List<Paquete>> GetAll() => Task.FromResult(new List<Paquete>());
        public Task<Paquete?> GetPaquete(Guid id) => Task.FromResult<Paquete?>(null);
        public Task<Paquete?> GetPaqueteByCodigoSeguimiento(string codigoSeguimiento) => Task.FromResult<Paquete?>(null);
        public Task<List<Paquete>> GetPaquetes(string? codigoSeguimiento, string? destinatario) => Task.FromResult(new List<Paquete>());
        public Task<List<Paquete>> GetPaquetesByIds(List<Guid> paqueteIds) => Task.FromResult(new List<Paquete>());
        public Task<List<Paquete>> GetPaquetesEnSucursal() => Task.FromResult(new List<Paquete>());
        public Task<List<Sucursal>> GetSucursales() => Task.FromResult(new List<Sucursal>());
    }

    private sealed class FakeRutasRepository : IRutasRepository
    {
        public Ruta? RutaToReturn { get; set; }
        public bool AddCalled { get; private set; }

        public Task Add(Ruta ruta)
        {
            AddCalled = true;
            return Task.CompletedTask;
        }

        public Task<Ruta?> GetRutaById(Guid id) => Task.FromResult(RutaToReturn);
        public Task<List<Ruta>> GetHistorialRutas(Guid transportista) => Task.FromResult(new List<Ruta>());
        public Task<List<Ruta>> GetMisRutasSupervisadas(Guid supervisor) => Task.FromResult(new List<Ruta>());
        public Task<List<Ruta>> GetRutas() => Task.FromResult(new List<Ruta>());
        public Task<bool> IsVehiculoEnRuta(Guid vehiculoId) => Task.FromResult(false);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public Usuario? UserById { get; set; }

        public Task Add(Usuario usuario) => Task.CompletedTask;
        public Task<List<Usuario>> GetAll() => Task.FromResult(new List<Usuario>());
        public Task<List<Operador>> GetOperadores() => Task.FromResult(new List<Operador>());
        public Task<List<Supervisor>> GetSupervisores() => Task.FromResult(new List<Supervisor>());
        public Task<List<Transportista>> GetTransportistas() => Task.FromResult(new List<Transportista>());
        public Task<Usuario?> GetUsuarioByDni(string dni) => Task.FromResult<Usuario?>(null);
        public Task<Usuario?> GetUsuarioByEmail(string email) => Task.FromResult<Usuario?>(null);
        public Task<Usuario?> GetUsuarioById(Guid id) => Task.FromResult(UserById);
    }
}
