﻿using ibcdatacsharp.UI.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ibcdatacsharp.UI.SagitalAngles
{
    /// <summary>
    /// Lógica de interacción para GraphHip.xaml
    /// </summary>
    public partial class GraphHip : Page
    {
        public Model model { get; private set; }
        public GraphHip()
        {
            InitializeComponent();
            model = new Model(plot);
            DataContext = this;
        }
        public void initCapture()
        {
            model.initCapture();
        }
        public async void drawData(float[] data)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.updateData(data);
            });
        }
    }
}
