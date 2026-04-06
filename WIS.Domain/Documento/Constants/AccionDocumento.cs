namespace WIS.Domain.Documento.Constants
{
    public class AccionDocumento
    {
        public const string Cancelar = "CAN";
        public const string EnviarDocumento = "ENV";
        public const string AprobarDocumento = "APR";
        public const string IniciarVerificacion = "INIVER";
        public const string Finalizar = "FIN";
        public const string GenerarLineas = "GENLIN";
        public const string Agrupar = "AGR";
        public const string Desagrupar = "DESAGR";
        public const string FinalizarSinCierreAgenda = "FINSCA";
        public const string FinalizarConDiferencia = "FINCD";
        public const string Validar = "VAL";
        public const string Editar = "EDI";
        public const string ConfirmarEdicion = "CONEDI";
        public const string RetrocederLineasGeneradas = "RETWLG";
        public const string FinalizarOperacion = "FINOP";
        public const string IniciarOperacion = "INIOP";
        public const string TransferirSinPreparacion = "TRASP";

        public static bool ExcluirEnCambioEstado(string accion)
        {
            switch (accion)
            {
                case Agrupar:
                case Desagrupar:
                case FinalizarSinCierreAgenda:
                case FinalizarConDiferencia:
                case Validar:
                case FinalizarOperacion:
                case TransferirSinPreparacion:
                case IniciarOperacion:
                    return true;
                default:
                    return false;
            }
        }
    }
}
