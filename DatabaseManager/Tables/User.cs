using System.ComponentModel.DataAnnotations;
namespace DatabaseManager.Tables
{
    public class User :IDBTable
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public char? Gender { get; set; }
        public ICollection<Session> Sessions { get; set; }
    }
}
