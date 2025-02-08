public class UserListDto
{
    public string Id { get; set; }  // 🔹 IdentityUser folosește string ca Id
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Rol { get; set; }
}
