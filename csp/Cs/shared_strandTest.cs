#load "shared_strand.cs"

using System.Reflection;

public class shared_strandTest
{
    [Fact]
    public void work_service_TypeExists()
    {
        var type = typeof(work_service);
        Assert.NotNull(type);
    }

    [Fact]
    public void shared_strand_TypeExists()
    {
        var type = typeof(shared_strand);
        Assert.NotNull(type);
    }

    [Fact]
    public void work_engine_hosted_service_TypeExists()
    {
        var type = typeof(work_engine_hosted_service);
        Assert.NotNull(type);
    }
}