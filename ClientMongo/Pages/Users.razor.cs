using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using ProjectMongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace ClientMongo.Pages
{
    public partial class Users
    {
        private User SelectedUser { get; set; }
        private List<User> Users1 = new();
        [Inject] private HttpClient HttpClient { get; set; }
        [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; }
        [Inject] private IConfiguration Config { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var accessToken = await HttpContextAccessor.HttpContext.GetTokenAsync("access_token");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var result = await HttpClient.GetAsync(Config["apiUrl"] + "/users");

            if (result.IsSuccessStatusCode)
            {
                Users1 = await result.Content.ReadFromJsonAsync<List<User>>();
            }
        }
        async Task GetUser(string id)
        {
            NavigationManager.NavigateTo($"/userDetail/{id}");
            //SelectedUser = new();
            //SelectedUser = await HttpClient.GetFromJsonAsync<User>($"{Config["apiUrl"]}/UserWithPassport/{id}");
        }
    }
}
