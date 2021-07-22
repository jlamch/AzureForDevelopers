using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System;
using System.Threading.Tasks;

namespace az204_blobdemo
{
  class Program
  {
    private static string connectionString = "DefaultEndpointsProtocol=https;AccountName=YOUNEEDTOGETYOUROWN;AccountKey=someaccountkey==;EndpointSuffix=core.windows.net";
    //private static string containerName = "newcontainer"; //name must be lower case
    private static string containerName = "newcontainer1";
    private static string blobName = "sample-blob1.png";
    private static string dpath = "e:\\downloads\\blobdemo\\";

    static void Main(string[] args)
    {
      Console.WriteLine("Hello Azure blob storage Demo!s");
      ProcessAsync().GetAwaiter().GetResult();
    }
    private static async Task ProcessAsync()
    {
      //step 1 create container client
      var containerClient = new BlobContainerClient(connectionString, containerName);
      containerClient.CreateIfNotExists();


      //step 2 get Blob client form container client
      var blobClient = containerClient.GetBlobClient(blobName);

      //download blob 
      // var cont = blobClient.DownloadContent();


      // download file from blob and save to downloads
      // await blobClient.DownloadToAsync(dpath+"downloaded10.png");

      //lets upload new file to new blob
      var newBlobName = "downloaded1.png";
      var newBlobClient = containerClient.GetBlobClient(newBlobName);
      newBlobClient.Upload(dpath + "\\" + newBlobName, true);

      // delete last uploaded
      // newBlobClient.Delete(DeleteSnapshotsOption.IncludeSnapshots);
      newBlobClient.SetAccessTier(AccessTier.Hot);

      await LeaseBlobForNow(containerClient, newBlobName);
    }
    private static async Task LeaseBlobForNow(BlobContainerClient container, string blobName)
    {
      BlobClient blob = container.GetBlobClient(blobName);
      BlobLeaseClient leaseClient = blob.GetBlobLeaseClient();
      var lease = await leaseClient.AcquireAsync(TimeSpan.FromSeconds(50));
      Console.WriteLine(lease.Value.LeaseId);
      DoSometingWithBlob();
      Console.ReadLine();
      leaseClient.Release();
    }

    private static void DoSometingWithBlob()
    {

    }
  }
}
