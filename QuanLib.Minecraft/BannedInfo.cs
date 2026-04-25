using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace QuanLib.Minecraft
{
    public class BannedInfo
    {
        public const string FOREVER = "forever";

        public BannedInfo(string source, string reason, DateTimeOffset created, DateTimeOffset expires)
        {
            ArgumentException.ThrowIfNullOrEmpty(source, nameof(source));
            ArgumentException.ThrowIfNullOrEmpty(reason, nameof(reason));

            Source = source;
            Reason = reason;
            Created = created;
            Expires = expires;
        }

        public BannedInfo(Model model)
        {
            NullValidator.ValidateObject(model, nameof(model));

            Source = model.source;
            Reason = model.reason;
            Created = DateTimeOffset.ParseExact(model.created, "yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture);
            Expires = model.expires == FOREVER ?
                DateTimeOffset.MaxValue :
                DateTimeOffset.ParseExact(model.expires, "yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture);
        }

        public string Source { get; }

        public string Reason { get; }

        public DateTimeOffset Created { get; }

        public DateTimeOffset Expires { get; }

        public class Model
        {
            public required string source { get; set; }

            public required string reason { get; set; }

            public required string created { get; set; }

            public required string expires { get; set; }
        }
    }
}
