using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using ProjectMongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using IdentityServer4.Test;
using System.Reflection;


namespace ClientMongo.Pages
{
    public partial class EditUser
    {
        [Parameter]
        public string? Id { get; set; }
        private List<User> UsersWithPassport = new();
        public User CurrentUser { get; set; } = new();
        public PassportUser CurrentPassportUser { get; set; } = new();
        [Inject] private HttpClient HttpClient { get; set; }
        [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; }
        [Inject] private IConfiguration Config { get; set; }
        [Inject] public NavigationManager UrlNavigationManager { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            var accessToken = await HttpContextAccessor.HttpContext.GetTokenAsync("access_token");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            if (Id != null)
            {
                //var user = await HttpClient.GetAsync($"{Config["apiUrl"]}/UserWithPassport/{Id}");
                //var user = await HttpClient.GetFromJsonAsync<User>($"{Config["apiUrl"]}/UserWithPassport/{Id}");
                List<User> Users = await HttpClient.GetFromJsonAsync<List<User>>($"{Config["apiUrl"]}/UserWithPassport/{Id}");
                var user = Users[0];
                if (user != null)
                {
                    //CurrentUser = await user.Content.ReadFromJsonAsync<User>();
                    CurrentUser = user;
                    CurrentPassportUser = CurrentUser.Passport;
                    //CurrentUser = JsonConvert.DeserializeObject<User>(Convert.ToString(user.Content));
                }
            }
        }
        private async Task HandleSubmit()
        {
            var accessToken = await HttpContextAccessor.HttpContext.GetTokenAsync("access_token");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            if (Id == null)
            {
                try
                {
                    //CurrentUser.Id = Guid.NewGuid().ToString("N");
                    //CurrentPassportUser.Id = Guid.NewGuid().ToString("N");
                    CurrentUser.Passport = CurrentPassportUser;
                    CurrentUser.Passport.UserId = CurrentUser.Id;
                    await HttpClient.PostAsJsonAsync($"{Config["apiUrl"]}/AddUser", CurrentUser);
                    await HttpClient.PostAsJsonAsync($"{Config["apiUrl"]}/AddPassport", CurrentPassportUser);
                }
                catch(BadHttpRequestException ex)
                {
                    throw new BadHttpRequestException(ex.Message);
                }

            }
            else
            {
                CurrentUser.Passport = CurrentPassportUser;
                await HttpClient.PutAsJsonAsync($"{Config["apiUrl"]}/updateUser/{CurrentUser.Id}", CurrentUser);
                await HttpClient.PutAsJsonAsync($"{Config["apiUrl"]}/updatePassport/{CurrentPassportUser.Id}", CurrentPassportUser);
                StateHasChanged();
                UrlNavigationManager.NavigateTo("/users");
            }
        }
        //private async Task Create()
        //{
        //    var accessToken = await HttpContextAccessor.HttpContext.GetTokenAsync("access_token");
        //    HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        //    var result = await HttpClient.PostAsJsonAsync("/api/user/", user);
        //}

        //private async Task Update(string id)
        //{

        //}
    }
}
//protected override async Task OnInitializedAsync()
//{
//    var accessToken = await HttpContextAccessor.HttpContext.GetTokenAsync("access_token");
//    HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
//    var result = await HttpClient.GetAsync(Config["apiUrl"] + "/users");

//    if (result.IsSuccessStatusCode)
//    {
//        Users1 = await result.Content.ReadFromJsonAsync<List<User>>();
//    }
//}
