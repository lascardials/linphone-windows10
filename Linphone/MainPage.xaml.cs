﻿/*
MainPage.xaml.cs
Copyright (C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using Windows.UI.Xaml.Controls;
using BelledonneCommunications.Linphone.Native;
using Linphone.Model;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Linphone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Core.LogLevel = OutputTraceLevel.Message;
            LinphoneManager.Instance.Dispatcher = Dispatcher;
            LinphoneManager.Instance.Core.IsIterateEnabled = true;
        }

        private void CallButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Core core = LinphoneManager.Instance.Core;
            if (core.CallsNb == 0)
            {
                string contact = ContactTextBox.Text;
                if (contact.Length > 0)
                {
                    Address address = core.InterpretURL(contact);
                    if (address != null)
                    {
                        core.InviteAddress(address);
                    }
                }
            }
            else
            {
                foreach (Call c in core.Calls)
                {
                    core.TerminateCall(c);
                }
            }
        }

        private void RegisterButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if ((UsernameTextBox.Text.Length > 0) && (ServerTextBox.Text.Length > 0))
            {
                Core core = LinphoneManager.Instance.Core;
                ProxyConfig proxy = core.CreateProxyConfig();
                AuthInfo authInfo = core.CreateAuthInfo(UsernameTextBox.Text, "", PasswordBox.Password, "", "", "");
                core.AddAuthInfo(authInfo);
                proxy.Identity = string.Format("sip:{0}@{1}", UsernameTextBox.Text, ServerTextBox.Text);
                proxy.ServerAddr = ServerTextBox.Text;
                Address addr = core.CreateAddress(proxy.ServerAddr);
                addr.Transport = Transport.TCP;
                proxy.ServerAddr = addr.AsString();
                proxy.Route = addr.AsString();
                proxy.IsRegisterEnabled = true;
                core.AddProxyConfig(proxy);
                core.DefaultProxyConfig = proxy;
                core.VideoPolicy = new VideoPolicy(false, false);
            }
        }
    }
}
