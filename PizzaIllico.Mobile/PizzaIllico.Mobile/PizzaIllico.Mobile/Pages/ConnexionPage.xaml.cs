using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Storm.Mvvm;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PizzaIllico.Mobile.ViewModels;

namespace PizzaIllico.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnexionPage : ContentPage
    {
        public ConnexionPage()
        {         
            BindingContext = new ConnexionPageModel();
            InitializeComponent();
        }
    }
}