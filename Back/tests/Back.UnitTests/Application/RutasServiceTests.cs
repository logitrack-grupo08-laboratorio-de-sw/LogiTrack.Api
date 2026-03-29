using Back.Application.Services;
using Back.Controllers;
using Back.Domain.Models;
using Back.Domain.Repositories;

namespace Back.UnitTests.Application;

public class RutasServiceTests
{
    [Fact]
    public async Task CP_Back_CrearRuta_SinPaquetes_LanzaError()
    {
        var service = BuildService();

        var request = new CrearRutaRequest
        {
            VehiculoId = Guid.NewGuid(),
            TransportistaId = Guid.NewGuid(),
            PaqueteIds = new List<Guid>(),
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CrearRuta(request));

        Assert.Contains("sin paquetes", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CP_Back_CrearRuta_VehiculoNoEncontrado_LanzaError()
    {
        var service = BuildService();

        var request = new CrearRutaRequest
        {
            VehiculoId = Guid.NewGuid(),
            TransportistaId = Guid.NewGuid(),
            PaqueteIds = new List<Guid> { Guid.NewGuid() },
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CrearRuta(request));

        Assert.Contains("Vehiculo no encontrado", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CP_Back_CrearRuta_VehiculoYaAsignado_LanzaError()
    {
        var enviosRepo = new FakeEnviosRepository();
        var vehiculoRepo = new FakeVehiculoRepository { VehiculoById = new Vehiculo("AA123BB", "Iveco", 1000) };
        var rutasRepo = new FakeRutasRepository { IsVehiculoEnRutaResult = true };
        var userRepo = new FakeUserRepository();
        var service = new RutasService(enviosRepo, vehiculoRepo, rutasRepo, userRepo);

        var request = new CrearRutaRequest
        {
            VehiculoId = Guid.NewGuid(),
            TransportistaId = Guid.NewGuid(),
            PaqueteIds = new List<Guid> { Guid.NewGuid() },
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CrearRuta(request));

        Assert.Contains("ya está asignado", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CP_Back_CrearRuta_TransportistaNoDisponible_LanzaError()
    {
        var enviosRepo = new FakeEnviosRepository
        {
            PaquetesByIdsResult = new List<Paquete> { BuildPaquete() }
        };
        var vehiculoRepo = new FakeVehiculoRepository { VehiculoById = new Vehiculo("AA123BB", "Iveco", 1000) };
        var rutasRepo = new FakeRutasRepository();

        var transportistaSuspendido = new Transportista("Ana", "Lopez", "ana@mail.com", "pwd", "12345678", "LIC-1");
        transportistaSuspendido.CambiarEstado(Transportista.EstadoTransportista.Suspendido);

        var userRepo = new FakeUserRepository { UserById = transportistaSuspendido };

        var service = new RutasService(enviosRepo, vehiculoRepo, rutasRepo, userRepo);

        var request = new CrearRutaRequest
        {
            VehiculoId = Guid.NewGuid(),
            TransportistaId = transportistaSuspendido.Id,
            PaqueteIds = new List<Guid> { Guid.NewGuid() },
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CrearRuta(request));

        Assert.Contains("suspendido o inhabilitado", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CP_Back_CrearRuta_Valida_CreaYGuardaRuta()
    {
        var paquete = BuildPaquete();
        var enviosRepo = new FakeEnviosRepository
        {
            PaquetesByIdsResult = new List<Paquete> { paquete }
        };

        var vehiculo = new Vehiculo("AB123CD", "Mercedes", 1200);
        var vehiculoRepo = new FakeVehiculoRepository { VehiculoById = vehiculo };
        var rutasRepo = new FakeRutasRepository();

        var transportista = new Transportista("Luis", "Perez", "luis@mail.com", "pwd", "87654321", "LIC-2");
        var userRepo = new FakeUserRepository { UserById = transportista };

        var service = new RutasService(enviosRepo, vehiculoRepo, rutasRepo, userRepo);

        var request = new CrearRutaRequest
        {
            VehiculoId = vehiculo.Id,
            TransportistaId = transportista.Id,
            PaqueteIds = new List<Guid> { paquete.Id },
        };

        await service.CrearRuta(request);

        Assert.True(rutasRepo.AddCalled);
        Assert.NotNull(rutasRepo.AddedRuta);
        Assert.Equal(transportista.Id, rutasRepo.AddedRuta!.Transportista.Id);
        Assert.Equal(vehiculo.Id, rutasRepo.AddedRuta.Vehiculo.Id);
        Assert.Single(rutasRepo.AddedRuta.Paquetes);
    }

    private static RutasService BuildService()
    {
        return new RutasService(
            new FakeEnviosRepository(),
            new FakeVehiculoRepository(),
            new FakeRutasRepository(),
            new FakeUserRepository());
    }

    private static Paquete BuildPaquete()
    {
        return new Paquete(
            10,
            20,
            30,
            new Cliente("Ana", "Lopez", new Direccion("Calle 1", "CABA", "1000")),
            new Cliente("Luis", "Perez", new Direccion("Calle 2", "La Plata", "1900")),
            "Fragil");
    }

    private sealed class FakeEnviosRepository : IEnviosRepository
    {
        public List<Paquete> PaquetesByIdsResult { get; set; } = new();

        public Task Add(Paquete envio) => Task.CompletedTask;
        public Task Add(Sucursal sucursal) => Task.CompletedTask;
        public Task<List<Paquete>> GetAll() => Task.FromResult(new List<Paquete>());
        public Task<Paquete?> GetPaquete(Guid id) => Task.FromResult<Paquete?>(null);
        public Task<Paquete?> GetPaqueteByCodigoSeguimiento(string codigoSeguimiento) => Task.FromResult<Paquete?>(null);
        public Task<List<Paquete>> GetPaquetes(string? codigoSeguimiento, string? destinatario) => Task.FromResult(new List<Paquete>());
        public Task<List<Paquete>> GetPaquetesByIds(List<Guid> paqueteIds) => Task.FromResult(PaquetesByIdsResult);
        public Task<List<Paquete>> GetPaquetesEnSucursal() => Task.FromResult(new List<Paquete>());
        public Task<List<Sucursal>> GetSucursales() => Task.FromResult(new List<Sucursal>());
    }

    private sealed class FakeVehiculoRepository : IVehiculoRepository
    {
        public Vehiculo? VehiculoById { get; set; }

        public Task Add(Vehiculo vehiculo) => Task.CompletedTask;
        public Task<List<Vehiculo>> GetAll() => Task.FromResult(new List<Vehiculo>());
        public Task<Vehiculo?> GetVehiculo(Guid id) => Task.FromResult(VehiculoById);
        public Task<List<Vehiculo>> GetVehiculosActivos() => Task.FromResult(new List<Vehiculo>());
    }

    private sealed class FakeRutasRepository : IRutasRepository
    {
        public bool IsVehiculoEnRutaResult { get; set; }
        public bool AddCalled { get; private set; }
        public Ruta? AddedRuta { get; private set; }

        public Task Add(Ruta ruta)
        {
            AddCalled = true;
            AddedRuta = ruta;
            return Task.CompletedTask;
        }

        public Task<Ruta?> GetRutaById(Guid id) => Task.FromResult<Ruta?>(null);
        public Task<List<Ruta>> GetHistorialRutas(Guid transportista) => Task.FromResult(new List<Ruta>());
        public Task<List<Ruta>> GetMisRutasSupervisadas(Guid supervisor) => Task.FromResult(new List<Ruta>());
        public Task<List<Ruta>> GetRutas() => Task.FromResult(new List<Ruta>());
        public Task<bool> IsVehiculoEnRuta(Guid vehiculoId) => Task.FromResult(IsVehiculoEnRutaResult);
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
