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
    public class PreviousCommandsPageModel : ViewModelBase
    {
        private string _token;
        private ObservableCollection<OrderItem> _orders;

        public ObservableCollection<OrderItem> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        public ICommand RetourMenu { get; }
        public ICommand SelectedCommand { get; }

        public PreviousCommandsPageModel()
        {
            RetourMenu = new Command(RetournerMenu);
            SelectedCommand = new Command<PizzaItem>(SelectedAction);
        }

        private void SelectedAction(PizzaItem obj)
        {
            Console.WriteLine(obj.Name);
        }

        private async void RetournerMenu()
        {
            await NavigationService.PopAsync();
        }

        [NavigationParameter]
        public string Token
        {
            get { return _token; }
            set { SetProperty<string>(ref _token, value); }
        }

        public override async void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);
            Token = GetNavigationParameter<string>("Token");

            IPizzaApiService service = DependencyService.Get<IPizzaApiService>();

            Response<List<OrderItem>> response = await service.ListOrders(Token);
            if (response.IsSuccess)
            {
                Orders = new ObservableCollection<OrderItem>(response.Data);
                for(int i = 0; i < Orders.Count; i++)
                {
                    Console.WriteLine("Orders : " + Orders[i].Shop + " " + Orders[i].Date);
                }
            }

        }
    }
}