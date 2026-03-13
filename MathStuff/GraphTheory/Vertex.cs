using NoParamlessCtor.Shared.Attributes;

namespace MathStuff.GraphTheory
{
    [NoParamlessCtor]
    public readonly partial record struct Vertex(int Value)
    {
        public static implicit operator Vertex(int value) => new(value);

        public static implicit operator int(Vertex vertex) => vertex.Value;

        public override string ToString()
        {
            return $"V{Value}";
        }
    }
}