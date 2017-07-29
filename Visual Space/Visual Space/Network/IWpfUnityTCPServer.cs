using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nollan.Visual_Space.Network
{
    interface IWpfUnityTCPServer
    {
        void Connect();
        void Send(WpfUnityPacketHeader header);
        void Disconnect();

        // 전송완료 이벤트
        event Action<WpfUnityPacketHeader> OnReceviedCompleted;
    }
}
