using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Storm.Mvvm;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PizzaIllico.Mobile.ViewModels;
using Storm.Mvvm.Navigation;
using PizzaIllico.Mobile.Dtos.Pizzas;

namespace PizzaIllico.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PanierPage
    {
        public PanierPage()
        {
            BindingContext = new PanierPageModel();
            InitializeComponent();
        }
    }
}