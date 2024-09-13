namespace EventStaf.Infra.Swagger
{
	public class SwaggerPropertyAttribute: Attribute
	{
        public SwaggerPropertyType SwaggerPropertyType { get; private set; }

        public SwaggerPropertyAttribute(SwaggerPropertyType swaggerPropertyType)
        {
            SwaggerPropertyType = swaggerPropertyType;

		}
    }


    public enum SwaggerPropertyType 
    {
        Name,
        LastName,
        UserName,
        Password,
        MaskedPasword,
        Email,
        PhoneNumber,
        BirthDate,  
        RegisterDate,
        Salary
    }
}