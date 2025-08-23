using Gorevtakipv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
namespace Gorevtakipv2;

public partial class App : Application
{


    protected override Window CreateWindow(IActivationState activationState) =>
        new Window(new Login())

        {


            X = 100,
            Y = 100
        };
}