namespace Andrei15193.Kepler.AbstractCore
{
    public interface ICilGenerator<TCode>
        where TCode : struct
    {
        void Generate(string assemblyName, string fileName, ParsedNode<TCode> root, CilGeneratorSettings settings);
    }
}
