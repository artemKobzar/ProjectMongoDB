using ClientMongo.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Rendering;
using ProjectMongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Helpers;


namespace ClientMongo.Pages
{
    public partial class UserWithPassport
    {
        private List<User> UsersWithPassport = new();
        private string firstName;
        private string lastName;
        private string nationality;
        private string gender;
        [Inject] private HttpClient HttpClient { get; set; }
        [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; }
        [Inject] private IConfiguration Config { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        protected override async Task OnInitializedAsync()
        {
            var accessToken = await HttpContextAccessor.HttpContext.GetTokenAsync("access_token");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            try
            {
                var result = await HttpClient.GetAsync($"{Config["apiUrl"]}/UserWithPassport?firstName={firstName}&lastName={lastName}&nationality={nationality}&gender={gender}");
                UsersWithPassport = await result.Content.ReadFromJsonAsync<List<User>>();
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message, ex);
            }


            //if (result.IsSuccessStatusCode)
            //{
            //    UsersWithPassport = await result.Content.ReadFromJsonAsync<List<User>>();
            //}
        }
        void EditUser(string id)
        {
            NavigationManager.NavigateTo($"/editUser/{id}");
        }
        void AddUser()
        {
            NavigationManager.NavigateTo($"/editUser");
        }

    }
}