using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PizzaIllico.Mobile.Services;
using Plugin.Settings;
using Storm.Mvvm;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class ConnexionPageModel : ViewModelBase
    {

        private string _login;
        private bool _running;
        private bool _isCheck;
        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }
        public bool Running
        {
            get => _running;
            set => SetProperty(ref _running, value);
        }
        public bool IsCheck
        {
            get => _isCheck;
            set => SetProperty(ref _isCheck, value);
        }
        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _erreur;
        public string Erreur
        {
            get => _erreur;
            set => SetProperty(ref _erreur, value);
        }

        public ICommand Connexion { get; }
        public ICommand Inscription { get; }
        public ConnexionPageModel()
        {
            Login = "";
            Password = "";
            Connexion = new Command(goSubmit);
            Inscription = new Command(goInscription);
            string log = CrossSettings.Current.GetValueOrDefault("login", string.Empty);
            string pwd = CrossSettings.Current.GetValueOrDefault("password", string.Empty);
            Console.WriteLine("Savedss : " + log + " " + pwd);
            if(!(log == "" && pwd == ""))
            {
                Login = JsonConvert.DeserializeObject<string>(log);
                Password = JsonConvert.DeserializeObject<string>(pwd);
                goSubmit();
            }
        }


        public async void goSubmit()
        {
            Running = true;

            IPizzaApiService service = DependencyService.Get<IPizzaApiService>();

            string myJson = "{ 'login':'" + Login + "','password':'" + Password + "','client_id': 'MOBILE', 'client_secret': 'UNIV'}";

            var response = await service.Login(myJson);
            var tokenUser = (JObject)JsonConvert.DeserializeObject(@"" + response);
            if (tokenUser["is_success"].ToString() == "True")
            {
                if(IsCheck)
                {
                    var login = JsonConvert.SerializeObject(Login);
                    var password = JsonConvert.SerializeObject(Password);
                    CrossSettings.Current.AddOrUpdateValue("login", login);
                    CrossSettings.Current.AddOrUpdateValue("password", password);
                }
                else
                {
                    CrossSettings.Current.Clear();
                }
                await NavigationService.PushAsync<Pages.ShopListPage>(
                    new Dictionary<string, object>()
                    {
                        { "Token", tokenUser["data"]["access_token"].ToString() }
                    }

                );

            }
            else
            {
                Erreur = "Login et/ou mot de passe incorrects";
            }
            Running = false;
        }

        public void goInscription()
        {
            Running = true;
            NavigationService.PushAsync<Pages.InscriptionPage>();
            Running = false;
        }
    }
}