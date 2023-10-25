namespace Crumb.Core.Utility;
public class SystemConsole : IConsole
{
    public int WindowWidth { get => Console.WindowWidth; }
    public int WindowHeight { get => Console.WindowHeight; }

    public void Clear() => Console.Clear();

    public int Read() => Console.Read();

    public string? ReadLine() => Console.ReadLine();

    public void Write(string value) => Console.Write(value);

    public void WriteLine(string value) => Console.WriteLine(value);
}
