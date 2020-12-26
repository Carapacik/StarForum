using System.Collections.Generic;

namespace SForum.Models.ApplicationUser
{
    public class ProfileListModel
    {
        public IEnumerable<ProfileModel> Profiles { get; set; }
        public string TypeQuery { get; set; }
        public string OldTypeQuery { get; set; }
    }
}