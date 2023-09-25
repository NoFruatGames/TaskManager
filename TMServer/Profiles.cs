using System;
using TransferDataTypes;
using TransferDataTypes.Payloads;

namespace TMServer
{
    internal class Profiles
    {
        List<Profile> profiles = new List<Profile>();
        public CreateAccountResult Add(Profile profile)
        {
            CreateAccountResult check = CheckAccountCanBeCreated(profile);
            if (check != CreateAccountResult.Success) return check;
            profiles.Add(profile);
            return CreateAccountResult.Success;
        }
        public CreateAccountResult Add(PayloadAccountInfo accountInfo)
        {
            Profile profile = new Profile();
            profile.Username = accountInfo.Username;
            profile.Password = accountInfo.Password;
            profile.Email = accountInfo.Email;
            CreateAccountResult check = CheckAccountCanBeCreated(profile);
            if (check != CreateAccountResult.Success) return check;
            profiles.Add(profile);
            return CreateAccountResult.Success;
        }
        public int Count { get { return profiles.Count; } }
        public Profile? GetByUsername(string username)
        {
            foreach (var p in profiles)
            {
                if (p.Username == username)
                    return p;
            }
            return null;
        }
        public Profile? GetById(int id)
        {
            if(id >= 0 && id < profiles.Count)
                return profiles[id];
            else
                return null;
        }
        private CreateAccountResult CheckAccountCanBeCreated(Profile profile)
        {
            foreach(var p in profiles)
            {
                if (p.Username == profile.Username)
                    return CreateAccountResult.UsernameAlreadyExist;
            }
            return CreateAccountResult.Success;
        }
    }
}
