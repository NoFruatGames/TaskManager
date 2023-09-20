using System;
using TransferDataTypes;
using TransferDataTypes.Payloads;

namespace TMServer
{
    internal class Profiles
    {
        List<Profile> profiles = new List<Profile>();
        public PayloadCreateAccountResult Add(Profile profile)
        {
            PayloadCreateAccountResult check = CheckAccountCanBeCreated(profile);
            if (check != PayloadCreateAccountResult.Success) return check;
            profiles.Add(profile);
            return PayloadCreateAccountResult.Success;
        }
        public PayloadCreateAccountResult Add(PayloadAccountInfo accountInfo)
        {
            Profile profile = new Profile();
            profile.Username = accountInfo.Username;
            profile.Password = accountInfo.Password;
            profile.Email = accountInfo.Email;
            PayloadCreateAccountResult check = CheckAccountCanBeCreated(profile);
            if (check != PayloadCreateAccountResult.Success) return check;
            profiles.Add(profile);
            return PayloadCreateAccountResult.Success;
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
        private PayloadCreateAccountResult CheckAccountCanBeCreated(Profile profile)
        {
            foreach(var p in profiles)
            {
                if (p.Username == profile.Username)
                    return PayloadCreateAccountResult.UsernameAlreadyExist;
            }
            return PayloadCreateAccountResult.Success;
        }
    }
}
