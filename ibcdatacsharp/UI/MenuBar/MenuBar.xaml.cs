﻿using ibcdatacsharp.UI.ToolBar.Enums;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ibcdatacsharp.UI.MenuBar
{
    /// <summary>
    /// Lógica de interacción para MenuBar.xaml
    /// </summary>
    public partial class MenuBar : Page
    {
        public MenuBar()
        {
            InitializeComponent();
            deactivateButtons();
        }
        private void deactivateButtons()
        {
            connect.IsEnabled = false;
            disconnect.IsEnabled = false;
            openCamera.IsEnabled = false;
            record.IsEnabled = false;
            capture.IsEnabled = false;
            pause.IsEnabled = false;
            stop.IsEnabled = false;
            capturedFiles.IsEnabled = false;
        }
        public void activateButtons()
        {
            connect.IsEnabled = true;
            disconnect.IsEnabled = true;
            openCamera.IsEnabled = true;
            record.IsEnabled = true;
            capture.IsEnabled = true;
            pause.IsEnabled = true;
            stop.IsEnabled = true;
            capturedFiles.IsEnabled = true;
        } 
        // Cambia el icono del boton pause
        public void changePauseState(PauseState pauseState)
        {
            if (pauseState == PauseState.Pause)
            {
                pauseImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/Blue/pause-blue-icon.png"));
                pause.Header = "Pause";
            }
            else if (pauseState == PauseState.Play)
            {
                pauseImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/Blue/play-pause-blue-icon.png"));
                pause.Header = "Play";
            }
        }
        // Cambia el icono del boton Record
        public void changeRecordState(RecordState recordState)
        {
            if (recordState == RecordState.RecordStopped)
            {
                recordImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/Blue/record-stop-blue-icon.png"));
                record.Header = "Record Stopped";
            }
            else if (recordState == RecordState.Recording)
            {
                recordImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/record-recording-icon.png"));
                record.Header = "Recording...";
            }
        }
        // Muestra la ventana de la version
        private void showVersion(object sender, RoutedEventArgs e)
        {
            Version version = new Version();
            version.ShowDialog();
        }
    }
}
