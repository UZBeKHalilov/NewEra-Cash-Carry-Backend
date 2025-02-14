namespace NewEraAPI.DTOs
{
    public class BaseGetDTO : IBaseGetDTO
    {
        public int ID { get; set; }

        public BaseGetDTO(int ID)
        {
            this.ID = ID;
        }
    }
}
