namespace NewEraAPI.DTOs.CustomerDTO
{
    public class CustomerGetDTO : BaseGetDTO
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Address { get; private set; }

        public CustomerGetDTO(int ID, string firstName, string lastName, string email, string phoneNumber, string address) : base(ID)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
        }

    }
}
