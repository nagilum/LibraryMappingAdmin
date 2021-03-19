namespace DotNetLibraryAdmin.Controllers.Payloads
{
    public class PackageNewRequestPayload
    {
        public string Name { get; set; }
        public string NuGetUrl { get; set; }
        public string InfoUrl { get; set; }
        public string RepoUrl { get; set; }
        public string[] Files { get; set; }
    }
}