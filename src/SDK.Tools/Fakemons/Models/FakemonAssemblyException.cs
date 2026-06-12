namespace SDK.Tools.Fakemons.Models;

public class FakemonAssemblyException : Exception
{
    public FakemonAssemblyException(string message) : base(message) { }
    public FakemonAssemblyException(string message, Exception inner) : base(message, inner) { }
}
