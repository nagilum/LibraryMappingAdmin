namespace DotNetLibraryAdmin.Controllers.Payloads
{
    public class BadPackageVersionCreateNewRequestPayload
    {
        public long PackageId { get; set; }
        public string FileVersionFrom { get; set; }
        public string FileVersionTo { get; set; }
        public string ProductVersionFrom { get; set; }
        public string ProductVersionTo { get; set; }
    }
}