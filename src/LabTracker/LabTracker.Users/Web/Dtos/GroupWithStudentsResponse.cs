using LabTracker.Users.Domain;

namespace Users.Web.Dtos;

public class GroupWithStudentsResponse
{
    public string Group { get; set; }
    public List<UserResponse> Students { get; set; }

    public static GroupWithStudentsResponse Create(string group, List<User> students)
    {
        return new GroupWithStudentsResponse
        {
            Group = group,
            Students = students.Select(UserResponse.Create).ToList()
        };
    }
}