namespace Assurity.PolicyInfo.Contracts.V1
{
    public class Participant
    {
        public Address Address { get; set; }

        public Business? Business { get; set; }

        public bool IsBusiness { get; set; }

        public Person? Person { get; set; }

        public string PhoneNumber { get; set; }
    }
}