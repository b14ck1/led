using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Led.Interfaces;

namespace Led.Services
{
    public class MediatorService : IMediator
    {
        public System.Windows.Window MainWindow { private get; set; }

        /// <summary>
        /// All Participants which send and recieve Messages
        /// </summary>
        private List<IParticipant> _Participants;        

        public void Register(IParticipant sender)
        {
            if (!_Participants.Contains(sender))
                _Participants.Add(sender);
        }

        public void Unregister(IParticipant sender)
        {
            if (_Participants.Contains(sender))
                _Participants.Remove(sender);
        }

        public void BroadcastMessage(MediatorMessages message, object sender, object data)
        {
            MainWindow.Dispatcher.BeginInvoke((Action)( () =>
            {
                List<IParticipant> receivers = new List<IParticipant>(_Participants);
                foreach (IParticipant receiver in receivers)
                {
                    if (!receiver.Equals(sender as IParticipant))
                        receiver.RecieveMessage(message, sender, data);
                }
            }));
        }

        public MediatorService()
        {
            _Participants = new List<IParticipant>();
        }
    }
}
