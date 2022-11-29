using StringFormatter.impl.Exception;

namespace StringFormatter.Tests;

public class Tests
{
    private class User
    {
        public string FirstName { get; }
        public string LastName { get; }
    
        public User(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string GetGreeting()
        {
            return impl.StringFormatter.Shared.Format(
                "Привет, {FirstName} {LastName}!", this);
        }
    }
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void String_Formatter_Default_Scenario()
    {
        var user = new User("Олег", "Броварской");
        var temp = impl.StringFormatter.Shared.Format("Привет,{{ }}{FirstName} {LastName}!", user);
        Assert.That(temp, Is.EqualTo("Привет,{ }Олег Броварской!"));
    }

    [Test]
    public void String_Formatter_Example()
    {
        var user = new User("Олег", "Броварской");
        var temp = impl.StringFormatter.Shared.Format("{{FirstName}} транслируется в {FirstName}", user);
        Assert.That(temp, Is.EqualTo("{FirstName} транслируется в Олег"));
    }
    
    [Test]
    public void String_Formatter_Greet_Scenario()
    {
        var user = new User("Олег", "Броварской");
        Assert.That(user.GetGreeting(), Is.EqualTo("Привет, Олег Броварской!"));
    }

    [Test]
    public void String_Formatter_Only_Escapes_Scenario()
    {
        var user = new User("Олег", "Броварской");
        var temp = impl.StringFormatter.Shared.Format("{{{{{{}}}}}}", user);
        Assert.That(temp, Is.EqualTo("{{{}}}"));
    }
    
    [Test]
    public void String_Formatter_Field_Among_Escapes()
    {
        var user = new User("Олег", "Броварской");
        var temp = impl.StringFormatter.Shared.Format("{{{{{{{LastName}}}}}}}", user);
        Assert.That(temp, Is.EqualTo("{{{Броварской}}}"));
    }
    
    [Test]
    public void String_Formatter_Exception_Among_Escapes()
    {
        var user = new User("Олег", "Броварской");
        Assert.Throws<InvalidStringException>(() => 
            impl.StringFormatter.Shared.Format("{{{{{{}}}}}}}", user));
    }
    
    [Test]
    public void String_Formatter_Invalid_String_Exception()
    {
        var user = new User("Олег", "Броварской");
        Assert.Throws<InvalidStringException>(() => impl.StringFormatter.Shared.Format("Привет,{{ }}{FirstName} {LastName}!{", user));
    }
    
    [Test]
    public void String_Formatter_Empty_String()
    {
        var user = new User("Олег", "Броварской");
        var temp = impl.StringFormatter.Shared.Format("", user);
        Assert.That(temp, Is.EqualTo(""));
    }
}
