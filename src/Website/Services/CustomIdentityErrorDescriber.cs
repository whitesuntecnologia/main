using Microsoft.AspNetCore.Identity;

namespace Website.Services
{
    public class CustomIdentityErrorDescriber: IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = $"El nombre de usuario {userName} ya existe, debe elegir otro."
            };
        }
    }
}
