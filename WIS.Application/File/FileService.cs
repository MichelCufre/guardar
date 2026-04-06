using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.File.Execution;

namespace WIS.Application.File
{
    public class FileService : IFileService
    {
        protected readonly IEjecucionService _ejecucionService;
        protected readonly IFacturacionService _facturacionService;
        protected readonly IOptions<FileSettings> _configuration;
        protected readonly IUnitOfWorkFactory _uowFactory;

        public FileService(IEjecucionService ejecucionService, IFacturacionService facturacionService, IOptions<FileSettings> configuration, IUnitOfWorkFactory uowFactory)
        {
            this._ejecucionService = ejecucionService;
            this._facturacionService = facturacionService;
            this._configuration = configuration;
            this._uowFactory = uowFactory;
        }

        public virtual async Task<FileDownloadResponse> GetFile(int user, FileDownloadRequest request)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var isAdjuntoEventoWebhook = request.Application == "EVT050" && request.FileId.StartsWith($"{EventoArchivoTipoReferenciaDb.INTERFAZ}.");

                if (request.Application == "INT050" || isAdjuntoEventoWebhook)
                {
                    var fileId = request.FileId;

                    if (isAdjuntoEventoWebhook)
                    {
                        fileId = fileId.Substring(fileId.IndexOf('.') + 1);
                    }

                    var nroEjecucion = long.Parse(fileId);
                    var ejecucion = await _ejecucionService.GetEjecucion(nroEjecucion);
                    var data = await _ejecucionService.GetEjecucionData(nroEjecucion);
                    var fileExtension = "xml";

                    if (ejecucion == null || data == null)
                        throw new ValidationFailedException("Archivo no encontrado");

                    if (ejecucion.IsAPI)
                        fileExtension = "json";

                    var fileName = fileId;
                    fileName += Path.HasExtension(fileName) ? string.Empty : $".{fileExtension}";

                    return new FileDownloadResponse(fileName, GetContentType(fileName), data.Data);
                }
                else if (request.Application == "FAC001")
                {
                    var fileExtension = "xlsx";

                    string nameFile = string.Format("Detalle_{0}", request.FileId);

                    byte[] libro = _facturacionService.DescargarExcel(int.Parse(request.FileId), 0, request.Application, nameFile);

                    if (libro == null)
                        throw new ValidationFailedException("Archivo no encontrado");

                    nameFile += Path.HasExtension(nameFile) ? string.Empty : $".{fileExtension}";

                    return new FileDownloadResponse(nameFile, GetContentType(nameFile), libro);
                }
                else if (request.Application == "FAC004")
                {
                    var arrayAux = request.FileId.Split(',');
                    string nuEjecucion = arrayAux[0];
                    string cdEmpresa = arrayAux[1];
                    string cdFacturacion = arrayAux[2];

                    var fileExtension = "xlsx";

                    string nameFile = string.Format("Detalle_{0}_{1}_{2}", nuEjecucion, cdEmpresa, cdFacturacion);

                    byte[] libro = _facturacionService.DescargarExcel(int.Parse(nuEjecucion), int.Parse(cdEmpresa), cdFacturacion, nameFile);

                    if (libro == null)
                        throw new ValidationFailedException("Archivo no encontrado");

                    nameFile += Path.HasExtension(nameFile) ? string.Empty : $".{fileExtension}";

                    return new FileDownloadResponse(nameFile, GetContentType(nameFile), libro);
                }
                else if (request.Application == "EVT050" && request.FileId.StartsWith($"{EventoArchivoTipoReferenciaDb.NOTIFICACION}."))
                {
                    var fileId = request.FileId.Substring(request.FileId.IndexOf('.') + 1);
                    var fileIdArgs = fileId.Split('.', StringSplitOptions.RemoveEmptyEntries);
                    var nuNotificacionArchivo = int.Parse(fileIdArgs[0]);
                    var nuNotificacion = long.Parse(fileIdArgs[1]);
                    var notificacion = uow.NotificacionRepository.GetNotificacionArchivoData(nuNotificacionArchivo, nuNotificacion);

                    return new FileDownloadResponse(notificacion.DsArchivo, GetContentType(notificacion.DsArchivo), notificacion.VlData);
                }
                else
                {
                    if (!uow.FileRepository.AnyFileById(request.FileId))
                        throw new ValidationFailedException("Archivo no encontrado");

                    var file = uow.FileRepository.GetFileById(request.FileId);
                    var path = this._configuration.Value.DirectoryPath + request.FileId;
                    var bytes = new byte[0];

                    if (System.IO.File.Exists(path))
                        bytes = System.IO.File.ReadAllBytes(path);

                    return new FileDownloadResponse(file.FileName, GetContentType(file.FileName), bytes);
                }
            }
        }

        public virtual string GetContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(fileName, out contentType))
            {
                contentType = MediaTypeNames.Application.Json;
            }
            return contentType;
        }

        public virtual FileUploadResponse AddFile(int user, FileUploadRequest request)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                //Nos fijamos que no haya un documento con el mismo nombre para evitar problemas en las descargas
                if (uow.FileRepository.AnyFileByName(request.TipoEntidad, request.CodigoEntidad, request.FileName))
                    throw new ValidationFailedException("Ya existe un documento con ese nombre");

                //Convertimos el base 64 en archivo y luego guardamos
                byte[] bytes = Convert.FromBase64String(request.Payload);

                if (!Directory.Exists(this._configuration.Value.DirectoryPath))
                    Directory.CreateDirectory(this._configuration.Value.DirectoryPath);

                var fileId = Guid.NewGuid().ToString();

                //Falta agregar el enlace con el appsetings
                string path = this._configuration.Value.DirectoryPath + fileId;
                System.IO.File.WriteAllBytes(path, bytes);

                //Preparamos para la insertar en la base de datos
                request.Size = decimal.Round(request.Size, 1);

                uow.FileRepository.AddFile(new Domain.General.File()
                {
                    FileId = fileId,
                    FileName = request.FileName,
                    Size = request.Size,
                    Content = request.Payload,
                    TipoEntidad = request.TipoEntidad,
                    CodigoEntidad = request.CodigoEntidad,
                    CodigoFuncionario = user
                });

                uow.SaveChanges();

                return new FileUploadResponse()
                {
                    FileId = fileId
                };
            }
        }

        public virtual FileDeleteResponse DeleteFile(int user, FileDeleteRequest request)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (!uow.FileRepository.AnyFileById(request.FileId))
                    throw new ValidationFailedException("No se encontró documento");

                string path = this._configuration.Value.DirectoryPath + request.FileId;

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                uow.FileRepository.RemoveFileById(request.FileId);
                uow.SaveChanges();

                return new FileDeleteResponse()
                {
                    FileId = request.FileId
                };
            }
        }
    }
}
