using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace GlobalShopping.Helpers
{
    public class BlobHelper : IBlobHelper
    {
        private readonly IConfiguration _configuration;

        //private readonly CloudBlobClient _blobClient;
        public BlobHelper(IConfiguration configuration)
        {
            //Subida imagenes con Azure
                //string keys = configuration["Blob:ConnectionString"];
                //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(keys);
                //_blobClient = storageAccount.CreateCloudBlobClient();

            _configuration = configuration;
        }
        public async Task DeleteBlobAsync(Guid id, string containerName)
        {
            try
            {
                Guid name = id;
                string Path = $"{_configuration["Blob:ImagesPath"]}\\{containerName}\\{name}.png";
                if (File.Exists(Path))
                    File.Delete(Path);
            }
            catch { }
        }

        public async Task<Guid> UploadBlobAsync(IFormFile file, string containerName)
        {
            Stream stream = file.OpenReadStream();
            return await UploadBlobAsync(stream, containerName);

        }

        public async Task<Guid> UploadBlobAsync(byte[] file, string containerName)
        {
            MemoryStream stream = new MemoryStream(file);
            return await UploadBlobAsync(stream, containerName);

        }

        public async Task<Guid> UploadBlobAsync(string image, string containerName)
        {
            Stream stream = File.OpenRead(image);
            return await UploadBlobAsync(stream, containerName);

        }

        private async Task<Guid> UploadBlobAsync(Stream stream, string containerName)
        {
            Guid name = Guid.NewGuid();

            string Path = $"{_configuration["Blob:ImagesPath"]}\\{containerName}\\{name}.png";
            if (File.Exists(Path))  
                File.Delete(Path);
            
            using (FileStream fileStream = System.IO.File.Create(Path))
            {
                fileStream.Write(ReadFully(stream));
                fileStream.Close();
            }

            return name;

            //Guid name = Guid.NewGuid();
            //CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            //CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{name}");
            //await blockBlob.UploadFromStreamAsync(stream);
            //return name;
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
