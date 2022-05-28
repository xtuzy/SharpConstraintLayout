using SharpConstraintLayout.Maui.Example.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Example.ViewModels
{
    internal class ListViewViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {

        private List<MicrosoftNews> news;
        public List<MicrosoftNews> News
        {
            get => news;
            set => SetProperty(ref news, value);
        }

    }
}
