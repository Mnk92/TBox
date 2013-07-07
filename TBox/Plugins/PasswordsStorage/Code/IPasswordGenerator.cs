namespace Mnk.TBox.Plugins.PasswordsStorage.Code
{
    public interface IPasswordGenerator
    {
        string Generate(int passwordLength, int passwordNonAlphaCharacters);
    }
}
