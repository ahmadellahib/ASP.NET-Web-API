using CourseLibrary.API.Contracts.Users;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace CourseLibrary.Tests.Integration.Controllers.Users;

public partial class UsersControllerTests
{
    [Fact]
    public async Task ShouldCreateUserAsync()
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
    public async Task ShouldPatchUserAsync()
    {
        // Arrange
        UserDto randomUser = await PostRandomUserAsync();
        UserForUpdate userForUpdate = UpdateRandomUser(randomUser);

        // Act
        HttpResponseMessage responseMessage = await _httpClient.PatchAsJsonAsync(UsersRelativeUrl + "/" + randomUser.Id, userForUpdate);
        string responseContent = await responseMessage.Content.ReadAsStringAsync();
        UserDto expectedUser = JsonConvert.DeserializeObject<UserDto>(responseContent);
        UserDto actualUser = await GetUserByIdAsync(expectedUser.Id);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        actualUser.Should().BeEquivalentTo(expectedUser);

        await DeleteUserByIdAsync(actualUser.Id);
    }

    [Fact]
    public async Task ShouldGetUserByIdAsync()
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
}
