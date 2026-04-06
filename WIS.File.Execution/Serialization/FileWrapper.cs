using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using WIS.Serialization;
using WIS.Serialization.Binders;

namespace WIS.File.Execution.Serialization
{
    public class FileWrapper : TransferWrapper, IFileWrapper
    {
        public override ISerializationBinder GetSerializationBinder()
        {
            return new CustomSerializationBinder(new List<Type> {
                typeof(FileUploadRequest),
                typeof(FileUploadResponse),
                typeof(FileDownloadRequest),
                typeof(FileDownloadResponse),
                typeof(FileDeleteRequest),
                typeof(FileDeleteResponse),
            });
        }
    }
}
