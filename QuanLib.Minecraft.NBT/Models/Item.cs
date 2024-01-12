﻿using Newtonsoft.Json.Linq;
using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.NBT.Models
{
    public class Item
    {
        public Item(Model model)
        {
            NullValidator.ValidateObject(model, nameof(model));

            Count = model.Count;
            ID = model.id;
            Slot = model.Slot;
            Tag = model.tag;
        }

        public Item(sbyte count, string iD, sbyte slot, Dictionary<string, object>? tag = null)
        {
            Count = count;
            ID = iD;
            Slot = slot;
            Tag = tag;
        }

        public sbyte Count { get; }

        public string ID { get; }

        public sbyte Slot { get; }

        public Dictionary<string, object>? Tag { get; }

        public string GetItemName()
        {
            if (Tag is not null &&
                Tag.TryGetValue("display", out var display) &&
                display is Dictionary<string, object> displayTag &&
                displayTag.TryGetValue("Name", out var name) &&
                name is string nameString)
            {
                try
                {
                    JObject nameJson = JObject.Parse(nameString);
                    if (nameJson.TryGetValue("text", out var text) && text is JValue textValue && textValue.Value is string textString)
                    {
                        return textString;
                    }
                }
                catch
                {

                }
            }

            return ID;
        }

        public override string ToString()
        {
            return ID;
        }

        public static bool EqualsID(Item? item1, Item? item2)
        {
            return string.Equals(item1?.ID, item2?.ID);
        }

        public class Model
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public sbyte Count { get; set; }

            public string id { get; set; }

            public sbyte Slot { get; set; }

            [Nullable]
            public Dictionary<string, object>? tag { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        }
    }
}
