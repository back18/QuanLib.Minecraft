using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.Packet
{
    public static class CommandPacket
    {
        public static RequestPacket CreateRequestPacket(string command, int id, bool needResponse = true)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            return new(PacketKey.Command, PacketType.String, Encoding.UTF8.GetBytes(command), id, needResponse);
        }

        public static string ParseResponsePacket(ResponsePacket responsePacket)
        {
            ArgumentNullException.ThrowIfNull(responsePacket, nameof(responsePacket));

            return Encoding.UTF8.GetString(responsePacket.Data);
        }

        public static async Task<string> SendCommandAsync(this McapiClient client, string command)
        {
            RequestPacket request = CreateRequestPacket(command, client.GetNextID(), true);
            ResponsePacket response = await client.SendRequestPacketAsync(request).ConfigureAwait(false);
            response.ValidateStatusCode();
            return ParseResponsePacket(response);
        }

        public static async Task SendOnewayCommandAsync(this McapiClient client, string command)
        {
            RequestPacket request = CreateRequestPacket(command, client.GetNextID(), false);
            await client.SendRequestPacketAsync(request);
        }
    }
}
