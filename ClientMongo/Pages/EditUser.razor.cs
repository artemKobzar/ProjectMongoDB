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
using System.Reflection;
using MongoDB.Bson;
using Microsoft.AspNetCore.Components.Forms;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;


namespace ClientMongo.Pages
{
    public partial class EditUser
    {
        [Parameter]
        public string? Id { get; set; }
        private List<User> UsersWithPassport = new();
        public User CurrentUser { get; set; } = new();
        public PassportUser CurrentPassportUser { get; set; } = new();
        private EditContext editContext;
        private ValidationMessageStore validationMessages;
        private string? errorMessage;
        private IBrowserFile? SelectedFile { get; set; }
        [Inject] private HttpClient HttpClient { get; set; }
        [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; }
        [Inject] private IConfiguration Config { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            var accessToken = await HttpContextAccessor.HttpContext.GetTokenAsync("access_token");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            if (Id != null)
            {
                List<User> Users = await HttpClient.GetFromJsonAsync<List<User>>($"{Config["apiUrl"]}/UserWithPassport/{Id}");
                var user = Users[0];
                if (user != null)
                {
                    CurrentUser = user;
                    CurrentPassportUser = CurrentUser.Passport;
                }
            }
            editContext = new EditContext(CurrentUser);
            validationMessages = new ValidationMessageStore(editContext);
        }
        private async Task HandleFileSelected (InputFileChangeEventArgs e)
        {
            SelectedFile = e.File;
        }
        private async Task UploadFile()
        {
            if (SelectedFile == null)
            {
                Console.WriteLine("No file selected!");
                return;
            }
            var accessToken = await HttpContextAccessor.HttpContext.GetTokenAsync("access_token");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            using var content = new MultipartFormDataContent();
            using var filestream = SelectedFile.OpenReadStream(1024*1024);
            content.Add(new StreamContent(filestream), "file", SelectedFile.Name);
            
            var fileName = Path.GetFileNameWithoutExtension(SelectedFile.Name);
            var passportId = CurrentPassportUser.Id;

            var response = await HttpClient.PostAsync($"{Config["apiUrl"]}/api/UserImage/upload?name={fileName}&passportId={passportId}", content);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("File has been uploaded successfully");
            }
            else
            {
                Console.WriteLine($"File upload failed: {response.ReasonPhrase}");
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
                    CurrentUser.Id = ObjectId.GenerateNewId().ToString();
                    //CurrentPassportUser.Id = Guid.NewGuid().ToString("N");
                    CurrentPassportUser.Id = ObjectId.GenerateNewId().ToString();
                    
                    await HttpClient.PostAsJsonAsync($"{Config["apiUrl"]}/AddUser", CurrentUser);
                    CurrentPassportUser.UserId = CurrentUser.Id;
                    //CurrentUser.Passport = CurrentPassportUser;
                    await HttpClient.PostAsJsonAsync($"{Config["apiUrl"]}/AddPassport", CurrentPassportUser);
                    if (SelectedFile !=  null)
                    {
                        await UploadFile();
                    }
                    NavigationManager.NavigateTo("/users");
                }
                catch (BadHttpRequestException ex)
                {
                    throw new BadHttpRequestException(ex.Message);
                }
                catch (Exception ex)
                {
                    errorMessage = $"An error occurred: {ex.Message}";
                    StateHasChanged();  //  Ensure UI updates
                }
            }
            else
            {
                CurrentUser.Passport = CurrentPassportUser;
                await HttpClient.PutAsJsonAsync($"{Config["apiUrl"]}/updateUser/{CurrentUser.Id}", CurrentUser);
                await HttpClient.PutAsJsonAsync($"{Config["apiUrl"]}/updatePassport/{CurrentPassportUser.Id}", CurrentPassportUser);
                if (SelectedFile != null)
                {
                    await UploadFile();
                }
                StateHasChanged();
                NavigationManager.NavigateTo("/users");
            }
        }
    }
}