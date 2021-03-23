using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Accounts;
using PizzaIllico.Mobile.Services;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class EditUserPageModel : ViewModelBase
    {

        private string _tokenu;

        private string _email;
        private string _nom;
        private string _prenom;
        private string _telephone;
        private string _changesuccess;

        public string ChangeSuccess
        {
            get => _changesuccess;
            set => SetProperty(ref _changesuccess, value);
        }

        public string Nom
        {
            get => _nom;
            set => SetProperty(ref _nom, value);
        }
        public string Prenom
        {
            get => _prenom;
            set => SetProperty(ref _prenom, value);
        }
        public string Telephone
        {
            get => _telephone;
            set => SetProperty(ref _telephone, value);
        }

        public ICommand MiseAJour { get; }
        public ICommand ChangePwd { get; }

        public EditUserPageModel()
        {
            MiseAJour = new Command(mettreAJour);
            ChangePwd = new Command(goChangePwd);
        }

        public async void goChangePwd()
        {
             await NavigationService.PushAsync<Pages.EditPwdPage>(
                 new Dictionary<string, object>()
                 {
                     { "TokenU", TokenU }
                 }
              );
        }

        public async void mettreAJour()
        {
            IPizzaApiService service = DependencyService.Get<IPizzaApiService>();
            string myJson = "{'email':'" + _email + "',  'first_name': '" + Nom + "',  'last_name': '" + Prenom + "',  'phone_number': '" + Telephone + "'}";
            var response = await service.AccountPatch(myJson,TokenU);

            if(response.IsSuccess)
            {
                Console.WriteLine($"Appel HTTP : {response.Data.Email}");
                await App.Current.MainPage.DisplayAlert("Confirmation", "Votre profil a été mis à jour avec succès", "OK");
                await NavigationService.PushAsync<Pages.ShopListPage>(
                new Dictionary<string, object>()
                {
                                { "Token", TokenU }
                }
                );
            }
            else
            {
                ChangeSuccess = "Une erreur s'est produite.";
            }
        }

        [NavigationParameter]
        public string TokenU
        {
            get { return _tokenu; }
            set { SetProperty<string>(ref _tokenu, value); }
        }

        public async override void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);

            TokenU = GetNavigationParameter<string>("TokenU");

            IPizzaApiService service = DependencyService.Get<IPizzaApiService>();

            Response<UserProfileResponse> response = await service.GetUser(TokenU);

            _email = response.Data.Email.ToString();
            Nom = response.Data.FirstName.ToString();
            Prenom = response.Data.LastName.ToString();
            Telephone = response.Data.PhoneNumber.ToString();
        }

    }
}