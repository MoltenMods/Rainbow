using Hazel;
using Rainbow.Extensions;
using Reactor;
using Reactor.Networking;
using UnityEngine;

namespace Rainbow.Net
{
    [RegisterCustomRpc((uint) CustomRpcCalls.SetColor)]
    public class SetColorRpc : PlayerCustomRpc<RainbowPlugin, SetColorRpc.Data>
    {
        public SetColorRpc(RainbowPlugin plugin, uint id) : base(plugin, id) {}
        
        public readonly struct Data
        {
            public readonly Color32 FrontColor;
            public readonly Color32 BackColor;

            public readonly string ColorName;
            public readonly string ShortColorName;

            public Data(Color32 frontColor, Color32 backColor, string colorName, string shortColorName)
            {
                FrontColor = frontColor;
                BackColor = backColor;
                
                ColorName = colorName;
                ShortColorName = shortColorName;
            }
        }

        public override RpcLocalHandling LocalHandling => RpcLocalHandling.None;

        public override void Write(MessageWriter writer, Data data)
        {
            writer.Write(data.FrontColor.r);
            writer.Write(data.FrontColor.g);
            writer.Write(data.FrontColor.b);
            writer.Write(data.FrontColor.a);
            
            writer.Write(data.BackColor.r);
            writer.Write(data.BackColor.g);
            writer.Write(data.BackColor.b);
            writer.Write(data.BackColor.a);
            
            writer.Write(data.ColorName);
            writer.Write(data.ShortColorName);
        }

        public override Data Read(MessageReader reader)
        {
            return new Data(new Color32(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte()),
                new Color32(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte()),
                reader.ReadString(), reader.ReadString());
        }

        public override void Handle(PlayerControl innerNetObject, Data data)
        {
            innerNetObject.SetCustomColor(data.FrontColor, data.BackColor, data.ColorName, data.ShortColorName);
        }
    }
}