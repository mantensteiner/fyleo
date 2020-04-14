using System.Collections.Generic;

namespace fyleo.Repository.Models
{
    public interface IAuthRepository
    {
        AuthConfig Get();
    }
}