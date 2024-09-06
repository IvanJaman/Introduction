
// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


// Configure the HTTP request pipeline.


public class FacebookPost
{
    public Guid Id { get; set; }
    public string Caption { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}