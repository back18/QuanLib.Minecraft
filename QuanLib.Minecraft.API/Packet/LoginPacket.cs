﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.Packet
{
    public static class LoginPacket
    {
        public static RequestPacket CreateRequestPacket(string password, int id)
        {
            ArgumentException.ThrowIfNullOrEmpty(password, nameof(password));

            return new(PacketKey.Login, PacketType.String, Encoding.UTF8.GetBytes(password), id, true);
        }

        public static ResponseData ParseResponsePacket(ResponsePacket responsePacket)
        {
            ArgumentNullException.ThrowIfNull(responsePacket, nameof(responsePacket));

            return responsePacket.Data.DeserializeBson<ResponseData>();
        }

        public static async Task<ResponseData> SendLoginAsync(this McapiClient client, string password)
        {
            RequestPacket request = CreateRequestPacket(password, client.GetNextID());
            ResponsePacket response = await client.SendRequestPacketAsync(request);
            response.ValidateStatusCode();
            return ParseResponsePacket(response);
        }

        public class ResponseData
        {
            public bool? IsSuccessful { get; set; }

            public string? Message { get; set; }
        }
    }
}
