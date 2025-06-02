namespace UserCore.ViewModels.Respones
{
    public class ServiceRespone
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public List<ServiceTypeRespone>? ServiceTypes { get; set; } = new List<ServiceTypeRespone>();
    }
    public class ServiceTypeRespone
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public float Price { get; set; } = 0.0f;
    }
}
