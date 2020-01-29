using AutoLua.Events;
using AutoLua.Views.Logs.Models;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AutoLua.Views.Logs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogHomePage : ContentPage
    {
        /// <summary>
        /// 数据绑定源
        /// </summary>
        private readonly ObservableCollection<LogMessageModel> items = new ObservableCollection<LogMessageModel>();

        public LogHomePage()
        {
            InitializeComponent();
            App.Pages.Add("LogHomePage", this);
            //绑定数据
            Logs.ItemsSource = items;
            //绑定事件
            LogEventDelegates.Instance.Log += OnLog; ;
        }

        private void OnLog(object sender, LogEventArgs e)
        {
            if (e == null)
                return;

            if (items.Count > 200)
            {
                items.Clear();
                items.RemoveAt(items.Count);
            }

            items.Insert(0, new LogMessageModel(e.Type, e.Message)
            {
                TextColor = e.TextColor
            });
        }

        private void ClearLog(object sender, EventArgs e)
        {
            items.Clear();
        }
    }
}