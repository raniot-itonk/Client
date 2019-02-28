namespace Client.Models.Requests.BankService
{
    public class CreateAccountRequest
    {
        public string OwnerName { get; set; }
        public double Balance { get; set; }
    }
}
