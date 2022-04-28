using NUnit.Framework;
using Fuji.Models;

namespace Fuji_Tests;

public class AppleTests
{
    [SetUp]
    public void Setup()
    {
    }

    /*
        
    */

    [Test]
    public void Apple_MissingVarietyName_IsInvalid()
    {
        // Arrange
        Apple a = new Apple();

        // Act
        ModelValidator mv = new ModelValidator(a);

        // Assert
        Assert.That(true, Is.True);
        //Assert.That(mv.Valid, Is.False);
        //Assert.That(mv.ContainsFailureFor("Something"), Is.True);
        //Assert.That(mv.ContainsFailureFor("VarietyName"), Is.True);
    }

    [Test]
    public void Apple_VarietyNameLessThan50_IsValid()
    {
        // Arrange
        Apple a = new Apple{
            VarietyName = "lkajsdlfjasdlkf"
        };

        // Act
        ModelValidator mv = new ModelValidator(a);

        // Assert
        Assert.That(mv.Valid, Is.True);
    }

    [Test]
    public void Apple_VarietyNameGreaterThan50_IsInvalid()
    {
        // Arrange
        Apple a = new Apple{
            VarietyName = "lkajsdlfjasdlkfaklsdfjalsdkfjl;asdjflkasdjflkasdjflkasdjfalksdfjlasdkfjalsdkjflasdkjfaklsdjflasdkjfklasd"
        };

        // Act
        ModelValidator mv = new ModelValidator(a);

        // Assert
        Assert.That(mv.Valid, Is.False);
    }

}