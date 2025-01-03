﻿using CourseLibrary.API.Contracts.Users;
using System.Net.Http.Json;
using System.Text.Json;
using Tynamix.ObjectFiller;

namespace CourseLibrary.Tests.Integration.Controllers.Users;

public partial class UsersControllerTests : IClassFixture<IntegrationTestFactory>
{
    public const string UsersRelativeUrl = "api/v1/users";
    private readonly HttpClient _httpClient;

    public UsersControllerTests(IntegrationTestFactory integrationTestFactory)
    {
        _httpClient = integrationTestFactory.CreateClient();
    }

    public UserForCreation CreateRandomUserForCreation()
    {
        DateTimeOffset dateTimeOffset = GetRandomDateTime();
        Filler<UserForCreation> filler = new();

        filler.Setup()
            .OnProperty(user => user.DateOfBirth).Use(dateTimeOffset)
            .OnProperty(user => user.DateOfDeath).Use(dateTimeOffset);

        return filler.Create();
    }

    public UserForUpdate UpdateRandomUser(UserDto userDto)
    {
        DateTimeOffset dateTimeOffset = GetRandomDateTime();
        Filler<UserForUpdate> filler = new();

        filler.Setup()
            .OnProperty(user => user.ConcurrencyStamp).Use(userDto.ConcurrencyStamp)
            .OnProperty(user => user.DateOfBirth).Use(dateTimeOffset)
            .OnProperty(user => user.DateOfDeath).Use(dateTimeOffset);

        return filler.Create();
    }

    private async Task<UserDto> PostRandomUserAsync()
    {
        UserForCreation userForCreation = CreateRandomUserForCreation();
        HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync(UsersRelativeUrl, userForCreation);
        string responseContent = await responseMessage.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    private async Task<List<UserDto>> CreateRandomPostedUsersAsync()
    {
        int randomNumber = GetRandomNumber(2, 10);
        List<UserDto> randomPosts = new();

        for (int i = 0; i < randomNumber; i++)
        {
            randomPosts.Add(await PostRandomUserAsync());
        }

        return randomPosts;
    }

    private async Task<UserDto> GetUserByIdAsync(Guid userId) =>
        await _httpClient.GetFromJsonAsync<UserDto>(UsersRelativeUrl + "/" + userId);

    private async Task DeleteUserByIdAsync(Guid userId) =>
        await _httpClient.DeleteAsync(UsersRelativeUrl + "/" + userId);

    private DateTimeOffset GetRandomDateTime() =>
        new DateTimeRange(earliestDate: new DateTime()).GetValue();

    private int GetRandomNumber(int minimum, int maximum) =>
        new IntRange(minimum, maximum).GetValue();
}