namespace Crumb.Core.Utility;
public class SystemConsole : IConsole
{
    public int Read() => Console.Read();

    public string? ReadLine() => Console.ReadLine();

    public void Write(string value) => Console.Write(value);

    public void WriteLine(string value) => Console.WriteLine(value);
}
