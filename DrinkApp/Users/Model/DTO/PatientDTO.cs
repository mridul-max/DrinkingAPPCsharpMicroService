using System;

namespace Users.Model.DTO
{
    public class PatientDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool Active { get; set; }
        public PatientCareGiverDTO Nurse { get; set; }
        //public CareGiver CareGiver { get; set; }
        public int DailyLimit { get; set; }
        public UserRole UserRole { get; set; }

        //public List<RecipeModel> LikedRecipes { get; set; } = new List<RecipeModel>();
        //public Guid LogId { get; set; }
        public int DailyGoal { get; set; }
        public DateTime DateOfBirth { get; set; }

    }
}
