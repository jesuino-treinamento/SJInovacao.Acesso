using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects
{
    public class Document
    {
        public string Number { get; set; } = string.Empty;
        public PersonType PersonType { get; set; }

        private static readonly int[] CpfMultiplier1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        private static readonly int[] CpfMultiplier2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        private static readonly int[] CnpjMultiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        private static readonly int[] CnpjMultiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        public Document()
        {
            
        }

        public Document(string number, PersonType personType)
        {
            number = RemoveNonDigits(number);

            if (!Validate(number, personType))
                throw new DomainException("Invalid document number.");

            Number = number;
            PersonType = personType;
        }

        private static bool Validate(string number, PersonType personType)
        {
            return personType switch
            {
                PersonType.Fisica => ValidateCpf(number),
                PersonType.Juridica => ValidateCnpj(number),
                _ => false
            };
        }

        private static bool ValidateCpf(string cpf)
        {
            if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
                return false;

            return ValidateDigits(cpf, CpfMultiplier1, CpfMultiplier2);
        }

        private static bool ValidateCnpj(string cnpj)
        {
            if (cnpj.Length != 14 || cnpj.Distinct().Count() == 1)
                return false;

            return ValidateDigits(cnpj, CnpjMultiplier1, CnpjMultiplier2);
        }

        private static bool ValidateDigits(string document, int[] multiplier1, int[] multiplier2)
        {
            string tempDoc = document.Substring(0, multiplier1.Length);
            int firstDigit = CalculateDigit(tempDoc, multiplier1);
            int secondDigit = CalculateDigit(tempDoc + firstDigit, multiplier2);

            return document.EndsWith($"{firstDigit}{secondDigit}");
        }

        private static int CalculateDigit(string document, int[] multiplier)
        {
            int sum = document.Select((t, i) => (t - '0') * multiplier[i]).Sum();
            int remainder = sum % 11;
            return remainder < 2 ? 0 : 11 - remainder;
        }

        private static string RemoveNonDigits(string text)
        {
            return new string(text.Where(char.IsDigit).ToArray());
        }
    }
}