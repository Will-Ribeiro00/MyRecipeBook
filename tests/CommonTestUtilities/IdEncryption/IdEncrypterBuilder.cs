using Sqids;

namespace CommonTestUtilities.IdEncryption
{
    public class IdEncrypterBuilder
    {
        public static SqidsEncoder<long> Build()
        {
            return new SqidsEncoder<long>(new()
            {
                MinLength = 3,
                Alphabet = "lKBNsyI2Xa0OY6xLnRgdPfp47STDerFoiVEZwmWhkjQqvM1cCbJHG9Az5tu8U3"
            });
        }
    }
}
