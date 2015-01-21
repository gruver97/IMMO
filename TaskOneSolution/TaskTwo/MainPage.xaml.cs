// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

using System;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.Web;

namespace TaskTwo
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MessageWebSocket _messageWebSocket;
        private DataWriter _messageWriter;

        public MainPage()
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        ///     Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">
        ///     Event data that describes how this page was reached.
        ///     This parameter is typically used to configure the page.
        /// </param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                MessageWebSocket webSocket = _messageWebSocket;

                if (webSocket == null)
                {
                    var server = new Uri("wss://echo.websocket.org");
                    webSocket = new MessageWebSocket();
                    webSocket.Control.MessageType = SocketMessageType.Utf8;
                    webSocket.MessageReceived += MessageReceived;
                    webSocket.Closed += Closed;

                    await webSocket.ConnectAsync(server);
                    ConnectionTextBlock.Text = "Connected";
                    _messageWebSocket = webSocket;
                    _messageWriter = new DataWriter(webSocket.OutputStream);
                }
            }
            catch (Exception ex)
            {
                WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                ConnectionTextBlock.Text = status.ToString();
            }
        }

        private async void MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                using (DataReader reader = args.GetDataReader())
                {
                    reader.UnicodeEncoding = UnicodeEncoding.Utf8;
                    string read = reader.ReadString(reader.UnconsumedBufferLength);
                    await
                        CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            EchoTextBox.Text = read;
                            SendButton.IsEnabled = true;
                        });
                }
            }
            catch (Exception ex)
            {
                WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                SendButton.IsEnabled = true;
            }
        }

        private void Closed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            MessageWebSocket webSocket = Interlocked.Exchange(ref _messageWebSocket, null);
            if (webSocket != null)
            {
                webSocket.Dispose();
            }
        }

        private async void SendButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            SendButton.IsEnabled = false;
            try
            {
                MessageWebSocket webSocket = _messageWebSocket;

                _messageWebSocket = webSocket;
                _messageWriter = new DataWriter(webSocket.OutputStream);

                string message = string.IsNullOrWhiteSpace(MessageTextBox.Text) ? "DefaultMessage" : MessageTextBox.Text;

                _messageWriter.WriteString(message);

                await _messageWriter.StoreAsync();
            }
            catch (Exception ex)
            {
                WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                ConnectionTextBlock.Text = status.ToString();
            }
        }
    }
}