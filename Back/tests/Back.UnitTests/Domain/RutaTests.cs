using Back.Domain.Models;

namespace Back.UnitTests.Domain;

public class RutaTests
{
    private static Cliente BuildCliente(string nombre, string apellido)
    {
        return new Cliente(nombre, apellido, new Direccion("Calle 1", "CABA", "1000"));
    }

    private static Paquete BuildPaquete()
    {
        return new Paquete(
            10,
            20,
            30,
            BuildCliente("Ana", "Lopez"),
            BuildCliente("Luis", "Perez"),
            "Fragil");
    }

    private static Transportista BuildTransportista(string dni, string licencia = "LIC-1")
    {
        return new Transportista("Juan", "Gomez", $"{dni}@mail.com", "pwd", dni, licencia);
    }

    private static Vehiculo BuildVehiculo()
    {
        return new Vehiculo("AA123BB", "Iveco", 1000);
    }

    [Fact]
    public void CP_Back_Ruta_Iniciar_ActualizaEstadoYPaquetes()
    {
        var ruta = new Ruta(BuildTransportista("12345678"), BuildVehiculo());
        var paquete = BuildPaquete();
        ruta.AgregarPaquete(paquete);

        ruta.Iniciar();

        Assert.Equal(RutaStatus.EnCurso, ruta.Estado);
        Assert.Equal(PaqueteStatus.EnTransito, paquete.Status);
        Assert.NotNull(ruta.IniciadoEn);
    }

    [Fact]
    public void CP_Back_Ruta_Finalizar_ConPendientes_LanzaError()
    {
        var ruta = new Ruta(BuildTransportista("12345678"), BuildVehiculo());
        ruta.AgregarPaquete(BuildPaquete());
        ruta.Iniciar();

        var ex = Assert.Throws<InvalidOperationException>(() => ruta.Finalizar());

        Assert.Contains("pendientes", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CP_Back_Ruta_EntregarUltimoPaquete_FinalizaRuta()
    {
        var ruta = new Ruta(BuildTransportista("12345678"), BuildVehiculo());
        var paquete = BuildPaquete();
        ruta.AgregarPaquete(paquete);
        ruta.Iniciar();

        ruta.EntregarPaquete(paquete.Id);

        Assert.Equal(RutaStatus.Finalizada, ruta.Estado);
        Assert.NotNull(ruta.FinalizadoEn);
    }

    [Fact]
    public void CP_Back_Ruta_Reasignar_EnRutaCancelada_LanzaError()
    {
        var ruta = new Ruta(BuildTransportista("12345678"), BuildVehiculo());
        ruta.Cancelar("falla mecanica");

        var ex = Assert.Throws<InvalidOperationException>(() => ruta.ReasignarTransportista(BuildTransportista("87654321")));

        Assert.Contains("finalizada o cancelada", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
