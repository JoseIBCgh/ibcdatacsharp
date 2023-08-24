using ibcdatacsharp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ibcdatacsharp.UI.Remote
{
    /// <summary>
    /// Lógica de interacción para ChoseUser.xaml
    /// </summary>
    public partial class ChoseUser : Window
    {
        public ChoseUser()
        {
            List<User> users = new List<User>();
            InitializeComponent();
            using (var dbContext = new PrefallContext())
            {
                var user = dbContext.Users.Include(d => d.IdPacientes).FirstOrDefault(user => user.Username == RemoteService.username);
                if (user != null)
                {
                    var roleUser = dbContext.RolesUsers.FirstOrDefault(rolesUsers => rolesUsers.UserId == user.Id);
                    if(roleUser != null)
                    {
                        var role = dbContext.Roles.FirstOrDefault(roles => roles.Id == roleUser.RoleId);
                        if(role != null)
                        {
                            if(role.Name == "paciente")
                            {
                                users.Add(user);
                            }
                            else if(role.Name == "auxiliar")
                            {
                                users = dbContext.Users.Where(paciente => paciente.IdCentro == user.IdCentro).ToList();
                                users = users.Where((u) =>
                                {
                                    var roleU = dbContext.RolesUsers.FirstOrDefault(rolesUsers => rolesUsers.UserId == u.Id);
                                    if (roleU != null)
                                    {
                                        var r = dbContext.Roles.FirstOrDefault(roles => roles.Id == roleU.RoleId);
                                        if (r != null)
                                        {
                                            return r.Name == "paciente";
                                        }
                                    }
                                    return false;
                                }).ToList();
                            }
                            else if(role.Name == "medico")
                            {
                                Trace.WriteLine("IdPacientes");
                                foreach (var u in user.IdPacientes)
                                {
                                    Trace.WriteLine(u.Nombre);
                                }
                                users = user.IdPacientes.ToList(); //No funciona esta vacia
                            }
                            Trace.WriteLine("list of users");
                            foreach(User u in users)
                            {
                                Trace.WriteLine(u.Nombre);
                            }
                        }
                        else
                        {
                            Trace.WriteLine("role == null");
                        }
                    }
                    else
                    {
                        Trace.WriteLine("roleUser == null");
                    }
                }
                else
                {
                    Trace.WriteLine("user == null");
                }
                pacientesListView.ItemsSource = users;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RemoteService.selectedUser = pacientesListView.SelectedItem as User;
            Trace.WriteLine("selected user " + RemoteService.selectedUser.Username);
            Close();
        }
    }
}
