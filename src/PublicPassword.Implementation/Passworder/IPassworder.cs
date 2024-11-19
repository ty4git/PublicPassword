using System.Threading.Tasks;

namespace PublicPassword.Implementation.Passworder;

public interface IPassworder
{
    public abstract Task Do();
}