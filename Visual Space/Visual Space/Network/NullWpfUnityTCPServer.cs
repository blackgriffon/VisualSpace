using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nollan.Visual_Space.Network
{
    // 빈 객체를 생성 네트워크 통신을 안할 때는 이것을 사용
    class NullWpfUnityTCPServer : IWpfUnityTCPServer
    {
        public event Action<WpfUnityPacketHeader> OnReceviedCompleted = null;

        public void Connect()
        {

        }

        public void Disconnect()
        {

        }

        public void Send(WpfUnityPacketHeader header)
        {

        }
    }
}
