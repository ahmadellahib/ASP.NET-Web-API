using CourseLibrary.API.Contracts.Users;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace CourseLibrary.Tests.Integration.Controllers.Users;

public partial class UsersControllerTests
{

    [Fact]
    public async Task POST_ShouldCreateUserAsync()
    {
        // Arrange
        UserForCreation userForCreation = CreateRandomUserForCreation();

        // Act
        HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync(UsersRelativeUrl, userForCreation);
        string responseContent = await responseMessage.Content.ReadAsStringAsync();
        UserDto expectedUser = JsonConvert.DeserializeObject<UserDto>(responseContent);
        UserDto actualUser = await GetUserByIdAsync(expectedUser.Id);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Created);
        actualUser.Should().BeEquivalentTo(expectedUser);

        await DeleteUserByIdAsync(actualUser.Id);
    }

    [Fact]
    public async Task PATCH_ShouldUpdateUserAsync()
    {
        // Arrange
        UserDto randomUser = await PostRandomUserAsync();
        UserForUpdate userForUpdate = UpdateRandomUser(randomUser);

        // Act
        HttpResponseMessage responseMessage = await _httpClient.PatchAsJsonAsync(UsersRelativeUrl + "/" + randomUser.Id, userForUpdate);
        string responseContent = await responseMessage.Content.ReadAsStringAsync();
        UserDto expectedUser = JsonConvert.DeserializeObject<UserDto>(responseContent);
        UserDto actualUser = await GetUserByIdAsync(randomUser.Id);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        actualUser.Should().BeEquivalentTo(expectedUser);

        await DeleteUserByIdAsync(actualUser.Id);
    }

    [Fact]
    public async Task GET_ShouldGetUserByIdAsync()
    {
        // Arrange
        UserDto expectedUser = await PostRandomUserAsync();

        // Act
        HttpResponseMessage responseMessage = await _httpClient.GetAsync(UsersRelativeUrl + "/" + expectedUser.Id);
        string responseContent = await responseMessage.Content.ReadAsStringAsync();
        UserDto actualUser = JsonConvert.DeserializeObject<UserDto>(responseContent);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        actualUser.Should().BeEquivalentTo(expectedUser);

        await DeleteUserByIdAsync(actualUser.Id);
    }

    [Fact]
    public async Task GET_ShouldReturnNotFound_WhenUserNotExistsAsync()
    {
        // Arrange
        Guid randomUserId = Guid.NewGuid();

        // Act
        HttpResponseMessage responseMessage = await _httpClient.GetAsync(UsersRelativeUrl + "/" + randomUserId);
        string responseContent = await responseMessage.Content.ReadAsStringAsync();

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
        responseContent.Should().Be($"The requested resource User with id: {randomUserId} could not be found.");
    }

    [Fact]
    public async Task GET_ShouldGetAllUsersAsync()
    {
        // Arrange
        List<UserDto> expectedUsers = await CreateRandomPostedUsersAsync();

        // Act
        HttpResponseMessage responseMessage = await _httpClient.GetAsync(UsersRelativeUrl);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        string responseContent = await responseMessage.Content.ReadAsStringAsync();
        List<UserDto> actualUsers = JsonConvert.DeserializeObject<List<UserDto>>(responseContent);

        foreach (UserDto expectedUser in expectedUsers)
        {
            UserDto actualUser = actualUsers.Single(user => user.Id == expectedUser.Id);
            actualUser.Should().BeEquivalentTo(expectedUser);

            await DeleteUserByIdAsync(actualUser.Id);
        }
    }

    [Fact]
    public async Task DELETE_ShouldReturnMethodNotAllowedAsync()
    {
        // Arrange
        Guid randomUserId = Guid.NewGuid();

        // Act
        HttpResponseMessage responseMessage = await _httpClient.DeleteAsync(UsersRelativeUrl + "/" + randomUserId);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
    }
}
