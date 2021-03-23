using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Accounts;
using PizzaIllico.Mobile.Services;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.Pages
{
    public class EditPwdPageModel : ViewModelBase
    {
        private string _tokenu;
        private string _changesuccess;
        private string _newpwd;
        private string _oldpwd;

        public string ChangeSuccess
        {
            get => _changesuccess;
            set => SetProperty(ref _changesuccess, value);
        }
        public string NewPassword
        {
            get => _newpwd;
            set => SetProperty(ref _newpwd, value);
        }
        public string OldPassword
        {
            get => _oldpwd;
            set => SetProperty(ref _oldpwd, value);
        }
        public ICommand MiseAJour { get; }

        public EditPwdPageModel()
        {
            MiseAJour = new Command(mettreAJour);
        }

        public async void mettreAJour()
        {
            IPizzaApiService service = DependencyService.Get<IPizzaApiService>();
            string myJson = "{'old_password':'" + OldPassword + "',  'new_password': '" + NewPassword + "'}";
            Console.WriteLine("JSON mdp : " + myJson);
            var response = await service.PasswordPatch(myJson, TokenU);
            var tokenUser = (JObject)JsonConvert.DeserializeObject(@"" + response);
            Console.WriteLine("Appel HTTP isSucceed : " + tokenUser["is_success"].ToString());

            if (tokenUser["is_success"].ToString() == "True")
            {
                await App.Current.MainPage.DisplayAlert("Confirmation", "Votre mot de passe a été mis à jour avec succès", "OK");
                await NavigationService.PushAsync<Pages.ShopListPage>(
                new Dictionary<string, object>()
                {
                                { "Token", TokenU }
                }
                );
            }
            else
            {
                ChangeSuccess = "Une erreur s'est produite. (" + tokenUser["error_message"].ToString() + ")";
            }
        }

        [NavigationParameter]
        public string TokenU
        {
            get { return _tokenu; }
            set { SetProperty<string>(ref _tokenu, value); }
        }

        public override void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);

            TokenU = GetNavigationParameter<string>("TokenU");
        }
    }
}