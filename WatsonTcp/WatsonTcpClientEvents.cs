﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WatsonTcp
{
    /// <summary>
    /// Watson TCP client events.
    /// </summary>
    public class WatsonTcpClientEvents
    {
        #region Public-Members

        /// <summary>
        /// Event fired when authentication has succeeded.
        /// </summary>
        public event EventHandler AuthenticationSucceeded = delegate { };

        /// <summary>
        /// Event fired when authentication has failed.
        /// </summary>
        public event EventHandler AuthenticationFailure = delegate { };

        /// <summary>  
        /// This event is fired when a message is received from the server and it is desired that WatsonTcp pass the byte array containing the message payload. 
        /// If MessageReceived is set, StreamReceived will not be used.
        /// </summary>
        public event EventHandler<MessageReceivedFromServerEventArgs> MessageReceived = delegate { };

        /// <summary> 
        /// This callback is called when a stream is received from the server and it is desired that WatsonTcp pass the stream containing the message payload to your application. 
        /// If MessageReceived is set, StreamReceived will not be used.
        /// </summary>
        public event EventHandler<StreamReceivedFromServerEventArgs> StreamReceived = delegate { };

        /// <summary>
        /// Event fired when the client successfully connects to the server.
        /// </summary>
        public event EventHandler ServerConnected = delegate { };

        /// <summary>
        /// Event fired when the client disconnects from the server.
        /// </summary>
        public event EventHandler ServerDisconnected = delegate { };

        /// <summary>
        /// This event is fired when an exception is encountered.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> ExceptionEncountered = delegate { };

        #endregion

        #region Internal-Members

        internal bool IsUsingMessages
        {
            get
            {
                if (MessageReceived != null && MessageReceived.GetInvocationList().Length > 0) return true;
                return false;
            }
        }

        internal bool IsUsingStreams
        {
            get
            {
                if (StreamReceived != null && StreamReceived.GetInvocationList().Length > 0) return true;
                return false;
            }
        }

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public WatsonTcpClientEvents()
        {

        }

        #endregion

        #region Public-Methods

        #endregion

        #region Internal-Methods

        internal void HandleAuthenticationSucceeded(object sender, EventArgs args)
        {
            CatchAndReport(() => AuthenticationSucceeded?.Invoke(sender, args), "AuthenticationSucceeded", sender);
        }

        internal void HandleAuthenticationFailure(object sender, EventArgs args)
        {
            CatchAndReport(() => AuthenticationFailure?.Invoke(sender, args), "AuthenticationFailure", sender);
        }

        internal void HandleMessageReceived(object sender, MessageReceivedFromServerEventArgs args)
        {
            CatchAndReport(() => MessageReceived?.Invoke(sender, args), "MessageReceived", sender);
        }

        internal void HandleStreamReceived(object sender, StreamReceivedFromServerEventArgs args)
        {
            CatchAndReport(() => StreamReceived?.Invoke(sender, args), "StreamReceived", sender);
        }

        internal void HandleServerConnected(object sender, EventArgs args)
        {
            CatchAndReport(() => ServerConnected?.Invoke(sender, args), "ServerConnected", sender);
        }

        internal void HandleServerDisconnected(object sender, EventArgs args)
        {
            CatchAndReport(() => ServerDisconnected?.Invoke(sender, args), "ServerDisconnected", sender);
        }

        internal void HandleExceptionEncountered(object sender, ExceptionEventArgs args)
        {
            CatchAndReport(() => ExceptionEncountered?.Invoke(sender, args), "ExceptionEncountered", sender);
        }

        #endregion

        #region nested

        public void CatchAndReport(Action action, string context, object sender)
        {
            Action<string> logger = ((WatsonTcpClient)sender).Settings.Logger;
            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                logger("Exception from " + context + " handler: " + e.Message);
            }
        }



        #endregion
    }
}
