namespace WebClient.ViewModels
{
    public sealed class ClaimViewModel
    {
        public string ClaimNumber { get; set; } = default!;
        public string PolicyNumber { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime ReportedOn { get; set; }
        public decimal Amount { get; set; }
    }
}
