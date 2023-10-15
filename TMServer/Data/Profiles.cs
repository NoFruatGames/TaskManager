namespace TMServer.Data
{
    internal class Profiles
    {
        private readonly List<Profile> profiles = new List<Profile>();
        public CanAddProfile Add(Profile profile)
        {
            CanAddProfile check = CannAddCheck(profile);
            if(check == CanAddProfile.Success)
                profiles.Add(profile);
            return check;
        }
        public Profile? this[int id]
        {
            get
            {
                if (id >= 0 && id < profiles.Count)
                    return profiles[id];
                else
                    return null;
            }
        }
        public Profile? this[string username]
        {
            get
            {
                foreach(var p in profiles)
                {
                    if(p.Username == username) return p;
                }
                return null;
            }
        }

        private CanAddProfile CannAddCheck(Profile profile)
        {
            foreach(var p in profiles)
            {
                if (p.Username == profile.Username)
                    return CanAddProfile.UsernameExist;
            }
            return CanAddProfile.Success;

        }
    }
    internal enum CanAddProfile
    {
        None, Success, UsernameExist
    }
}
