using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;           // dotnet add package System.Security.Claims
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;    // dotnet add package Microsoft.AspNetCore.Identity
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Fuji.Models;
using Fuji.Controllers;
using Fuji.DAL;
using Fuji.DAL.Concrete;

namespace Fuji_Tests;

public class HomeControllerTests
{
    private Mock<FujiDbContext> _mockContext;
    private Mock<DbSet<Apple>> _mockAppleDbSet;
    private List<Apple> _apples;

    private Mock<DbSet<FujiUser>> _mockFujiUserDbSet;
    private List<FujiUser> _fujiUsers;

    private Mock<IUserStore<IdentityUser>> _mockUserStore;

    // a helper to make dbset queryable
    private Mock<DbSet<T>> GetMockDbSet<T>(IQueryable<T> entities) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(entities.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(entities.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(entities.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(entities.GetEnumerator());
        return mockSet;
    }

    [SetUp]
    public void Setup()
    {
        _mockContext = new Mock<FujiDbContext>();

        _apples = new List<Apple>
        {
            new Apple { Id = 1, VarietyName = "Fuji", ScientificName = "Malus pumila"},
            new Apple { Id = 2, VarietyName = "Braeburn", ScientificName = "Malus domestica"},
            new Apple { Id = 3, VarietyName = "Gala", ScientificName = "Malus domestica"}
        };
        _mockAppleDbSet = GetMockDbSet<Apple>(_apples.AsQueryable<Apple>());
        _mockAppleDbSet.Setup(a => a.Find(It.IsAny<object[]>())).Returns((object[] x) => {
            int id = (int)x[0];
            return _apples.Where(a => a.Id == id).First();
        });
        _mockContext.Setup(ctx => ctx.Apples).Returns(_mockAppleDbSet.Object);
        _mockContext.Setup(ctx => ctx.Set<Apple>()).Returns(_mockAppleDbSet.Object);

        _fujiUsers = new List<FujiUser>
        {
            new FujiUser { Id = 1, AspnetIdentityId = "437c0c0a-6f47-45ac-b96e-e109e2393052", FirstName = "Hareem", LastName = "Davila"},
            new FujiUser { Id = 2, AspnetIdentityId = "697e2ba8-8b44-43f4-9f33-9d4ddd31992c", FirstName = "Krzysztof", LastName = "Ponce"},
            new FujiUser { Id = 3, AspnetIdentityId = "9f091fbe-c7c4-471e-a404-f0b83b8b18cf", FirstName = "Zayden", LastName = "Clark"},
            new FujiUser { Id = 4, AspnetIdentityId = "a3ba4ad5-dc0c-4571-acc2-87d53e0fd837", FirstName = "Talia", LastName = "Knott"}
        };
        _mockFujiUserDbSet = GetMockDbSet<FujiUser>(_fujiUsers.AsQueryable<FujiUser>());
        _mockFujiUserDbSet.Setup(f => f.Find(It.IsAny<object[]>())).Returns((object[] x) => {
            int id = (int)x[0];
            return _fujiUsers.Where(f => f.Id == id).First();
        });
        _mockContext.Setup(ctx => ctx.FujiUsers).Returns(_mockFujiUserDbSet.Object);
        _mockContext.Setup(ctx => ctx.Set<FujiUser>()).Returns(_mockFujiUserDbSet.Object);

        // Now mock the things we need from Identity: a store of users that the user manager will use
        _mockUserStore = new Mock<IUserStore<IdentityUser>>();
        
        
    }

    
    // Here's a test that mocks the controller for no logged in user
    [Test]
    public void IndexVM_UserNotSignedIn_ShouldNotHaveAFujiUser()
    {
        // Arrange
        AppleRepository appleRepo = new AppleRepository(_mockContext.Object);
        FujiUserRepository fujiUserRepo = new FujiUserRepository(_mockContext.Object);

        UserManager<IdentityUser> userManager = MockHelpers.TestUserManager<IdentityUser>();
        HomeController controller = new HomeController(null,userManager,fujiUserRepo,appleRepo);
        controller.ControllerContext = new ControllerContext();
        controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal( new ClaimsIdentity() ) };

        // Act (call the action method)
        ViewResult result = (ViewResult)controller.Index();
        MainPageVM vm = (MainPageVM)result.Model;

        // Assert
        Assert.That(vm.HasFujiUser, Is.False);
    }

    // And now one that sets it up to actually have a logged in user and a FujiUser
    [Test]
    public void IndexVM_SignedInUser_ShouldHaveAFujiUser()
    {
        // Arrange
        AppleRepository appleRepo = new AppleRepository(_mockContext.Object);
        FujiUserRepository fujiUserRepo = new FujiUserRepository(_mockContext.Object);

        // choose a user for the test and put them in the store.  This is what Identity knows already
        // Would be easy to put all our seeded users in there.  Do it just like the Find on line 67 above
        var store = new Mock<IUserStore<IdentityUser>>();
        store.Setup(x => x.FindByIdAsync("437c0c0a-6f47-45ac-b96e-e109e2393052", It.IsAny<CancellationToken>()))
             .ReturnsAsync(
                 new IdentityUser()
                 {
                     Id = "437c0c0a-6f47-45ac-b96e-e109e2393052",
                     Email = "hareem@example.com",
                     EmailConfirmed = false
    }
             );
        UserManager<IdentityUser> userManager = MockHelpers.TestUserManager<IdentityUser>(store.Object);
        // Now set up the controller and what info comes in with the request, i.e. the claims the user presents to us
        ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
            new Claim(ClaimTypes.NameIdentifier, "437c0c0a-6f47-45ac-b96e-e109e2393052"),
            new Claim(ClaimTypes.Name, "hareem@example.com"),
            new Claim(ClaimTypes.Email, "hareem@example.com")
        }));
        HomeController controller = new HomeController(null,userManager,fujiUserRepo,appleRepo);
        controller.ControllerContext = new ControllerContext();
        controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };

        // Act (call the action method)
        ViewResult result = (ViewResult)controller.Index();
        MainPageVM vm = (MainPageVM)result.Model;

        // Assert (in this case we're not asserting anything useful, but it does show that we can
        // authenticate and access a particular, faked, user)
        Assert.That(vm.HasFujiUser, Is.True);
        Assert.That(vm.FirstName, Is.EqualTo("Hareem"));
        Assert.That(vm.LastName, Is.EqualTo("Davila"));
    }
}