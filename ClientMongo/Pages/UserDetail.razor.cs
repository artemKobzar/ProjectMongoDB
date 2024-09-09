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
    public partial class UserDetail
    {
        [Parameter]
        public string? Id { get; set; }
        private User SelectedUser { get; set; }
        private UserImage SelectedImage { get; set; }
        private string base64Image;
        public PassportUser SelectedPassportUser { get; set; } = new();
        [Inject] private HttpClient HttpClient { get; set; }
        [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; }
        [Inject] private IConfiguration Config { get; set; }
        protected override async Task OnInitializedAsync()
        {
            var accessToken = await HttpContextAccessor.HttpContext.GetTokenAsync("access_token");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            List<User> Users = await HttpClient.GetFromJsonAsync<List<User>>($"{Config["apiUrl"]}/UserWithPassport/{Id}");
            var user = Users[0];
            if (user != null)
            {
                SelectedUser = user;
                try
                {
                    var image = await HttpClient.GetByteArrayAsync($"{Config["apiUrl"]}/downloadImage/{SelectedUser.Id}");
                    SelectedPassportUser = SelectedUser.Passport;
                    base64Image = Convert.ToBase64String(image);
                    SelectedPassportUser.Image = SelectedImage;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
