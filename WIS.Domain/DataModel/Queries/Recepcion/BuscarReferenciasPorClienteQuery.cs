using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

public class BuscarReferenciasPorClienteQuery : QueryObject<V_REC500_DETALLE_POR_REFERENCIA, WISDB>
{
    protected readonly string _searchValue;
    protected readonly string _cdCliente;
    protected readonly int _idEmpresa;

    public BuscarReferenciasPorClienteQuery(string searchValue)
    {
        _searchValue = searchValue;
    }
    public BuscarReferenciasPorClienteQuery(string searchValue, string cdCliente, int idEmpresa)
    {
        _searchValue = searchValue;
        _cdCliente = cdCliente;
        _idEmpresa = idEmpresa;
    }

    public override void BuildQuery(WISDB context)
    {
        this.Query = context.V_REC500_DETALLE_POR_REFERENCIA.AsNoTracking()
            .Where(r => r.CD_CLIENTE == _cdCliente && r.CD_EMPRESA == _idEmpresa && (r.NU_REFERENCIA.ToLower().Contains(_searchValue.ToLower()) || 
                         r.TP_REFERENCIA.ToLower().Contains(_searchValue.ToLower())))
            .OrderByDescending(r => r.NU_RECEPCION_REFERENCIA)
            .Take(50);
    }

    public virtual List<V_REC500_DETALLE_POR_REFERENCIA> GetResultados()
    {
        if (this.Query == null)
            throw new InvalidOperationException("La query no está inicializada");

        return this.Query.ToList();
    }
}


