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
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class InscriptionPageModel : ViewModelBase
    {
        private string _email;
        private string _nom;
        private string _prenom;
        private string _telephone;
        private string _motdepasse;
        private bool _running;

        public bool Running
        {
            get => _running;
            set => SetProperty(ref _running, value);
        }
        public string Email { get => _email;
            set => SetProperty(ref _email, value);
        }
        public string Nom { get => _nom;
            set => SetProperty(ref _nom, value);
        }
        public string Prenom { get => _prenom;
            set => SetProperty(ref _prenom, value);
        }
        public string Telephone { get => _telephone;
            set => SetProperty(ref _telephone, value);
        }
        public string MotDePasse { get => _motdepasse;
            set => SetProperty(ref _motdepasse, value);
        }

        private ObservableCollection<CreateUserRequest> _user;

        public ObservableCollection<CreateUserRequest> User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public ICommand Inscription { get; }
        public InscriptionPageModel()
        {
            Inscription = new Command(validerInscription);
        }

        public async void validerInscription(Object obj)
        {
            Running = true;

            IPizzaApiService service = DependencyService.Get<IPizzaApiService>();

            string myJson = "{'client_id': 'MOBILE', 'client_secret': 'UNIV','email':'"+Email+"',  'first_name': '"+ Nom +"',  'last_name': '"+ Prenom +"',  'phone_number': '"+ Telephone +"',  'password': '"+ MotDePasse +"'}";
            var response = await service.CreateUser(myJson);
            var rep = (JObject)JsonConvert.DeserializeObject(@"" + response);

            if (rep["is_success"].ToString() == "True")
            {
                Console.WriteLine($"Appel HTTP : {response}");
                Running = false;
                await App.Current.MainPage.DisplayAlert("Confirmation", "Votre compte a été enregistré avec succès.", "OK");
                await NavigationService.PushAsync<Pages.ConnexionPage>();
            }
            else
            {
                Running = false;
                await App.Current.MainPage.DisplayAlert("Attention", rep["error_message"].ToString(), "OK");

            }
        }
    }
}