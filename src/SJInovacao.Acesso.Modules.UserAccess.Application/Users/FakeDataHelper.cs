using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users
{
    public static class FakeDataHelper
    {
        private static readonly Random _random = new Random();
        private static readonly string[] _fakeStreets =
        {
            //"Rua das Flores", "Avenida Paulista", "Rua Augusta",
            //"Praça da Sé", "Rua Oscar Freire", "Avenida Brasil",
            //"Rua do Comércio", "Avenida Central", "Rua das Acácias"
            "...."
        };

        private static readonly string[] _fakeNeighborhoods =
        {
            //"Centro", "Jardins", "Vila Mariana", "Moema",
            //"Pinheiros", "Brooklin", "Itaim Bibi", "Perdizes"
            "...."
        };

        private static readonly string[] _fakeCities =
        {
            //"São Paulo", "Rio de Janeiro", "Belo Horizonte",
            //"Curitiba", "Porto Alegre", "Salvador", "Brasília"
            "...."
        };

        private static readonly string[] _fakeStates =
        {
            //"SP", "RJ", "MG", "PR", "RS", "BA", "DF",
            ".."
        };

        public static string GetFakePhoneNumber()
        {
            //var ddd = "00";// _random.Next(11, 99);
            //var number = "00000"; // _random.Next(90000, 99999);
            //var digit = "0000"; //_random.Next(1000, 9999);
            return "+5100000000000";// $"({ddd}) {number}-{digit}";
        }

        public static string GetFakeStreet()
        {
            return _fakeStreets[_random.Next(_fakeStreets.Length)];
        }

        public static string GetFakeNumber()
        {
            return "....";// _random.Next(1, 9999).ToString();
        }

        public static string GetFakeNeighborhood()
        {
            return _fakeNeighborhoods[_random.Next(_fakeNeighborhoods.Length)];
        }

        public static string GetFakeCity()
        {
            return _fakeCities[_random.Next(_fakeCities.Length)];
        }

        public static string GetFakeState()
        {
            return _fakeStates[_random.Next(_fakeStates.Length)];
        }

        public static string GetFakeZipCode()
        {
            return $"{_random.Next(1000, 99999)}-{_random.Next(100, 999)}";
        }

        public static (string Latitude, string Longitude) GetFakeGeolocation()
        {
            var lat = 1;// -23.5505 + (_random.NextDouble() - 0.5) * 0.1;
            var lng = 1;// -46.6333 + (_random.NextDouble() - 0.5) * 0.1;
            return (lat.ToString("F6"), lng.ToString("F6"));
        }
    }
}