using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Pizzas;
using PizzaIllico.Mobile.Services;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using Xamarin.Forms;



namespace PizzaIllico.Mobile.ViewModels
{
    public class PanierPageModel : ViewModelBase
    {
        private ShopItem _shop;
        private string _token;
        private List<long> _commandIds;
        private ObservableCollection<PizzaItem> _pizzasCommand;
        private string _total;
        private string _erreur ="";
        public string Erreur
        {
            get => _erreur;
            set => SetProperty(ref _erreur, value);
        }
        public string Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
        }

        public ICommand SelectedCommand { get; }
        public ICommand Continuer { get; }
        public ICommand Commander { get; }
        public PanierPageModel()
        {
            SelectedCommand = new Command<PizzaItem>(SelectedAction);
            Continuer = new Command(ContinuerCommande);
            Commander = new Command(CommandPizzas);
        }
        public async void CommandPizzas()
        {
            CreateOrderRequest cor = new CreateOrderRequest();
            cor.PizzaIds = CommandIds;
            string myJson = JsonConvert.SerializeObject(cor);
            Console.WriteLine("CreateOrderRequest : " + myJson);

            IPizzaApiService service = DependencyService.Get<IPizzaApiService>();

            string response = await service.PostCommand(Token, Shop.Id, myJson);
            var tokenUser = (JObject)JsonConvert.DeserializeObject(@"" + response);

            if (tokenUser["is_success"].ToString() == "True")
            {
                Console.WriteLine("Post Command : " + response);
                await App.Current.MainPage.DisplayAlert("Confirmation", "Votre commande a été prise en compte", "OK");
                await NavigationService.PushAsync<Pages.ShopListPage>(
                new Dictionary<string, object>()
                {
                                { "Token", Token }
                }
            );
            }
            else
            {
                Erreur += "Une erreur est survenue. " + tokenUser["error_message"];
            }
        }
        private async void ContinuerCommande()
        {
            await NavigationService.PushAsync<Pages.PizzaListPage>(
                new Dictionary<string, object>()
                {
                                { "Token", Token },
                                { "CommandIds", CommandIds },
                                { "Shop", Shop }
                }
            );
        }

        private void SelectedAction(PizzaItem obj)
        {
            Console.WriteLine(obj.Name);
            for (int i = 0; i < CommandIds.Count; i++)
            {

                    if (CommandIds[i] == obj.Id)
                    {
                        CommandIds.RemoveAt(i);

                    }
                    if (PizzaCommand[i].Id == obj.Id)
                    {
                        PizzaCommand.RemoveAt(i);
                    }
                
            }
            double tot = 0;
            for (int i = 0; i < PizzaCommand.Count; i++)
            {
                tot += PizzaCommand[i].Price;
            }
            Total = tot.ToString();
        }

        [NavigationParameter]
        public List<long> CommandIds
        {
            get { return _commandIds; }
            set { SetProperty<List<long>>(ref _commandIds, value); }
        }

        [NavigationParameter]
        public ShopItem Shop
        {
            get { return _shop; }
            set { SetProperty<ShopItem>(ref _shop, value); }
        }

        [NavigationParameter]
        public string Token
        {
            get { return _token; }
            set { SetProperty<string>(ref _token, value); }
        }

        [NavigationParameter]
        public ObservableCollection<PizzaItem> PizzaCommand
        {
            get { return _pizzasCommand; }
            set { SetProperty<ObservableCollection<PizzaItem>>(ref _pizzasCommand, value); }
        }

        public override void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);
            Console.WriteLine("SALUT");
            Shop = GetNavigationParameter<ShopItem>("Shop");
            CommandIds = GetNavigationParameter<List<long>>("CommandIds");
            Token = GetNavigationParameter<string>("Token");
            PizzaCommand = GetNavigationParameter<ObservableCollection<PizzaItem>>("CommandPizzas");

            double tot = 0;
            for(int i=0; i < PizzaCommand.Count; i++)
            {
                tot += PizzaCommand[i].Price;
            }
            Total = tot.ToString();
        }
    }
}