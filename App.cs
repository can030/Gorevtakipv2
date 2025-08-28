using Gorevtakipv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Gorevtakipv2.adminpncr;
namespace Gorevtakipv2;

public partial class App : Application
{
    protected override Window CreateWindow(IActivationState activationState) =>
        new Window(new Login())
        {
            X = 100,
            Y = 100
        };

    public App()
    {
        

        // Başlangıçta koyu tema açılır
        tema.KoyuTema();
    }
}