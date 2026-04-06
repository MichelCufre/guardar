using System;
using System.Collections.Generic;
using System.Linq;

namespace WIS.Domain.Impresiones.Utils
{
    public class ImpresionUtils
    {
        public virtual EtiquetaCampos RetornarCamposRecortados(string linea)
        {
            var etiquetaCropped = new EtiquetaCampos();

            if (!string.IsNullOrEmpty(linea))
            {
                int length_comando_wis = EtiquetaComando.WisInicio.Length;
                int length_comando_campo = EtiquetaComando.CampoInicio.Length;
                int length_comando_largo = EtiquetaComando.LargoInicio.Length;
                int length_comando_texto = EtiquetaComando.TextoInicio.Length;

                int comienzoEti = -1;
                int finEti = 0;

                comienzoEti = linea.IndexOf(EtiquetaComando.WisInicio, 0);
                if (comienzoEti > -1)
                {
                    finEti = linea.IndexOf(EtiquetaComando.WisFinal, 0);

                    //contenido entre <WIS></WIS>
                    etiquetaCropped.ContenidoEtiquetaWis = linea.Substring(comienzoEti, (finEti - comienzoEti) + (length_comando_wis + 1));

                    //Busqueda contenido de <CAMPO></CAMPO>
                    do
                    {
                        comienzoEti = etiquetaCropped.ContenidoEtiquetaWis.IndexOf(EtiquetaComando.CampoInicio, 0);
                        if (comienzoEti > 0)
                        {
                            finEti = etiquetaCropped.ContenidoEtiquetaWis.IndexOf(EtiquetaComando.CampoFinal, 0);

                            //contenido entre <CAMPO></CAMPO>
                            etiquetaCropped.ContenidoEtiquetaCampo = etiquetaCropped.ContenidoEtiquetaWis.Substring(comienzoEti, ((finEti - comienzoEti) + (length_comando_campo + 1)));

                            //quito <CAMPO></CAMPO>
                            var campoReemplazado = etiquetaCropped.ContenidoEtiquetaCampo.Replace(EtiquetaComando.CampoInicio, null)
                                .Replace(EtiquetaComando.CampoFinal, null);

                            etiquetaCropped.Campos.Add(campoReemplazado);

                            //remuevo lo cortado
                            etiquetaCropped.ContenidoEtiquetaWis = etiquetaCropped.ContenidoEtiquetaWis.Remove(comienzoEti, ((finEti - comienzoEti) + (length_comando_campo + 1)));


                        }
                        else
                        {
                            //TODO: REVISAR
                            etiquetaCropped.Campos = new List<string>();
                        }
                    }
                    while (etiquetaCropped.ContenidoEtiquetaWis.IndexOf(EtiquetaComando.CampoInicio, 0) > 0);


                    //Busqueda contenido de <LARGO></LARGO>
                    do
                    {
                        comienzoEti = etiquetaCropped.ContenidoEtiquetaWis.IndexOf(EtiquetaComando.LargoInicio, 0);
                        if (comienzoEti > 0)
                        {
                            finEti = etiquetaCropped.ContenidoEtiquetaWis.IndexOf(EtiquetaComando.LargoFinal, 0);

                            //contenido entre <LARGO></LARGO>
                            etiquetaCropped.ContenidoEtiquetaLargo = etiquetaCropped.ContenidoEtiquetaWis.Substring(comienzoEti, ((finEti - comienzoEti) + (length_comando_largo + 1)));

                            //quito <LARGO></LARGO>
                            var largoReemplazado = etiquetaCropped.ContenidoEtiquetaLargo.Replace(EtiquetaComando.LargoInicio, null)
                                .Replace(EtiquetaComando.LargoFinal, null);

                            etiquetaCropped.Largos.Add(largoReemplazado);

                            //remuevo lo cortado
                            etiquetaCropped.ContenidoEtiquetaWis = etiquetaCropped.ContenidoEtiquetaWis.Remove(comienzoEti, ((finEti - comienzoEti) + (length_comando_largo + 1)));


                        }
                        else
                        {
                            //TODO: REVISAR
                            etiquetaCropped.Largos = new List<string>();
                        }
                    }
                    while (etiquetaCropped.ContenidoEtiquetaWis.IndexOf(EtiquetaComando.LargoInicio, 0) > 0);


                    //Busqueda contenido de <TEXTO></TEXTO>
                    do
                    {
                        comienzoEti = etiquetaCropped.ContenidoEtiquetaWis.IndexOf(EtiquetaComando.TextoInicio, 0);
                        if (comienzoEti > 0)
                        {
                            finEti = etiquetaCropped.ContenidoEtiquetaWis.IndexOf(EtiquetaComando.TextoFinal, 0);

                            //contenido entre <TEXTO></TEXTO>
                            etiquetaCropped.ContenidoEtiquetaTexto = etiquetaCropped.ContenidoEtiquetaWis.Substring(comienzoEti, ((finEti - comienzoEti) + (length_comando_texto + 1)));

                            //quito <TEXTO></TEXTO>
                            var textoReemplazado = etiquetaCropped.ContenidoEtiquetaTexto.Replace(EtiquetaComando.TextoInicio, null)
                                .Replace(EtiquetaComando.TextoFinal, null);

                            etiquetaCropped.Textos.Add(textoReemplazado);

                            //remuevo lo cortado
                            etiquetaCropped.ContenidoEtiquetaWis = etiquetaCropped.ContenidoEtiquetaWis.Remove(comienzoEti, ((finEti - comienzoEti) + (length_comando_texto + 1)));


                        }
                        else
                        {
                            //TODO: REVISAR
                            etiquetaCropped.Textos = new List<string>();
                        }
                    }
                    while (etiquetaCropped.ContenidoEtiquetaWis.IndexOf(EtiquetaComando.TextoInicio, 0) > 0);

                }
            }
            else
            {
                return null;
            }

            return etiquetaCropped;
        }

