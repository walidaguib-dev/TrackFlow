using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Moq;
using Tests.UnitTests.Infrastructure.Auth;
using Xunit;

namespace TrackFlow.Tests.UnitTests.Infrastructure.Repositories
{
    public class UsersRepositoryTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly UsersRepository _repository;

        public UsersRepositoryTests()
        {
            _mockUserManager = MockUserManager.CreateMock<User>();
            _repository = new UsersRepository(_mockUserManager.Object);
        }

        // ==================== SUCCESS TESTS ====================

        [Fact]
        public async Task AddAsync_ValidUser_ShouldCreateUserAndAssignRole()
        {
            // Arrange
            var email = "test@example.com";
            var userName = "Test User";
            var password = "SecurePass123!";
            var role = UserRoles.Customer;
            var id = Guid.NewGuid();

            var user = new User { Email = email, UserName = "Test User" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((User)null!);

            _mockUserManager
                .Setup(x => x.CreateAsync(user, password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager
                .Setup(x => x.AddToRoleAsync(user, role.ToString()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _repository.AddAsync(user, role, password);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(email);
            result.UserName.Should().Be(userName);

            _mockUserManager.Verify(x => x.FindByEmailAsync(email), Times.Once);
            _mockUserManager.Verify(x => x.CreateAsync(user, password), Times.Once);
            _mockUserManager.Verify(x => x.AddToRoleAsync(user, role.ToString()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_WithAdminRole_ShouldAssignAdminRole()
        {
            // Arrange
            var email = "admin@example.com";
            var role = UserRoles.Admin;
            var password = "SecurePass123!";

            var user = new User { Email = email, UserName = "Test User" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((User)null!);

            _mockUserManager
                .Setup(x => x.CreateAsync(user, password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager
                .Setup(x => x.AddToRoleAsync(user, role.ToString()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _repository.AddAsync(user, role, password);

            // Assert
            _mockUserManager.Verify(x => x.AddToRoleAsync(user, "Admin"), Times.Once);
        }

        [Fact]
        public async Task AddAsync_WithDriverRole_ShouldAssignDriverRole()
        {
            // Arrange
            var email = "driver@example.com";
            var role = UserRoles.Driver;
            var password = "SecurePass123!";

            var user = new User { Email = email, UserName = "Test User" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((User)null!);

            _mockUserManager
                .Setup(x => x.CreateAsync(user, password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager
                .Setup(x => x.AddToRoleAsync(user, role.ToString()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _repository.AddAsync(user, role, password);

            // Assert

            _mockUserManager.Verify(x => x.AddToRoleAsync(user, "Driver"), Times.Once);
        }

        [Fact]
        public async Task AddAsync_WithDispatcherRole_ShouldAssignDispatcherRole()
        {
            // Arrange
            var email = "dispatcher@example.com";
            var role = UserRoles.Dispatcher;
            var password = "SecurePass123!";

            var user = new User { Email = email, UserName = "Test User" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((User)null!);

            _mockUserManager
                .Setup(x => x.CreateAsync(user, password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager
                .Setup(x => x.AddToRoleAsync(user, role.ToString()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _repository.AddAsync(user, role, password);

            // Assert
            _mockUserManager.Verify(x => x.AddToRoleAsync(user, "Dispatcher"), Times.Once);
        }

        // ==================== EXISTING USER TESTS ====================

        [Fact]
        public async Task AddAsync_UserAlreadyExists_ShouldThrowInvalidDataException()
        {
            // Arrange
            var email = "existing@example.com";

            var existingUser = new User { Email = email, UserName = "Existing User" };

            var newUser = new User { Email = email, UserName = "Test User" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(existingUser);

            // Act
            var act = () => _repository.AddAsync(newUser, UserRoles.Customer, "password");

            // Assert
            await act.Should()
                .ThrowAsync<InvalidDataException>()
                .WithMessage("User already exists!");

            _mockUserManager.Verify(
                x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()),
                Times.Never
            );
            _mockUserManager.Verify(
                x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()),
                Times.Never
            );
        }

        [Fact]
        public async Task AddAsync_UserAlreadyExists_WithDifferentCaseEmail_ShouldThrowException()
        {
            // Arrange
            var email = "Existing@Example.com";
            var lowerEmail = "existing@example.com";

            var existingUser = new User { Email = email, UserName = "Existing User" };

            var newUser = new User { Email = lowerEmail, UserName = "Test User" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(lowerEmail)).ReturnsAsync(existingUser);

            // Act
            var act = () => _repository.AddAsync(newUser, UserRoles.Customer, "password");

            // Assert
            await act.Should()
                .ThrowAsync<InvalidDataException>()
                .WithMessage("User already exists!");
        }

        // ==================== FAILURE TESTS ====================

        [Fact]
        public async Task AddAsync_CreateFails_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var email = "test@example.com";

            var user = new User { Email = email, UserName = "Test User" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((User)null!);

            var errors = new IdentityError[]
            {
                new() { Description = "Password too weak" },
                new() { Description = "Email already taken" },
            };

            _mockUserManager
                .Setup(x => x.CreateAsync(user, "password"))
                .ReturnsAsync(IdentityResult.Failed(errors));

            // Act
            var act = () => _repository.AddAsync(user, UserRoles.Customer, "password");

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("User creation failed: Password too weak, Email already taken");

            _mockUserManager.Verify(
                x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()),
                Times.Never
            );
        }

        [Fact]
        public async Task AddAsync_CreateFailsWithMultipleErrors_ShouldShowAllErrors()
        {
            // Arrange
            var email = "test@example.com";

            var user = new User { Email = email, UserName = "Existing User" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((User)null!);

            var errors = new IdentityError[]
            {
                new() { Description = "Password too weak" },
                new() { Description = "Email already taken" },
                new() { Description = "Invalid username format" },
            };

            _mockUserManager
                .Setup(x => x.CreateAsync(user, "password"))
                .ReturnsAsync(IdentityResult.Failed(errors));

            // Act
            var act = () => _repository.AddAsync(user, UserRoles.Customer, "password");

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage(
                    "User creation failed: Password too weak, Email already taken, Invalid username format"
                );
        }

        [Fact]
        public async Task AddAsync_RoleAssignmentFails_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { Email = email, UserName = "Test User" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((User)null!);

            _mockUserManager
                .Setup(x => x.CreateAsync(user, "password"))
                .ReturnsAsync(IdentityResult.Success);

            var errors = new IdentityError[] { new() { Description = "Role not found" } };

            _mockUserManager
                .Setup(x => x.AddToRoleAsync(user, "Customer"))
                .ReturnsAsync(IdentityResult.Failed(errors));

            // Act
            var act = () => _repository.AddAsync(user, UserRoles.Customer, "password");

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Role assignment failed: Role not found");

            _mockUserManager.Verify(x => x.CreateAsync(user, "password"), Times.Once);
        }

        [Fact]
        public async Task AddAsync_RoleAssignmentFailsWithMultipleErrors_ShouldShowAllErrors()
        {
            // Arrange
            var email = "test@example.com";

            var user = new User { Email = email, UserName = "Test User" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((User)null!);

            _mockUserManager
                .Setup(x => x.CreateAsync(user, "password"))
                .ReturnsAsync(IdentityResult.Success);

            var errors = new IdentityError[]
            {
                new() { Description = "Role not found" },
                new() { Description = "User not found" },
            };

            _mockUserManager
                .Setup(x => x.AddToRoleAsync(user, "Customer"))
                .ReturnsAsync(IdentityResult.Failed(errors));

            // Act
            var act = () => _repository.AddAsync(user, UserRoles.Customer, "password");

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Role assignment failed: Role not found, User not found");
        }

        // ==================== EDGE CASE TESTS ====================

        [Fact]
        public async Task AddAsync_UserWithoutPassword_ShouldStillCallCreateAsync()
        {
            // Arrange
            var email = "test@example.com";
            var password = ""; // Empty password

            var user = new User { Email = email, UserName = "Test User" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((User)null!);

            _mockUserManager
                .Setup(x => x.CreateAsync(user, password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager
                .Setup(x => x.AddToRoleAsync(user, "Customer"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _repository.AddAsync(user, UserRoles.Customer, password);

            // Assert
            result.Should().NotBeNull();
            _mockUserManager.Verify(x => x.CreateAsync(user, password), Times.Once);
        }

        [Fact]
        public async Task AddAsync_CreateAsyncThrowsException_ShouldPropagateException()
        {
            // Arrange
            var email = "test@example.com";

            var user = new User { Email = email, UserName = "Test User" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((User)null!);

            _mockUserManager
                .Setup(x => x.CreateAsync(user, "password"))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var act = () => _repository.AddAsync(user, UserRoles.Customer, "password");

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Database connection failed");
        }
    }
}
