#load "Scalar.cs"

using System.Reflection;

public class ScalarTest
{
    [Fact]
    public void IPaymentProcessor_TypeExists()
    {
        var type = typeof(App.IPaymentProcessor);
        Assert.NotNull(type);
    }

    [Fact]
    public void LoggingBehavior_TypeExists()
    {
        var type = typeof(App.LoggingBehavior<,>);
        Assert.NotNull(type);
    }

    [Fact]
    public void StripeProcessor_TypeExists()
    {
        var type = typeof(App.StripeProcessor);
        Assert.NotNull(type);
    }

    [Fact]
    public void PayPalProcessor_TypeExists()
    {
        var type = typeof(App.PayPalProcessor);
        Assert.NotNull(type);
    }

    [Fact]
    public void Order_TypeExists()
    {
        var type = typeof(App.Order);
        Assert.NotNull(type);
    }

    [Fact]
    public void UserEvent_TypeExists()
    {
        var type = typeof(App.UserEvent);
        Assert.NotNull(type);
    }

    [Fact]
    public void Customer_TypeExists()
    {
        var type = typeof(App.Customer);
        Assert.NotNull(type);
    }

    [Fact]
    public void CheckoutService_TypeExists()
    {
        var type = typeof(App.CheckoutService);
        Assert.NotNull(type);
    }
}