        public virtual string LimpiarComandos(string linea, string comandoInicial, string comandoFinal)
        {
            string nuevoCampo = "";

            int comienzo = -1;
            comienzo = linea.IndexOf(comandoInicial, 0);

            if (comienzo > -1)
            {
                nuevoCampo = linea.Replace(comandoInicial, null)
                    .Replace(comandoFinal, null);

                return nuevoCampo;
            }

            return linea;
        }

        public virtual string InsertarContenidoLineas(string linea, string dato, string etiInicial, string etiFinal)
        {
            int comienzo = -1;
            int fin = 0;

            comienzo = linea.IndexOf(etiInicial, 0);
            if (comienzo > -1)
            {
                fin = linea.IndexOf(etiFinal, 0);
                linea = linea.Remove(comienzo, (fin - comienzo) + (etiFinal.Length));
                linea = linea.Insert(comienzo, dato);
            }
            return linea;
        }

        public virtual List<string> GetLineasTemplate(string dato, string delimitador)
        {
            if (!string.IsNullOrEmpty(dato))
            {
                List<string> colLineasCuerpo = dato.Split(new[] { delimitador }, StringSplitOptions.RemoveEmptyEntries).ToList();
                return colLineasCuerpo;
            }
            return null;
        }

        public virtual List<string> ManejoEtiquetasContenido(string contenidoTemplate)
        {
            List<string> templateProcesado = new List<string>();

            List<string> ContenidoTemplateDelineado = GetLineasTemplate(contenidoTemplate, "\n");

            foreach (var lineasTemplate in ContenidoTemplateDelineado)
            {
                string lineaAgregar = lineasTemplate;

                EtiquetaCampos lineaRecortada = RetornarCamposRecortados(lineasTemplate);

                if (lineaRecortada.Textos.Count > 0)
                {

                    foreach (string texto in lineaRecortada.Textos)
                    {
                        lineaAgregar = InsertarContenidoLineas(lineasTemplate, texto, EtiquetaComando.TextoInicio, EtiquetaComando.TextoFinal);

                        templateProcesado.Add("\n" + lineaAgregar);
                    }

                    lineaAgregar = string.Empty;
                }

                if (lineaRecortada.Campos.Count > 0)
                {
                    foreach (string campo in lineaRecortada.Campos)
                    {

                        lineaAgregar = InsertarContenidoLineas(lineasTemplate, campo, EtiquetaComando.CampoInicio, EtiquetaComando.CampoFinal);

                        templateProcesado.Add("\n" + lineaAgregar);
                    }

                    lineaAgregar = string.Empty;
                }

                if (lineaRecortada.Largos.Count > 0)
                {

                    foreach (string texto in lineaRecortada.Largos)
                    {
                        lineaAgregar = InsertarContenidoLineas(lineasTemplate, texto, EtiquetaComando.LargoInicio, EtiquetaComando.LargoFinal);

                        templateProcesado.Add("\n" + lineaAgregar);
                    }

                    lineaAgregar = string.Empty;
                }

                if (!string.IsNullOrEmpty(lineaAgregar))
                    templateProcesado.Add("\n" + lineaAgregar);

            }

            return templateProcesado;
        }
    }
}
