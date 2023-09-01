namespace Crumb.Core.Utility;
public interface IConsole
{
    void Write(string value);
    void WriteLine(string value);
    int Read();
    string? ReadLine();
}
