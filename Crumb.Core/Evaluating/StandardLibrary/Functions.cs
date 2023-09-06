namespace Crumb.Core.Evaluating.StandardLibrary;
internal class Functions
{
    internal static string PrintLine => $$"""
        {
            (
                fun [ line ]
                {
                    ( print line )
                    ( print "\r\n" )
                }
            )
        }
        """;
}
