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
using Microsoft.JSInterop;


namespace ClientMongo.Pages
{
    public partial class UserWithPassport
    {
        private List<User> UsersWithPassport = new();
        private string firstName;
        private string lastName;
        private string nationality;
        private string gender;
        private string? errorMessage = null;
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
                var response = await HttpClient.GetAsync($"{Config["apiUrl"]}/UserWithPassport?firstName={firstName}&lastName={lastName}&nationality={nationality}&gender={gender}");
                if(response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    errorMessage = "You do not have permission to access this page.";
                    return;
                }
                else if (!response.IsSuccessStatusCode)
                {
                    errorMessage = $"An error occurred: {response.ReasonPhrase}";
                    return;
                }
                UsersWithPassport = await response.Content.ReadFromJsonAsync<List<User>>();
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message, ex);
            }
        }
        void EditUser(string id)
        {
            NavigationManager.NavigateTo($"/editUser/{id}");
        }
        void AddUser()
        {
            NavigationManager.NavigateTo($"/editUser");
        }
        async Task DeleteUser(string id)
        {
            bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this user?");
            if (!confirmed)
            {
                return; // Stop deletion if user cancels
            }

            var accessToken = await HttpContextAccessor.HttpContext.GetTokenAsync("access_token");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            List<User> Users = await HttpClient.GetFromJsonAsync<List<User>>($"{Config["apiUrl"]}/UserWithPassport/{id}");
            var user = Users[0];
            if (user.Passport.Image != null)
            {
                var image = user.Passport.Image;
                var response = await HttpClient.DeleteAsync($"{Config["apiUrl"]}/api/UserImage/{image.Id}");
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Image of {user.FirstName} has been deleted successfully.");
                }
            }
            else
            {
                Console.WriteLine($"Image of {user.FirstName} hasn't been found.");
            }

            if (user != null && user.Passport != null)
            {
                var passportUser = user.Passport;
                var response = await HttpClient.DeleteAsync($"{Config["apiUrl"]}/api/PassportUser/{passportUser.Id}");
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Passport data of {user.FirstName} has been deleted successfully.");
                }
            }
            else
            {
                Console.WriteLine($"Failed to delete user passport data");
            }

            if (!string.IsNullOrEmpty(id))
            {
                var response = await HttpClient.DeleteAsync($"{Config["apiUrl"]}/api/User/{id}");
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"User {user.FirstName} {user.LastName} has been deleted successfully.");
                    StateHasChanged();
                    NavigationManager.NavigateTo("/users");
                }
                else
                {
                    Console.WriteLine($"Failed to delete user {id}: {response.ReasonPhrase}");
                }
            }
        }
    }
}