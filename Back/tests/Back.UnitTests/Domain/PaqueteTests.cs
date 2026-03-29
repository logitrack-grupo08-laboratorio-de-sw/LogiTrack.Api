using Back.Domain.Models;

namespace Back.UnitTests.Domain;

public class PaqueteTests
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

    [Fact]
    public void CP_Back_Paquete_EnTransito_DesdeEnSucursal_CambiaEstado()
    {
        var paquete = BuildPaquete();

        paquete.EnTransito();

        Assert.Equal(PaqueteStatus.EnTransito, paquete.Status);
    }

    [Fact]
    public void CP_Back_Paquete_Entregar_DesdeEnTransito_CambiaEstado()
    {
        var paquete = BuildPaquete();
        paquete.EnTransito();

        paquete.Entregar();

        Assert.Equal(PaqueteStatus.Entregado, paquete.Status);
    }

    [Fact]
    public void CP_Back_Paquete_Entregar_DesdeEnSucursal_LanzaError()
    {
        var paquete = BuildPaquete();

        var ex = Assert.Throws<InvalidOperationException>(() => paquete.Entregar());

        Assert.Contains("tránsito", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CP_Back_Paquete_Cancelar_Entregado_LanzaError()
    {
        var paquete = BuildPaquete();
        paquete.EnTransito();
        paquete.Entregar();

        var ex = Assert.Throws<InvalidOperationException>(() => paquete.Cancelar("cliente ausente"));

        Assert.Contains("entregado", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CP_Back_Paquete_Reenviar_Cancelado_VuelveASucursalYLimpiaRazon()
    {
        var paquete = BuildPaquete();
        paquete.Cancelar("direccion incorrecta");

        paquete.ReEnviar();

        Assert.Equal(PaqueteStatus.EnSucursal, paquete.Status);
        Assert.Null(paquete.RazonCancelacion);
    }
}
