using System;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Interfaces
{
    public class InterfazEjecucion
    {
        protected bool? _isSalida;

        public long Id { get; set; }                        //NU_INTERFAZ_EJECUCION
        public int? CdInterfazExterna { get; set; }         //CD_INTERFAZ_EXTERNA
        public string Archivo { get; set; }                 //NM_ARCHIVO
        public short? Situacion { get; set; }               //CD_SITUACAO
        public DateTime? Comienzo { get; set; }             //DT_COMIENZO
        public DateTime? FechaSituacion { get; set; }       //DT_SITUACAO
        public string ErrorCarga { get; set; }              //FL_ERROR_CARGA
        public string ErrorProcedimiento { get; set; }      //FL_ERROR_PROCEDIMIENTO
        public int? FuncionarioAceptacion { get; set; }     //CD_FUNCIONARIO_ACEPTACION
        public string Referencia { get; set; }              //DS_REFERENCIA
        public string NdSituacion { get; set; }             //ND_SITUACION
        public int? Empresa { get; set; }                   //CD_EMPRESA
        public string GrupoConsulta { get; set; }           //CD_GRUPO_CONSULTA
        public int? UserId { get; set; }                    //USERID
        public string Procesado { get; set; }               //ID_PROCESADO
        public string IdRequest { get; set; }               //ID_REQUEST
        public InterfazExterna InterfazExterna { get; set; }

        public bool IsAPI
        {
            get
            {
                return SituacionDb.IsAPI(Situacion);
            }
        }

        public bool IsSalida
        {
            set => _isSalida = value;
            get
            {
                string idEntradaSalida = null;
                if (InterfazExterna != null && InterfazExterna.Interfaz != null && !string.IsNullOrEmpty(InterfazExterna.Interfaz.IdEntradaSalida))
                {
                    idEntradaSalida = InterfazExterna.Interfaz.IdEntradaSalida;
                }

                _isSalida ??= (idEntradaSalida == "S" || CInterfazExterna.IsSalida(CdInterfazExterna.Value));
                return (bool)_isSalida;
            }
        }
    }
}
