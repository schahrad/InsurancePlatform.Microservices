namespace WebClient.ViewModels
{
    public sealed class PolicyViewModel
    {
        public string PolicyNumber { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public string Status { get; set; } = default!;
        public decimal Premium { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }

}
