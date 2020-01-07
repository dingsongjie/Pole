namespace Pole.Grpc.ExtraType
{
    public partial class ProtoDecimal
    {
        public ProtoDecimal(int v1, int v2, int v3, int v4)
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
            V4 = v4;
        }

        public static implicit operator decimal(ProtoDecimal protoDeciaml) => protoDeciaml.ToDecimal();

        public static implicit operator ProtoDecimal(decimal value) => FromDecimal(value);
        public decimal ToDecimal()
        {
            return new decimal(new int[] { V1, V2, V3, V4 });
        }
        public static ProtoDecimal FromDecimal(decimal value)
        {
            var bits = decimal.GetBits(value);
            return new ProtoDecimal(bits[0], bits[1], bits[2], bits[3]);
        }
    }
}
