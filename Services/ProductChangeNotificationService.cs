using WebApplication2.Hubs;
using WebApplication2.DTO;
using System;
using Microsoft.AspNetCore.SignalR;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;
using TableDependency.SqlClient.Base;

namespace WebApplication2.Services
{
    public class ProductChangeNotificationService : IProductChangeNotificationService
    {
          SqlTableDependency<Products> _tableDependency;
          ProductHub _hubContext;
        //SqlTableDependency<Product> tableDependency;
        //DashboardHub dashboardHub;

        public ProductChangeNotificationService(ProductHub hubContext)
        {
            _hubContext = hubContext;
            //_tableDependency = tableDependency;
            //_tableDependency = new SqlTableDependency<Product>(connectionString);
            //_tableDependency.OnChanged += OnProductChanged;
            ////_tableDependency.OnError += OnError;
            //_tableDependency.Start();
        }

        //private async void OnProductChanged(object sender, RecordChangedEventArgs<Product> e)
        //{
        //    if (e.ChangeType != ChangeType.None)
        //    {
        //        var updatedProduct = e.Entity;
        //        var message = $"Product {updatedProduct.Name} has been {e.ChangeType.ToString().ToLower()} (Quantity: {updatedProduct.Quantity})";
        //        await _hubContext.Clients.All.SendAsync("ReceiveProductUpdate", message);
        //    }
        //}
        private async void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Products> e)
        {
            if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
            {
                var updatedProduct = e.Entity;
                var message = $"Product {updatedProduct.Name} has been {e.ChangeType.ToString().ToLower()} (Quantity: {updatedProduct.Quantity})";

                await _hubContext.SendProductUpdate(message);
            }
        }
        public void SubscribeTableDependency(string connectionString)
        {
            _tableDependency = new SqlTableDependency<Products>(connectionString);
            _tableDependency.OnChanged += TableDependency_OnChanged;
            _tableDependency.OnError += TableDependency_OnError;
            _tableDependency.Start();
        }
        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            Console.WriteLine($"{nameof(Products)} SqlTableDependency error: {e.Error.Message}");
        }
    }
}